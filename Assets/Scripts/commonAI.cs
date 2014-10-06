using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class commonAI : MonoBehaviour 
{
	// each game object (human, zombie, guard, etc) will have some
	// speed movement basis.  These will be configurable from within
	// the Unity Properties window, so make these public
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

	// special flags for hurt and die animations
	protected bool hasHurtAnimation = false;
	protected bool hasDieAnimation = false;

	// only applicable now to humans, but if afraid and reaching 
	// a non safe-zone destination, we need to leave the person
	// in a running scared mode
	protected bool isAfraid = false;

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
				if( moveAfterCombat && engagedInCombat && !value )
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
	private float lastStagnantDistance = 100f;
	private Vector3 lastStagnantVector;


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

		// First time in for any animated element, don't allow the to have
		// the same exact speed.  100 humans can all have slightly different
		// speeds within their base range.  Don't change every time they are
		// moving between objects.  Why sometimes slow, then fast.  The speed
		// is the speed for the duration of the human's life (same with running)
		baseSpeed = Random.Range(baseSpeed * .6f, baseSpeed); 	// sample, 60% to 100% speed
		runSpeed = Random.Range(runSpeed * .85f, runSpeed);	// sample, 85% to 100% run speed 

		if( baseSpeed < .5f)
			baseSpeed = .5f;

		if( runSpeed < 1.75f )
			runSpeed = 1.75f;


		// animation modes are independent per specific clip.
		// walk is ALWAYS a looping
		AnimationClip ac = animComponent.GetClip ("walk");
		ac.wrapMode = WrapMode.Loop;

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
		if( isDestroying )
			return false;

		float remDist = Mathf.Abs( navAgent.remainingDistance );

		if( navAgent.remainingDistance == Mathf.Infinity)
		{
			moveToNewTarget();
			return false;
		}

		// if no taget, alway false, let the character move to another target
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
		return ( remDist < navStopDistance );
	}

	protected void IsMovementStagnant()
	{
		// no target, nothing to do, not stagnant...
		// disable for now by adding OR TRUE to the end
		// without actual commenting out entire block
		if( currentTarget == null || true)
			return;

		timeSinceStagnant += Time.deltaTime;
		// fixed limit for now... look every 2.0 seconds of game time...
		if (timeSinceStagnant < 4.0f)
			return;

		// what was distance from when we started
		// distance will always be a positive number
		float distance = Vector3.Distance(lastStagnantVector, currentTarget.transform.position);
//		Debug.Log ( "LastDistance: " + lastStagnantDistance + "   Distance Now: " + distance
//		           + "   Net Distance: " + Mathf.Abs( lastStagnantDistance - distance ) );

		// reset time and position before next compare
		// time, randomize so not all things are timing out for
		// a new same movement
		timeSinceStagnant = Random.Range( 0.0f, .7f );
		lastStagnantVector = currentTarget.transform.position;

		// Known combinations that stagnant movement.
		float netDist = Mathf.Abs( lastStagnantDistance - distance );
		
		// not sure of distance measurement with respect to world coordinates
		// but if not moving, force to a new target.
		if( distance < .05
		   || netDist < .3f 
		   || ( lastStagnantDistance == 0.0f ) && distance - netDist < .05)
			moveToNewTarget();

		lastStagnantDistance = distance;
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
		if( animComponent != null )
		{
			// if this is NOT a human (ie: Guard, Zombie) then we CAN play
			// the die animation as both of them have it
			if( animComponent["die"] != null )
			{
				animComponent.wrapMode = WrapMode.Once;
				animComponent.Play("die");
			}

			if (gameObject.tag == "Human")
				Invoke ("requestZombieCreation", animation["walk"].length * 2 );

			if( animComponent["die"] != null )
				Destroy(gameObject,animation["die"].length * 2 );
		}

		// original human has no die animation, kill object right-away
		if( animComponent["die"] == null )
			Destroy (gameObject);

	}
	
	protected void requestZombieCreation() 
	{
		gameControl gc = Camera.main.GetComponent<gameControl> ();
		gc.createZombie(gameObject.transform.position, gameObject.transform.rotation);
	}

	protected IEnumerator PauseGame(float duration)
	{
		yield return new WaitForSeconds( duration );
	}

	public string CurrentAnimation()
	{
		// if no animator, empty string...
		if( animComponent == null )
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
	
	protected void moveToNewTarget()
	{
		// based on the list of navTargets an entity has, 
		currentTarget = gs.getRandomNavTarget(defaultNavTargets);
		moveToSpecificGameObj( currentTarget );
	}
	
	public void moveToSpecificGameObj( GameObject NewTarget )
	{
		if( isDestroying || NewTarget == null )
			return;

		// preserve where we are with the new position
		timeSinceStagnant = 0.0f;
		lastStagnantVector = NewTarget.transform.position;
		lastStagnantDistance = 100f;

		// always start animation walking to target...
		// if human and in "Afraid" mode, it will change the the sprint animation
		if( navAgent != null )
		{
			if( !isAfraid )
			{
				animComponent.Play("walk");

				//Go to new target
				// see notation during start to compute one-time randomly adjusted
				// base speed for duration of the object instance.
				navAgent.speed = baseSpeed;
			}
			navAgent.SetDestination(NewTarget.transform.position);
		}
	}

	public void takeDamage(float damageTaken)
	{
		if( isDestroying )
			return;

		health -= damageTaken;
		if (health < 0.0f)
			die();

		else if( hasHurtAnimation )
			animComponent.Play("hurt");
	}
}

