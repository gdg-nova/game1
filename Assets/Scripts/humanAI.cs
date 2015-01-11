using UnityEngine;
using System.Collections;

public class humanAI : commonAI, ICanBeScared, ICanBeStruckDown, ICanBeConvertedToMonster
{
	// few more publics specific to humans...
	public float AfraidRadius = 2.5f;

	public bool stationary;

	// 0 = not-knocked down, 1 = falling down, 2 = getting back up
	public int knockedDownState { get; protected set; }

	// Use this for initialization
	public override void Start () 
	{


		// set Random rotation for visual interest
		// use global static object to utilize smaller memory
		// footprint vs per instance of class
		transform.rotation = gs.getRandomRotation();

		// human navigation targets... look for safe-zones first, then finish zones
		defaultNavTargets.Clear();
		// make human gathering spots a basis for default movement instead of
		// always going and hiding in a "SafeZone"
		defaultNavTargets.Add(eNavTargets.HumanGathering );
		defaultNavTargets.Add(eNavTargets.SafeZone);
		defaultNavTargets.Add(eNavTargets.Finish);
		// if all else fails, find SOME place in the playable area of the board
		defaultNavTargets.Add(eNavTargets.Playable);

		// do baseline start actions first...
		base.Start();


		// humans also have SPRINT mode animation which is always loop mode
		AnimationClip ac = animComponent.GetClip ("sprint");
		ac.wrapMode = WrapMode.Loop;

		// ensure we have "die" animation or not
		hasDieAnimation = animComponent["die"] != null;

		// get initial target for human...
		if (stationary)
		{
			// if ever in combat, retain your post if marked as stationary.
			moveAfterCombat = false;
			
			// default animation is to be idle...
			//animComponent.Play("idle");
		}
		else
		{
			// moving to new target will always initiate walking mode.
			moveToNewTarget();
		}

		animComponent.Play ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (knockedDownState > 0)
			return;

		// if no target, get one now regardless of requiring 
		// "SafeZone" vs "Finish"
		if( currentTarget == null && !stationary)
			moveToNewTarget();

		// if no target due to object being destroyed (such as safe-zone house), 
		// get out and try again next cycle
		if( currentTarget == null )
			return;

		// if was an infected human and we just create a new instance
		// of the infection (ie: Werewolf now), get out, all done
		if( IsInfectedHuman() )
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
			if (stationary)
			{
			//	if( animComponent["walk"].enabled )
		//			animComponent.Play ( "idle" );
			}
			else {
			navAgent.speed = baseSpeed;
		//	animComponent.Play ("walk");
			}

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

			// hard test for going to safe-zone as human for generating as guard on way out
			 if (( true || isAfraid) && atSafeZone && currentTarget != null)
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

		checkAnimation ();
	}

	// new delay for humans when moving to randomized "human gathering" locations
	// such as light posts, barrels, etc... set some random delay, much like letting
	// them sit and chat with others that may be near same gathering spot...
	float gatheringSpotDelay = 0.0f;
	// Called from the update method...
	private bool GatheringSpotHandled()
	{
		// if current target is not a human gathering spot, 
		// get out.  Nothing handled assocated with gathering spot.
		if( currentTarget.tag != eNavTargets.HumanGathering.ToString())
			return false;

		// if previously established and target reached, 
		// have we met our time-delay before going to next target?
		if( gatheringSpotDelay > 0f )
		{
			// reduce delay time, if delay completed, get out and return
			// FALSE... we are not doing anything specific to gathering spot
			gatheringSpotDelay -= Time.deltaTime;
			if( gatheringSpotDelay < 0f )
				return false;

			// we are still in the delay timeout of gathering.
			// take a look at other human/guard game objects in the vicinity
			// as we are so we can "LOOK" at them as if to be engaging in
			// converstation.  Similar approach to that of COMBAT where the
			// enemies are forced to look at each other when being attacked.
			Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 1);
			GameObject hitObj;
			foreach (Collider hit in colliders)
			{
				hitObj = hit.collider.gameObject;
				// did we find an object we are allowed to attack
				if( hitObj.tag.Equals ( "Human" )
				   || hitObj.tag.Equals( "Guard" ))
				{
					commonAI o = hitObj.GetComponent<commonAI>();
					// turn our game object in the direction of what is being attacked
					transform.LookAt(hitObj.transform);

					// stop our game object from walking / moving around
					animComponent.Play("idle");

					// break out of loop of possible humans to look at.
					break;
				}
			}
	
			// yes, we handled something (even if ATTEMPTING to look at other humans)
			return true;
		}

		// Not already within a timing-out delay at gathering point.
		// We may still be walking towards our gathering spot... did we get there yet?
		if( reachedTarget())
		{
			// Yup, we DID hit our gathering spot... set delay and get out
			// random timeout is 5 seconds minimum PLUS another 1 to 5 seconds
			gatheringSpotDelay = 5f + Random.Range( 1f, 5f );
			return true;
		}

		// nope, nothing handled... user still walking...
		return false;
	}



	private bool infectionDelaySet;	
	private float infectionDelay;

	private bool IsInfectedHuman()
	{
		if( !IsInfected )
			return false;

		// set our own timer
		if( !infectionDelaySet )
		{
			infectionDelay = 5.0f;
			infectionDelaySet = true;
			return false;
		}

		// subtract time... if not there yet, just get out
		// turn off flag so not recursive every cycle
		infectionDelay -= Time.deltaTime;
		if( infectionDelay > 0 )
			return false;

		// clear infection flag for false cycle until human actually destroyed...
		IsInfected = false;

		// Yup, infection time was reached, create a new werewolf and kill self.
		werewolfAi wAI = globalEvents.characterCreator.createWerewolf( gameObject.transform.position, gameObject.transform.rotation );
		if( wAI != null )
			wAI.InfectedHumanWerewolf = true;

		// set flag and self-destroy after the werewolf is created...
		isDestroying = true;
		// Destroy the game object of the human now that werewolf is created above
		Destroy ( this.gameObject );
		return true;
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
				//animComponent.Play("sprint");
			}
			else
			{
				navAgent.speed = baseSpeed;
				//animComponent.Play("walk");
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

	protected override void OnDie ()
	{
		// ONLY create a zombie if it was not killed by a werewolf.
		if( ! attackedByWerewolf )
		{
			if( isAfraid )
				Invoke ("requestZombieCreationFast", animation["walk"].length * 2 );
			else
				Invoke ("requestZombieCreation", animation["walk"].length * 2 );

			// Mana should be incremented when the human dies in this way
			globalEvents.manaControllerService.ChangeMana(+1);
		}
	}
	
	protected void requestZombieCreation()
	{
		globalEvents.characterCreator.createZombie(gameObject.transform.position, Quaternion.Euler (0,0,0));
	}
	
	protected void requestZombieCreationFast() 
	{
		zombieAI zAI = globalEvents.characterCreator.createFastZombie(gameObject.transform.position, Quaternion.Euler (0,0,0));
		zAI.MakeFastZombie();
	}

	#region ICanBeStruckDown

	public void StrikeDown ()
	{
		if (knockedDownState > 0)
			return; // Do nothing

		// No matter the current state, set in motion the strike down behavior
		AnimationState dieAnim;
		knockedDownState = 1;
		if (animComponent != null && (dieAnim = animComponent["die"]) != null)
		{
			navAgent.Stop ();
			dieAnim.speed = 1.0f;
			dieAnim.wrapMode = WrapMode.Once;
			animComponent.Play("die");

			Invoke ("WaitSomeTimeKnockedDown", dieAnim.length);
		}
		else
		{
			Invoke ("StartMovingAgain", Random.value*2.0f);
		}
	}

	private void WaitSomeTimeKnockedDown()
	{
		Invoke ("GetBackUpAfterKnockDown", Random.value*1.0f );
	}

	private void GetBackUpAfterKnockDown()
	{
		// This method is called when getting back up.
		AnimationState dieAnim;
		knockedDownState = 2;
		if (animComponent != null && (dieAnim = animComponent["die"]) != null)
		{
			dieAnim.time = dieAnim.length;
			dieAnim.speed = -1.0f;
			animComponent.Play("die");
			
			Invoke ("StartMovingAgain", dieAnim.length);
		}
		else
		{
			StartMovingAgain();
		}
	}

	private void StartMovingAgain()
	{
		// After getting up we should be afraid because something just happened.
		knockedDownState = 0;
		Afraid();
	}

	#endregion ICanBeStruckDown

	#region ICanBeConvertedToMonster

	public void ConvertToMonster ()
	{
		if (globalEvents.manaControllerService.CanIBuyAZombie())
		{
			die ();
			globalEvents.manaControllerService.ChangeMana(-2); // -1 for the Zombie, -1 to reverse effect OnDie() has
		}
	}

	#endregion ICanBeConvertedToMonster
	
	public override void playSound (string action, string target)
	{
		// TODO: No sounds
		return;
	}
}
