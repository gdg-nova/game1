using UnityEngine;
using System.Collections;

/// <summary>
/// The idea is to add this component to an object to give it additional attacking related states.
/// </summary>
/// <remarks>
/// This components provides the following states which can be used for the state machine:
/// - striking = Attacking another object
///   - while in striking state, the object cannot move and is playing L2R_swipe.
/// - being_hit - this state is entered if an enemy member hits this object. No attacking can be done in this state.
/// The following actions are added:
/// - new_target[Vector3] - this is sent when we're close enough to an attackable object and we choose to attack it
/// - strike[Transform] this is used to say that a particular object has been hit. This may not be the same the object that we're going after.
/// - hit - When the cast member is hit by someone else.
/// - killed - When the cast member is killed (Die state not implemented by this module)
/// </remarks>
public class AttackBehavior : MonoBehaviour {

	public StateMachineDriver stateMachineDriver;

	public float intervalBetweenStrikes = 0.5f;
	public float strikeRange = 2.0f;

	public string[] tagsToAttack;

	public float damage = 1.0f;

	public float health = 1.0f;

	private Animation animationComp;

	public string nameOfAttackAnimation = "L2R_swipe";
	public string nameOfHurtAnimation = "hurt";

	class Striking : StateImplementationHelper<AttackBehavior>, IStateImplementation
	{
		public Striking(AttackBehavior zombie)
			: base(zombie)
		{
		}
		
		#region IStateImplementation implementation

		private float m_animationTimeRemaining = 0.0f;

		public void OnStateEntry (string entryAction, object[] parameters)
		{
			if (parameters.Length != 1)
			{
				Debug.LogError("Expected a single parameter of type 'Transform'");
				return;
			}
			Transform hitObj = parameters[0] as Transform;
			if (hitObj == null)
			{
				// Maybe the object was destroyed before we got to it.
				m_userObj.stateMachineDriver.AddAction("go_to_target");
				return;
			}
			m_userObj.transform.LookAt(hitObj.position);

			// based on the initial animation, what is their respective
			// attack animation name string
			m_userObj.animationComp.wrapMode = WrapMode.Once;
			m_userObj.animationComp.Play(m_userObj.nameOfAttackAnimation);

			m_animationTimeRemaining = m_userObj.animationComp[m_userObj.nameOfAttackAnimation].length;

			globalEvents.OnEnemyElementsHit( this, hitObj.gameObject );
		}
		
		public void Tick ()
		{
			m_animationTimeRemaining -= Time.deltaTime;
			if (m_animationTimeRemaining > 0.0f)
				return;

			m_userObj.stateMachineDriver.AddAction("go_to_target");
			m_animationTimeRemaining = 1000.0f;
		}
		
		public void OnStateExit (string exitAction, object[] parameters)
		{
		}
		
		#endregion
	}

	class TakeHit : StateImplementationHelper<AttackBehavior>, IStateImplementation
	{
		public TakeHit(AttackBehavior zombie)
			: base(zombie)
		{
		}
		
		#region IStateImplementation implementation

		float m_animationTimeRemaining = 0.0f;
		
		public void OnStateEntry (string entryAction, object[] parameters)
		{
			if (parameters.Length != 2)
			{
				Debug.LogError("Expected 2 parameters of type 'float' and 'Transform'");
				return;
			}
			float damageTaken = (float)parameters[0];
			Transform hitterObj = parameters[1] as Transform;
			if (hitterObj == null)
			{
				// We might get here where the hitter was killed at the same time
				// as the message was sent. In that case we still apply the damage.
			}
			m_userObj.health -= damageTaken;
			//Debug.Log ("Health is: " + m_userObj.health);
			globalEvents.OnPlayerObjectHit(hitterObj, m_userObj.gameObject);
			if (m_userObj.health < 0.0f)
			{
				m_userObj.stateMachineDriver.AddPriorityAction("die");
			}
			else if (!string.IsNullOrEmpty(m_userObj.nameOfHurtAnimation))
			{
				m_userObj.animationComp.Play(m_userObj.nameOfHurtAnimation);
				m_animationTimeRemaining = m_userObj.animationComp[m_userObj.nameOfHurtAnimation].length;

			}
		}
		
		public void Tick ()
		{
			m_animationTimeRemaining -= Time.deltaTime;
			if (m_animationTimeRemaining < 0.0f)
			{
				m_userObj.stateMachineDriver.AddAction("go_to_target");
			}
			m_animationTimeRemaining = 1000.0f;
		}
		
		public void OnStateExit (string exitAction, object[] parameters)
		{
		}
		
		#endregion
	}
	
	// Use this for initialization
	void Awake () {
		stateMachineDriver.AddStateImplementation("striking", new Striking(this));
		stateMachineDriver.AddStateImplementation("hit", new TakeHit(this));

		animationComp = gameObject.GetComponent<Animation>();
	}

	float timeSinceAttack = 0.0f;

	// FixedUpdate is called fixed number of times per sec
	void FixedUpdate () {
	
		//update attack timer
		timeSinceAttack += Time.deltaTime;
		
		// if not yet time to attack, get out
		if (timeSinceAttack < intervalBetweenStrikes)
			return;
		
		// see if we need to reroute to new target if another enemy is nearby
		CheckForNearbyEnemy();
		
		// reset timer to allow for next attack check
		timeSinceAttack = 0;
		
		// sphere cast around the attacking game object.
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, strikeRange);
		
		// get pointer to this as an type-casted object of commonAI
		commonAI thisAttacker = GetComponent<commonAI>();
		
		GameObject hitObj;
		foreach (Collider hit in colliders)
		{
			hitObj = hit.collider.gameObject;
			// did we find an object we are allowed to attack
			if( CanAttack( hitObj.tag ))
			{
				commonAI o = hitObj.GetComponent<commonAI>();

				if (o != null && !o.isDestroying) 
				{

					// if the object is already infected, such as
					// a zombie or werewolf attempting to attack
					// a human infected, don't attack it again...
					if( o.IsInfected )
						continue;

					//Attacking a valid live target
					
					o.playSound( "attack", hitObj.tag.ToString() );

					//Debug.Log ("Striking: " + hitObj.name);
					stateMachineDriver.AddPriorityAction("strike", hitObj.transform);

					//Debug.Log ("valid attack hit: " + hitObj);
					
					// just to track engaged in attack/combat
					o.EngagedInCombat = true;
					
					// stop the game object from moving as we have just engaged attack
					// dont keep walking if it was attacked
					o.stop();
					
					// Now, apply damage to other
					o.takeDamage(damage, thisAttacker);
					
					// if we just killed the target,
					// we need to take ourselves OUT of combat mode
					// (unless something else hits IT again)
					if( o.isDestroying )
					{
					//	thisAttacker.EngagedInCombat = false;

					}
				} 
				else 
				{
					//attacking a house
					safeZoneAI o2 = hitObj.GetComponent<safeZoneAI>();
					if( o2 != null )
					{
						// a safe-zone or finish never is of a commonAI and 
						// never moves at the end of combat.
						stateMachineDriver.AddPriorityAction("strike", o2.transform);
						//animComponent.wrapMode = WrapMode.Once;
						//animComponent.Play(attackAnimation);
						o2.takeDamage( damage );
					}
				}
				return;
			}
		}
	}

	// look for an enemy with three attack distance radius 
	// of where we are moving...
	private void CheckForNearbyEnemy()
	{
		// sphere cast around the attacking game object.
		Vector3 position = gameObject.transform.position;
		Collider[] colliders = Physics.OverlapSphere(position, strikeRange * 3.0f);
		
		GameObject hitObj;
		foreach (Collider hit in colliders)
		{
			hitObj = hit.collider.gameObject;
			
			// did we find an object we are allowed to attack
			if( CanAttack( hitObj.tag ))
			{
				commonAI ai = hitObj.GetComponent<commonAI>();
				if (ai != null && ai.isDestroying)
					continue;

				StateMachineDriver sm = hitObj.GetComponent<StateMachineDriver>();
				if (sm != null && sm.isDead)
					continue;

				// Try to reroute to that location
				//Debug.Log ("Attacking: " + hitObj.name);
				stateMachineDriver.AddAction("new_target_object", hitObj.transform );
				return;
			}
		}
	}

	private bool CanAttack( string tag )
	{
		if (stateMachineDriver.currentState.id == "die") return false;
		foreach( string attackableTag in tagsToAttack)
		{
			if (attackableTag == tag)
				return true;
		}
		return false;
	}
}
