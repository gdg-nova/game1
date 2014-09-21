using UnityEngine;
using System.Collections;

public class humanAI : commonAI, ICanBeScared
{
	// few more publics specific to humans...
	public Material fearMaterial;
	public bool stayInSafe;
	private bool isAfraid = false;

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
		GetComponent("Halo").active = true;

		moveToNewTarget();

		// RUN when afraid after new target destination established
		// the HumanHero with child object LittleHero_Solo has
		// animations of walk, idle, sprint
		navAgent.speed = runSpeed;
		PlayAnimation("sprint");
	}
}
