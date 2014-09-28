using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class commonAI : MonoBehaviour 
{
	// each game object (human, zombie, guard, etc) will have some
	// speed movement basis.  These will be configurable from within
	// the Unity Properties window, so make these public
	public float speedVariation = .2f;
	public float baseSpeed = 1.0f;
	public float runSpeed = 2.5f;

	// different objects have different health levels.
	// a human has 1 as default... so once attacked, such as from
	// a zombie, it dies immediately.  Guards and Safe Zones have 
	// larger tollerances before being destroyed
	public float health = 1;

	// for detection of a stopping distance from a given target?
	// how frequent to check navigation adjustments
	public float navCheckInterval = 3.0f;
	// stop moving when we are within given distance of destination
	public float navStopDistance = 5.0f;

	// internally, to keep track of when last checked nav to destination
	private float timeSinceNavCheck = 0.0f;

	// Each INSTANCE of whatever object needs its own navigation agent
	// to help control movement. (humans, guards, zombies)
	protected NavMeshAgent navAgent;

	// Each object, regardless of human, guard, zombie, whatever 
	// needs a target to try and engage
	protected GameObject currentTarget;

	// each object may also have its own animation
	// Animations for 
	// Humans (walk, idle, sprint)
	// Guards (walk, idle, idle_lookaround, hurt, die, R2L_swipe)
	// Zombies (walk, idle_lookaround, hurt, die, L2R_swipe)
	protected Animation animComponent;

	// not all controls HAVE animators..
	// have boolean to handle these to not throw errors.
	private bool hasAnimator;

	// Each object will by default need SOME nav taget to move towards.
	protected List<eNavTargets> defaultNavTargets;

	public bool isDestroying
	{ get; protected set; }


	// by default, allow moving after combat...
	// ie: move to next destination.  However, if it is a stationary
	// guard, don't move... retain your position.  Default ok to move
	public bool moveAfterCombat = true;

	private bool engagedInCombat;
	public bool EngagedInCombat
	{ 
		get { return engagedInCombat; }
		set {	// during the setter, if we WERE engaged in a combat cycle,
			 	// and now coming off of it, force ourselves to look for new target.
				if( engagedInCombat && !value )
					moveToNewTarget();

				engagedInCombat = value;
			}
	}


	// There may be times when a moving object is trying to go to
	// a destination, but may be in a dead-lock bumping into something
	// such as another object moving in same direction and the navAgent
	// doesn't know of the conflict.  So, set a timerSince value and
	// preserve destination.  When timer hits and game player is STILL
	// within some finite distance, force the entity to find another target.
	private float timeSinceStagnant;
	private Vector3 lastStagnant;

	// Use this for initialization
	public virtual void Start () 
	{
		// always start NOT engaged in combat.
		EngagedInCombat = false;

		// Ensure nav targets list object is instantiated
		// and available for all subclassed from it.
		// always default the list to "Finish"
		defaultNavTargets = new List<eNavTargets>();
		defaultNavTargets.Add(eNavTargets.Finish);

		// load NavAgent for each 
		navAgent = (NavMeshAgent)GetComponent("NavMeshAgent");

		// pre-load animation component that is used on most elements
		animComponent = (Animation)GetComponent("Animation");

		// flag... do we ACTUALLY have an animator object?
		hasAnimator = animComponent != null ;

		// Initiate first target and set destination to it
		moveToNewTarget();
	}

	// in case something is getting attacked and we need to STOP them from
	// walk/run animation movement towards a given direction.
	public void stop() 
	{
		// such as a double-attack on a single target and each call stop,
		// don't re-stop an already stopped dying object.
		if( isDestroying )
			return;

		// should never be, but just in case from human-zombie conversion
		if( navAgent != null )
			navAgent.Stop();
	}
	
	// no "Update" for the base level... each instance will have 
	// it's OWN "to do" for the update method
	protected bool reachedTarget()
	{
		// if no taget, alway false
		if( currentTarget == null )
			return false;

		//update attack timer
		timeSinceNavCheck += Time.deltaTime;
			
		// if not yet time to check, get out
		if (timeSinceNavCheck < navCheckInterval)
			return false;

		// reset timer to allow for next attack check
		timeSinceNavCheck = 0;

		// are we within distance of target
		return ( navAgent.remainingDistance < navStopDistance );
	}

	protected void IsMovementStagnant()
	{
		// no target, nothing to do, not stagnant...
		// disable for now by adding OR TRUE to the end
		// without actual commenting out entire block
		if( currentTarget == null || true)
			return;

		timeSinceStagnant += Time.deltaTime;
		// fixed limit for now... look every 2.5 seconds of game time...
		if (timeSinceStagnant < 2.5f)
			return;

		// what was distance from when we started
		float distance = Vector3.Distance(lastStagnant, currentTarget.transform.position);

		// reset time and position before next compare
		timeSinceStagnant = 0.0f;
		lastStagnant = currentTarget.transform.position;

		// not sure of distance measurement with respect to world coordinates
		// but if not moving, force to a new target.
		if( distance < 1.0f )
			moveToNewTarget();
	}


	// Whatever "instance" of this object is, it is referred to as
	// gameObject... destroy itself.
	protected void die()
	{
		// if already attempting to be destroyed, just get out
		if( isDestroying )
			return;

		isDestroying = true;

		navAgent.Stop();
		if( hasAnimator )
		{
			// if this is NOT a human (ie: Guard, Zombie)
			// then we CAN play the die animation as both of them
			// have it
			if( gameObject.tag != "Human" )
			{
				animComponent.wrapMode = WrapMode.Once;
				animComponent.Play("die");
				PauseGame( animation["die"].length * 2 );
			}
		}

		Destroy (gameObject); 
	}

	protected IEnumerator PauseGame(float duration)
	{
		yield return new WaitForSeconds( duration );
	}

	public string CurrentAnimation()
	{
		// if no animator, empty string...
		if( ! hasAnimator )
			return "";

		// if it DOES have an animator, see which is ACTUALLY running
		// in case mode stops outside the PlayAnimation call
		string curStates = "";
		foreach( AnimationState aState in animComponent )
		{
			if( animComponent.IsPlaying( aState.name ))
				curStates += aState.name + ",";
		}

		return curStates;
	}

	protected void PlayAnimation(string animationName)
	{
		if( ! hasAnimator )
			return;

		foreach( AnimationState aState in animComponent )
		{
			if( aState.name == animationName )
			{
				animComponent.wrapMode = WrapMode.Loop;
				animComponent.Play(animationName);
				break;
			}
		}
	}

	protected void moveToNewTarget()
	{
		// based on the list of navTargets an entity has, 
		currentTarget = gs.getRandomNavTarget(defaultNavTargets);
		moveToSpecificGameObj( currentTarget );
	}
	
	public void moveToSpecificGameObj( GameObject NewTarget )
	{
		if( NewTarget == null )
			return;
		Debug.Log ("moving to target" + NewTarget);
		// preserve where we are with the new position
		timeSinceStagnant = 0.0f;
		lastStagnant = NewTarget.transform.position;
		
		if( speedVariation == 0.0f )
			speedVariation = 1.0f;
		
		// allow speed randomly ranged between 40% and its full base speed
		navAgent.speed = Random.Range(baseSpeed * .4f, baseSpeed);
		
		//Go to new target
		navAgent.SetDestination(NewTarget.transform.position);
	}

	public void takeDamage(float damageTaken)
	{
		health -= damageTaken;
		if (health < 0.0f)
			die();
		else
		{
			// Only partially hit... animate a "hurt" stage
			if( hasAnimator )
			{
				animComponent.wrapMode = WrapMode.Once;
				animComponent.Play("hurt");
			}
		}
	}
}

