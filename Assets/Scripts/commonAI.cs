using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class commonAI : MonoBehaviour 
{
	// each game object (human, zombie, guard, etc) will have some
	// speed movement basis.  These will be configurable from within
	// the Unity Properties window, so make these public
	// randomized speed variation as percentage of max speeds avail
	public float baseSpeed = 1.0f;
	public float baseRandomPct = 50.0f;

	public float runSpeed = 2.5f;
	public float runRandomPct = 85.0f;

	public float manaCost = 1f;

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
	protected Transform currentTarget;

	// each object may also have its own animation
	// Animations for 
	// Humans (walk, idle, sprint)
	// Guards (walk, idle, idle_lookaround, hurt, die, R2L_swipe)
	// Zombies (walk, idle_lookaround, hurt, die, L2R_swipe)
	protected Animation animComponent;
	//protected Animation currentActivityAnim;

	// special flags for hurt and die animations
	protected bool hasHurtAnimation = false;
	protected bool hasDieAnimation = false;

	// only applicable now to humans, but if afraid and reaching 
	// a non safe-zone destination, we need to leave the person
	// in a running scared mode
	protected bool isAfraid = false;

	// Each object will by default need SOME nav taget to move towards.
	protected List<eNavTargets> defaultNavTargets;
	
	// create instance of the ATTACK object which will have
	// all the attack components...
	public attackAI Attack;
	

	public bool isDestroying = false;
	//{ get; protected set; }
	
	// by default, allow moving after combat...
	// ie: move to next destination.  However, if it is a stationary
	// guard, don't move... retain your position.  Default ok to move
	public bool moveAfterCombat = true;

	private ArrayList loopedAnimations = new ArrayList();

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

	private string ParentObjectName;

	private float origBaseSpeed;
	private float origRunSpeed;

	// Use this for initialization
	public virtual void Start () 
	{
		loopedAnimations.Add ("idle");
		loopedAnimations.Add ("walk");

		loopedAnimations.Add ("sprint");

		loopedAnimations.Add ("run");

		loopedAnimations.Add ("idle_settle");
		loopedAnimations.Add ("idle_lookaround");



		GameObject gobj = gameObject;
		ParentObjectName = gobj.name;
		try
		{
			while( gobj.transform.parent.gameObject != null )
			{
				ParentObjectName = gobj.name;
				gobj = gobj.transform.parent.gameObject;
			}
		}
		catch{}



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
		origBaseSpeed = baseSpeed;
		origRunSpeed = runSpeed;
		//AdjustSpeeds();

		// animation modes are independent per specific clip.
		// walk is ALWAYS a looping

		foreach (string loopAnim in loopedAnimations) 
		{
			AnimationClip ac = animComponent.GetClip(loopAnim);

			if (ac != null)
				ac.wrapMode = WrapMode.Loop;
		}

		//AnimationClip ac = animComponent.GetClip ("walk");
		//ac.wrapMode = WrapMode.Loop;
		animComponent.Play ();

		// Initiate first target and set destination to it
		moveToNewTarget();
	}
	
	private void AdjustSpeeds()
	{
		baseSpeed = Random.Range(origBaseSpeed * baseRandomPct / 100.0f, origBaseSpeed); 	// sample, 60% to 100% speed
		runSpeed = Random.Range(origRunSpeed * runRandomPct / 100.0f, origRunSpeed);	// sample, 85% to 100% run speed 
		
		if( baseSpeed < .5f)
			baseSpeed = .5f;
		
		if( runSpeed < 1.75f )
			runSpeed = 1.75f;
	}

	protected void checkAnimation() {
		//if (currentTarget == null) {

		//Debug.Log (gameObject.tag + "  " +   animComponent.clip.ToString ());

		string animStr = getAnimNameforCurrentState ();

		ArrayList playingAnims = CurrentAnimationList ();

		//string playingAnim = animComponent.clip.ToString ();


		if (!playingAnims.Contains (animStr)) {
			bool changeAnimation = false;

			foreach (string a in playingAnims) {
				if (loopedAnimations.Contains(a)) {
                  animComponent.Stop(a);
					changeAnimation = true;
				}
				if (changeAnimation) animComponent.Play (animStr);

			}

		}

		if (playingAnims.Count == 0 && !isDestroying && animComponent != null)
			animComponent.Play (animStr);
		//Debug.Log ("playing anim: " + animStr);

		//if (playingAnims.ToUpper().Contains(animStr.ToUpper)
//		if (animStr != playingAnim && loopedAnimations.Contains (playingAnim)) {
//			Debug.Log ("playing string: " + playingAnim + ", switching to anim: " + animStr);
//
//			animComponent.clip = animComponent.GetClip(animStr);
//			animComponent.Play (animStr);
//
//
//		}
//


		//animComponent.clip = animComponent.GetClip(animStr);
		//Debug.Log ("set anim to " + getAnimNameforCurrentState ());
		//Debug.Log ("set animation clip: " + animComponent.clip.ToString());

		//	animComponent.Play();

		//}
	}

	public virtual string getAnimNameforCurrentState() 
	{
		if( navAgent == null)
			return "idle";

		if (!navAgent.hasPath) {
				return "idle";
		} else {
			if (navAgent.speed > 0 && navAgent.speed <= 6) return "walk";
					else if (navAgent.speed > 6 && navAgent.speed <= 12) return "run";
					else if (navAgent.speed > 12) return "sprint";
		}

		return "idle";
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

		// if no taget, alway false, let the character move to another target
		//if( navAgent.destination == null )
		//	return false;

//		if( navAgent.remainingDistance == Mathf.Infinity)
//		{
//			moveToNewTarget();
//			return false;
//		}

		//update attack timer
		timeSinceNavCheck += Time.deltaTime;
			
		// if not yet time to check, get out
		if (timeSinceNavCheck < navCheckInterval)
			return false;

		// reset timer to allow for next attack check
		timeSinceNavCheck = 0;

		// For SOME reason, the navAgent.remainingDistance is false on x/z basis.. may be ok with x/y though.
		// since our game has x/z, we need to compute the x and z differential, then compute basis from that
		//float deltaX = Mathf.Abs( navAgent.destination.x - gameObject.transform.position.x );
		//float deltaZ = Mathf.Abs( navAgent.destination.z - gameObject.transform.position.z );
		if( lastPosition == null )
			lastPosition = transform.position;
		else if( lastPosition == transform.position )
		{
			if( this.gameObject is werewolfAi )
				Debug.Log ( "exact same position:" );
		}



		float remainingDistance = Vector3.Distance (transform.position, navAgent.destination);

		//if (gameObject.tag =="Zombie")  Debug.Log (gameObject.tag + "  distance to target: " + remainingDistance);
		//return ( deltaX < navStopDistance  && deltaZ < navStopDistance );

		return (remainingDistance <= navStopDistance);
	}
	private Vector3 lastPosition;



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
		Debug.Log ( "LastDistance: " + lastStagnantDistance + "   Distance Now: " + distance
		           + "   Net Distance: " + Mathf.Abs( lastStagnantDistance - distance ) );

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
		{
			currentTarget = null;
			moveToNewTarget();
		}

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

		if( navAgent != null )
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
			{
				// Detect if the human was running or not... if running, then
				// make a FAST zombie, not a slow one...
				humanAI hAI = gameObject.GetComponent<humanAI>();
				// ONLY create a zombie if it was not killed by a werewolf.
				if( ! hAI.attackedByWerewolf )
				{
					if( hAI.isAfraid )
						Invoke ("requestZombieCreationFast", animation["walk"].length * 2 );
					else
						Invoke ("requestZombieCreation", animation["walk"].length * 2 );
				}
			
			}

			if( animComponent["die"] != null )
				Destroy(gameObject,animation["die"].length * 2 );

			// original human has no die animation, kill object right-away
			if( animComponent["die"] == null )
				Destroy (gameObject);
		}
	}
	
	protected void requestZombieCreation()
	{
		GameObject go = GameObject.FindWithTag ("GameController");
		gameControl gc = go.GetComponent<gameControl> ();
		gc.createZombie(gameObject.transform.position, gameObject.transform.rotation);
		gc.manaPool += 1;
	}

	protected void requestZombieCreationFast() 
	{
		GameObject go = GameObject.FindWithTag ("GameController");
		gameControl gc = go.GetComponent<gameControl> ();
		zombieAI zAI = gc.createZombie(gameObject.transform.position, gameObject.transform.rotation);
		zAI.MakeFastZombie();
		gc.manaPool += 1;
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

	public ArrayList CurrentAnimationList() 
	{
		ArrayList results = new ArrayList ();
		if( isDestroying || animComponent == null )
			return results;
			
		foreach( AnimationState aState in animComponent )
		{
			if( animComponent.IsPlaying( aState.name ))
				results.Add(aState.name);
		}
		return results;
	}

	protected void moveToNewTarget()
	{
		if (gameObject.tag == "Zombie")
			return;

		// based on the list of navTargets an entity has, 
		GameObject go = gs.getRandomNavTarget(defaultNavTargets);
		if (go != null)
		{
			currentTarget = go.transform;
			moveToSpecificTransform( currentTarget );
		}
	}
	
	public void moveToSpecificTransform( Transform newTarget )
	{
		moveToSpecificTV( newTarget );
	}

	public void moveToSpecificVector( Vector3 newTarget )
	{
		moveToSpecificTV( newTarget );
	}

	private void moveToSpecificTV( object newEndpoint )
	{
		if( isDestroying )
			return;

		Vector3 targetVector ;
		if( newEndpoint is Vector3 )
			targetVector = (Vector3)newEndpoint;

		if( newEndpoint is Transform )
		{
			currentTarget = (Transform)newEndpoint;
			targetVector = currentTarget.position;
		}
		else
		{
			currentTarget = null;
			// if null, force SOMETHING random +/- the current
			// x and z coordinates of character
			float x = Random.Range(gameObject.transform.position.x - 5.0f,
			                       gameObject.transform.position.x + 5.0f );
			float z = Random.Range(gameObject.transform.position.z - 5.0f,
			                       gameObject.transform.position.z + 5.0f );
			targetVector = new Vector3(x, gameObject.transform.position.y, z);
		}
		

		// preserve where we are with the new position
		timeSinceStagnant = 0.0f;
		lastStagnantVector = targetVector;
		lastStagnantDistance = 100f;

		// always start animation walking to target...
		// if human and in "Afraid" mode, it will change the the sprint animation
		if( navAgent != null )
			navAgent.SetDestination(targetVector);
	}

	public bool IsInfected { get; protected set; }
	protected bool attackedByWerewolf;

	public void takeDamage(float damageTaken, commonAI attackedBy)
	{
		if( isDestroying )
			return;

		// if already infected, just get out.
		if( IsInfected )
			return;

		if( attackedBy is werewolfAi )
		{
			attackedByWerewolf = true;

			// Allow only a 20% chance of being infected vs outright kill.
			if( Random.Range(0,100) > 80 )
			{
				IsInfected = true;
				return;
			}
		}

		health -= damageTaken;
		if (health < 0.0f)
			die();
		else if( hasHurtAnimation )
			animComponent.Play("hurt");
	}
	
	abstract public void playSound(string action, string target);
}

