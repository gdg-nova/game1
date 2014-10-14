using UnityEngine;
using System.Collections;

public class humanAI : commonAI, ICanBeScared
{
	// few more publics specific to humans...
	public float AfraidRadius = 2.5f;

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
		// if all else fails, find SOME place in the playable area of the board
		defaultNavTargets.Add(eNavTargets.Playable);

		// humans also have SPRINT mode animation which is always loop mode
		AnimationClip ac = animComponent.GetClip ("sprint");
		ac.wrapMode = WrapMode.Loop;

		// ensure we have "die" animation or not
		hasDieAnimation = animComponent["die"] != null;

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

		// if no target due to object being destroyed (such as safe-zone house), 
		// get out and try again next cycle
		if( currentTarget == null )
			return;

		// check if human MAY still be afraid or not.
		checkIfStillAfraid();

		// if player is moving, but not walking and they are not in "afraid" mode,
		// then set their animation to a simple walk mode
		if (	navAgent.velocity.magnitude > 0 
			&& ! CurrentAnimation().Equals ( "walk" ) 
			&& ! isAfraid 
		    && ! isDestroying) 
		{
			navAgent.speed = baseSpeed;
			animComponent.Play ("walk");
		}
	
		// if time passed the interval check, 
		// what do we need to do now?
		if( reachedTarget())
		{
			// in case a game object gets destroyed mid-stream, 
			// preserve just the TAG element as a string so we can still work with it here
			bool atSafeZone = ( currentTarget.tag == eNavTargets.SafeZone.ToString());

			// if the human is afraid and already running to a safe-zone
			// destination, enter them into said safe zone since they are
			// within the range of the stop distance vs remaining distance
			 if (isAfraid && atSafeZone && currentTarget != null)
			// humans only have one target destination, 
			// so at safe-zone, regardless of afraid or not, get into building
			//if (atSafeZone)
			{
				// Tell the target to add a new human represented by THIS
				// one just about to enter, then KILL this instance from
				// the visual screen
//				float remDist = Mathf.Abs( navAgent.remainingDistance );
//				string msg = "At safe zone: " +  gameObject.transform.parent.gameObject.name + "  "
//					+ "human position: " + gameObject.transform.position + "\r\n"
//					+ "target position: " + currentTarget.transform.position + "    "
//					+ "remain dist: " + remDist ;	
//				Debug.Log ( msg );

				currentTarget.SendMessage("addHuman");
				DestroyImmediate(gameObject);
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

		// get new target which defaults walk mode
		moveToNewTarget();

		// now, turn afraid to make human RUN
		makeAfraid(true);
	}

	private void makeAfraid(bool isNowAfraid )
	{
		// set flag that human is afraid, and then set the halo effect
		// which is a coloring under them when running to give visual effect
		// of movement state...
		isAfraid = isNowAfraid;
		// found this since depricated component.active.  We must call the BEHAVIOR of the component.
		// http://forum.unity3d.com/threads/component-active-is-obsolete-but-no-enabled-property.30105/
		((Behaviour)GetComponent("Halo")).enabled = isAfraid;

		if( animComponent != null )
		{
			animComponent.Stop();
			if( isAfraid )
			{
				navAgent.speed = runSpeed;
				animComponent.Play("sprint");
			}
			else
			{
				navAgent.speed = baseSpeed;
				animComponent.Play("walk");
			}
		}
	}


	// when a human is running away afraid, see after a certain amount of time
	// if there are any zombies STILL within a given proximity, if not, walk.
	private float lastAfraidCheck = 0.0f;

	private void checkIfStillAfraid()
	{	
		// if NOT afraid, nothing to do, get out.
		if( ! isAfraid  )
			return;

		lastAfraidCheck += Time.deltaTime;
		// if not 3 seconds since last check, don't bother and over cycle objects checking
		if( lastAfraidCheck < 1.5f )
			return;

		// we are within the time interval, reset counter for next time around
		// and then see IF there are any zombies left in the zone
		lastAfraidCheck = 0.0f;

		// if NO more zombies within 10 radius range, turn off the afraid flag
		if( ! gs.anyTagsInRange( transform.position, AfraidRadius, eNavTargets.Zombie ))
			makeAfraid(false);
	}
}
