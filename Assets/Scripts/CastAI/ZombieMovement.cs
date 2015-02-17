using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieMovement : MonoBehaviour {

	public StateMachineDriver stateMachineDriver;

	public float navCheckInterval = 3.0f;
	public float navStopDistance = 5.0f;

	private Animation animationComp;
	private NavMeshAgent navAgentComp;

	private List<eNavTargets> m_defaultNavTargets = new List<eNavTargets>();

	private Vector3 m_currentTargetPosition;
	private Transform m_currentTargetObject = null;
	private bool m_isTargetingObject;

	// This in an entry state, it will play an animation of the zombie
	// lying down and getting up (reverse Die animation). After that the
	// zombie an message will be sent to attack or roam in a random
	// direction.
	class StartLyingDown : StateImplementationHelper<ZombieMovement>, IStateImplementation
	{
		public StartLyingDown(ZombieMovement zombie)
			: base(zombie)
		{
		}

		#region IStateImplementation implementation

		public void OnStateEntry (string entryAction, object[] parameters)
		{
			// Play the die animation in reverse
			float animSpeed = -.5f;
			m_userObj.animationComp ["die"].speed = animSpeed;
			float animTime = Mathf.Abs ( m_userObj.animationComp ["die"].length * (1 / animSpeed));
			
			m_userObj.animationComp ["die"].time = m_userObj.animationComp ["die"].length;
			m_userObj.animationComp.Play ("die");

			m_userObj.stateMachineDriver.AddDelayedAction("got_up", animTime);
		}

		public void Tick ()
		{
		}

		public void OnStateExit (string exitAction, object[] parameters)
		{
			m_userObj.animationComp["die"].speed = 1.0f;
			m_userObj.animationComp["die"].time = 0.0f;
			m_userObj.animationComp.Stop ();
		}

		#endregion
	}

	class ChooseRandomTarget : StateImplementationHelper<ZombieMovement>, IStateImplementation
	{
		public ChooseRandomTarget(ZombieMovement zombie)
			: base(zombie)
		{
		}

		#region IStateImplementation implementation

		public void OnStateEntry (string entryAction, object[] parameters)
		{
			GameObject go = gs.getRandomNavTarget(m_userObj.m_defaultNavTargets);
			if (go == null)
			{
				Debug.Log(m_userObj.name + " couldn't find any where to go.");
			}
			else
			{
				m_userObj.stateMachineDriver.AddAction("new_target", go.transform.position);
			}
		}

		public void Tick ()
		{
		}

		public void OnStateExit (string exitAction, object[] parameters)
		{
		}

		#endregion
	}

	class MoveToTarget : StateImplementationHelper<ZombieMovement>, IStateImplementation
	{
		public MoveToTarget(ZombieMovement zombie)
			: base(zombie)
		{
		}
		
		#region IStateImplementation implementation
		
		public void OnStateEntry (string entryAction, object[] parameters)
		{
			if (parameters.Length != 1)
			{
				Debug.LogError ("Invalid number of parameters, expect 1 but got " + parameters.Length);
				return;
			}
			if (entryAction == "new_target")
			{
				Vector3 targetPosition = (Vector3)parameters[0];
				if (targetPosition == null)
				{
					Debug.LogError ("Expecting parameter of type 'Vector3'");
					return;
				}
				m_userObj.m_currentTargetPosition = targetPosition;
				m_userObj.m_currentTargetObject = null;
				m_userObj.m_isTargetingObject = false;
			} else if (entryAction == "new_target_object")
			{
				Transform targetObject = parameters[0] as Transform;
				if (targetObject == null)
				{
					// If we get this, probably the object was destroyed while someone
					// was attacking it.
					m_userObj.stateMachineDriver.AddAction("reached_destination");
					return;
				}
				m_userObj.m_currentTargetPosition = targetObject.position;
				m_userObj.m_currentTargetObject = targetObject;
				m_userObj.m_isTargetingObject = true;
			}
			// Make our way to the destination
			m_userObj.navAgentComp.SetDestination(m_userObj.m_currentTargetPosition);
			m_userObj.m_currentAnimation = null;
			m_timeSinceCheck = 0.0f;
		}
		private float m_timeSinceCheck;
		
		public void Tick ()
		{
			m_userObj.UpdateAnimation();

			m_timeSinceCheck += Time.deltaTime;
			if (m_timeSinceCheck < m_userObj.navCheckInterval)
				return;

			m_timeSinceCheck = 0.0f;

			// Determine if we've reached our destination
			if (m_userObj.m_currentTargetPosition != null)
			{
				if ((m_userObj.m_currentTargetPosition - m_userObj.transform.position).magnitude < m_userObj.navStopDistance)
				{
					m_userObj.navAgentComp.Stop ();
					m_userObj.navAgentComp.ResetPath();
					m_userObj.stateMachineDriver.AddAction( "reached_target" );
				}
			}
			if (m_userObj.m_isTargetingObject && m_userObj.m_currentTargetObject)
			{
				// Object was destroyed and we didn't reach there yet. Move on.
				m_userObj.navAgentComp.Stop ();
				m_userObj.navAgentComp.ResetPath ();
				m_userObj.stateMachineDriver.AddAction( "reached_target" );
			}
		}

		public void OnStateExit (string exitAction, object[] parameters)
		{
			m_userObj.m_currentAnimation = null;
			if (exitAction == "strike")
			{
				m_userObj.navAgentComp.Stop ();
				m_userObj.navAgentComp.ResetPath ();
			}
		}
		
		#endregion
	}

	class SetInfluencedTarget : StateImplementationHelper<ZombieMovement>, IStateImplementation
	{
		public SetInfluencedTarget(ZombieMovement zombie)
			: base(zombie)
		{
		}
		
		#region IStateImplementation implementation
		
		public void OnStateEntry (string entryAction, object[] parameters)
		{
			if (parameters.Length != 1)
			{
				Debug.LogError ("Invalid number of parameters, expect 1 but got " + parameters.Length);
				return;
			}
			Vector3 targetDirection = (Vector3)parameters[0];
			//Debug.Log("Direction of influence: " + targetDirection);
			// Use the direction to determine a target in the specified direction.
			m_userObj.navAgentComp.Stop ();
			m_userObj.navAgentComp.ResetPath();

			// Project in the direction of influence and check if we hit something
			Vector3 projectedPosition = m_userObj.transform.position + targetDirection*30.0f;
			NavMeshHit hit;
			if (NavMesh.Raycast(m_userObj.transform.position, projectedPosition, out hit, -1))
			{
				// Choose a closer position
				m_userObj.stateMachineDriver.AddAction("new_target", hit.position);
			}
			else
			{
				m_userObj.stateMachineDriver.AddAction("new_target", projectedPosition);
			}
		}
		
		public void Tick ()
		{
		}
		
		public void OnStateExit (string exitAction, object[] parameters)
		{
		}
		
		#endregion
	}

	class DieState : StateImplementationHelper<ZombieMovement>, IStateImplementation
	{
		public DieState(ZombieMovement zombie)
			: base(zombie)
		{
		}
		
		#region IStateImplementation implementation
		
		public void OnStateEntry (string entryAction, object[] parameters)
		{
			// Play the die animation in reverse
			float animTime = Mathf.Abs ( m_userObj.animationComp ["die"].length );
			
			m_userObj.animationComp ["die"].time = 0.0f;
			m_userObj.animationComp.Play ("die");

			GameObject.Destroy ( m_userObj.gameObject, animTime );
		}
		
		public void Tick ()
		{
		}
		
		public void OnStateExit (string exitAction, object[] parameters)
		{
		}
		
		#endregion
	}
	
	string m_currentAnimation = null;
	void UpdateAnimation()
	{
		string animation = "idle_lookaround";
		if (navAgentComp.hasPath) {
			if (navAgentComp.speed > 0 && navAgentComp.speed <= 6) animation = "walk";
			else if (navAgentComp.speed > 6 && navAgentComp.speed <= 12) animation = "run";
			else if (navAgentComp.speed > 12) animation = "sprint";
		}
		// TODO: Why doesn't this work?
		if (true)// (m_currentAnimation != animation)
		{
			if (animationComp.IsPlaying(animation))
				return;
			AnimationState animState = animationComp[animation];
			if (animState == null)
			{
				Debug.LogError("Invalid animation state specified: " + animation);
				return;
			}
			animState.speed = 1.0f;
			animState.time = 0.0f;
			//Debug.Log ("playing " + animation + " for obj: " + this.GetHashCode());
			animationComp.Stop ();
			animationComp.Play( animation /*, PlayMode.StopAll*/ );
			m_currentAnimation = animation;
		}
	}
	
	void MakeAnimationLooped(string animationName)
	{
		AnimationClip ac = animationComp.GetClip(animationName);
		
		if (ac != null)
			ac.wrapMode = WrapMode.Loop;
	}

	void MakeAnimationSingle(string animationName)
	{
		AnimationClip ac = animationComp.GetClip(animationName);
		
		if (ac != null)
			ac.wrapMode = WrapMode.Once;
	}

	void Awake()
	{
		// Register component classes with the driver
		stateMachineDriver.AddStateImplementation("start_lying_down", new StartLyingDown(this));
		stateMachineDriver.AddStateImplementation("choose_random_target", new ChooseRandomTarget(this));
		stateMachineDriver.AddStateImplementation("move_to_target", new MoveToTarget(this));
		stateMachineDriver.AddStateImplementation("set_influenced_target", new SetInfluencedTarget(this));
		stateMachineDriver.AddStateImplementation("die", new DieState(this));

		m_defaultNavTargets.Add (eNavTargets.SafeZone);
		m_defaultNavTargets.Add (eNavTargets.Finish);
		// Playable is from the original, does it still work?
		m_defaultNavTargets.Add (eNavTargets.Playable);

		animationComp = gameObject.GetComponent<Animation>();
		navAgentComp = gameObject.GetComponent<NavMeshAgent>();

		MakeAnimationLooped ("idle");
		MakeAnimationLooped ("walk");
		MakeAnimationLooped ("sprint");
		MakeAnimationLooped ("run");
		MakeAnimationLooped ("idle_settle");
		MakeAnimationLooped ("idle_lookaround");

		MakeAnimationSingle ("L2R_swipe");
		MakeAnimationSingle ("die");
		MakeAnimationSingle ("hurt");

	}
}
