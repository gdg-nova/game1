using UnityEngine;
using System.Collections;

public class humanAI : commonAI, ICanBeScared
{
	// few more publics specific to humans...
	public Material fearMaterial;
	public bool stayInSafe;
	private bool isAfraid = false;
	public float AfraidRadius = 3.0f;

	// Use this for initialization
	public override void Start () 
	{
		// do baseline start actions first...
		base.Start();

		// set Random rotation for visual interest
		// use global static object to utilize smaller memory
		// footprint vs per instance of class
		transform.rotation = gs.getRandomRotation();

		// human navigation targets... look for safe-zones first, then finish zones
		defaultNavTargets.Clear();
		defaultNavTargets.Add(eNavTargets.SafeZone);
		defaultNavTargets.Add(eNavTargets.Finish);

		// get initial target for human...
		moveToNewTarget();
	}

	// Update is called once per frame
	void Update () 
	{
		// if no target, get one now regardless of requiring 
		// "SafeZone" vs "Finish"
		if( currentTarget == null )
			moveToNewTarget();

		// check if user MAY still be afraid or not.
		checkIfStillAfraid();

		// if player is moving, but not walking and they are not in "afraid" mode,
		// then set their animation to a simple walk mode
		if (	navAgent.velocity.magnitude > 0 
			&& ! CurrentAnimation().Equals ( "walk" ) 
			&& ! isAfraid ) 
		{
			navAgent.speed = baseSpeed;
			PlayAnimation("walk");
		}
	
		// if time passed the interval check, 
		// what do we need to do now?
		if( reachedTarget())
		{
			// if the human is afraid and already running to a safe-zone
			// destination, enter them into said safe zone since they are
			// within the range of the stop distance vs remaining distance
			if (isAfraid && currentTarget.tag == eNavTargets.SafeZone.ToString())
			{
				// Tell the target to add a new human represented by THIS
				// one just about to enter, then KILL this instance from
				// the visual screen
				currentTarget.SendMessage("addHuman");
				die();
			}
			else
				moveToNewTarget();
		}
		else 
			// in addition, check for being stagnant (clarification in commonAI.cs)
			IsMovementStagnant();
	}

	// Sprint to "safe" location (0,0,0 is test)
	public void Afraid() 
	{
		// if being destroyed, get out, dont do anything else
		if (isDestroying)
			return;

		// set flag that human is afraid, and then set the halo effect
		// which is a coloring under them when running to give visual effect
		// of movement state...
		isAfraid = true;
		// found this since depricated component.active.  We must call the BEHAVIOR of the component.
		// http://forum.unity3d.com/threads/component-active-is-obsolete-but-no-enabled-property.30105/
		((Behaviour)GetComponent("Halo")).enabled = true;

		moveToNewTarget();

		// RUN when afraid after new target destination established
		// the HumanHero with child object LittleHero_Solo has
		// animations of walk, idle, sprint
		if( navAgent != null )
		{
			navAgent.speed = runSpeed;
			PlayAnimation("sprint");
		}
	}

	// when a human is running away afraid, see after a certain amount of time
	// if there are any zombies STILL within a given proximity, if not, walk.
	private float lastAfraidCheck = 0.0f;

	private void checkIfStillAfraid()
	{	
		// if NOT afraid, nothing to do, get out.
		if( ! isAfraid )
			return;

		lastAfraidCheck += Time.deltaTime;
		// if not 3 seconds since last check, don't bother and over cycle objects checking
		if( lastAfraidCheck < 3.0f )
			return;

		// we are within the time interval, reset counter for next time around
		// and then see IF there are any zombies left in the zone
		lastAfraidCheck = 0.0f;

		// if NO more zombies within 10 radius range, turn off the afraid flag
		if( ! gs.anyTagsInRange( transform.position, AfraidRadius, eNavTargets.Zombie ))
		{
			isAfraid = false;
			// turn OFF the halo effect when not afraid anymore.
			((Behaviour)GetComponent("Halo")).enabled = false;

			// back to walking mode
			navAgent.speed = baseSpeed;
			animComponent.Play("walk");
		}
	}
}
