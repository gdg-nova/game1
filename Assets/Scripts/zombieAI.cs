using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zombieAI : commonAI, ICanBeInfluenced
{
	// default life-span time for a zombie.
	// self-die after given amount of time.
	public float lifeSpan = 1;
	private float timeAlive = 0.0f;

	public float fastZombieSpeed = 7.0f;

	public float nearbyZombieRange = 3f;
	public float zombieManaCost = 1f;
	//public float manaGenerated

	public float updateSpeedInterval = .5f;
	private float timeSinceSpeedUpdate;

	// Zombies can attack...
	// these scripts will also be added to the zombie at interface
	// time and we can adjust attack and cast fear properties there.
	private castFearAI CastFear;

	private bool readyToBeInfluenced = false;

	public override void Start()
	{
		manaCost = zombieManaCost;

		//StartCoroutine ("updateSpeedbyNearbyZombies");

		lifeSpan = 5000f;
		// do baseline start actions first...
		// also grabs default animation component too.
		base.Start();
		
		// upon first creation, create the common "attackAI" instance
		// Attack = this.gameObject.AddComponent<attackAI>();
		Attack = this.gameObject.GetComponent<attackAI>();

		// Now, for scaring humans (or whatever else may be scared away.
		// CastFear = this.gameObject.AddComponent<castFearAI>();
		CastFear = this.gameObject.GetComponent<castFearAI>();

		// make sure that the castFear interval is not less than the attack,
		// otherwise every human would be cast "afraid" before a zombie could
		// attack them and turn human into a zombie.
		if( CastFear.fearInterval < Attack.attackInterval )
			// for grins add 1/100 second to delay
			CastFear.fearInterval = Attack.attackInterval + .01f;

		// What can a Zombie attack... Guards and Humans
		// and don't forget about the SafeZone houses too!
		Attack.AddAttackTarget(eNavTargets.Human);
		Attack.AddAttackTarget(eNavTargets.Guard);
		Attack.AddAttackTarget(eNavTargets.SafeZone);

		// Attack successful handler, called when we hit another object.
		// We delegate this directly to the global handler.
		Attack.SuccessfulAttack += (object sender, attackAI.SuccessfulAttackEventArgs e) => globalEvents.OnEnemyElementsHit(sender, e.objectHit);

		// via the "Hero Zombie" prefab, it has a child element
		// of "LittleHero_solo" which has animations of
		// L2R_swipe, walk, die, idle_lookaround, hurt
		Attack.AssignAnimation( gameObject, animComponent, "L2R_swipe" );		
		
		// Zombies have same default targets of SafeZone and Finish
		// only directly target zombies to safe or finish zones.
		// however, why dont the zombies try to target humans and guards too
		// maybe later
		defaultNavTargets.Clear();
		defaultNavTargets.Add(eNavTargets.SafeZone);
		defaultNavTargets.Add(eNavTargets.Finish);
		// if all else fails, find SOME place in the playable area of the board
		defaultNavTargets.Add(eNavTargets.Playable);


		// Zombies (walk, idle_lookaround, hurt, die, L2R_swipe)
		AnimationClip ac = animComponent.GetClip ("idle_lookaround");
		ac.wrapMode = WrapMode.Loop;
		ac = animComponent.GetClip ("hurt");
		ac.wrapMode = WrapMode.Once;
		ac = animComponent.GetClip ("die");
		ac.wrapMode = WrapMode.Once;
		ac = animComponent.GetClip ("L2R_swipe");
		ac.wrapMode = WrapMode.Once;

		// for commonAI during "takeDamage" from attacking 
		hasDieAnimation = true;
		hasHurtAnimation = true;

		float animSpeed = -.5f;

		animComponent.animation ["die"].speed = animSpeed;

		float animTime = Mathf.Abs ( animComponent.animation ["die"].length * (1 / animSpeed));

		animComponent.animation ["die"].time = animComponent.animation ["die"].length;
		animComponent.Play ("die");



		Invoke ("completeInit",animTime);


	}

	// when a zombie kills a running human, it comes back as a FAST zombie
	public void MakeFastZombie()
	{
		Start ();
		// set speed AFTER the default start is called.
		//baseSpeed = fastZombieSpeed;
		runSpeed = fastZombieSpeed;
	}


	public void setTarget( GameObject newTarget)
	{
		// In case object is being destroyed, don't allow set
		if( isDestroying )
			return;
		
		// in case issue with human-zombie conversion and navAgent lost
		if( navAgent != null )
			navAgent.SetDestination(newTarget.transform.position); 
	}


	public void completeInit() 
	{
		animComponent.animation ["die"].speed = 1f;
		animComponent.animation ["die"].time = 0f;

		animComponent.Play ();

		//animComponent.Play ("walk");
		//animComponent.wrapMode = WrapMode.Loop;


		readyToBeInfluenced = true;
		// pick a new target from either safe vs finish possibilities
		moveToNewTarget();
	}

	// What is difference between Update and FixedUpdate???
	void FixedUpdate() 
	{
		// if time to die, get out, nothing more to do
		// zombies have a self-destruct time interval...
		if (timeToDie())
			return;

		timeSinceSpeedUpdate += Time.deltaTime;
		if (timeSinceSpeedUpdate >= updateSpeedInterval) {
			updateSpeedbyNearbyZombies();
				}


		//check if time to attack again
		Attack.CheckAttack();
		
		// check if zombie cast fear in human object
		CastFear.CheckScare();
		
		// check if target was reached, if so, get new target
		if (reachedTarget ()) {
			navAgent.Stop();
			navAgent.ResetPath();
			//navAgent.speed =0;	
		}
		//	moveToNewTarget();

		// in addition, check for being stagnant (clarification in commonAI.cs)
		IsMovementStagnant();

		checkAnimation ();

	}

	void updateSpeedbyNearbyZombies() {


		float newSpeed = baseSpeed + (getNearbyZombieCount() * baseSpeed);

//		Debug.Log ("basespeed:  " + baseSpeed);

//		Debug.Log ("updateSpeed: " + newSpeed);

		navAgent.speed = newSpeed;

		timeSinceSpeedUpdate = 0;


	}

	private float getNearbyZombieCount() {
		float nearbyZombieCount = 0f;

		Collider[] colliders = Physics.OverlapSphere(transform.position, nearbyZombieRange);
		//bool anythingWasHit = false;

		//GameObject hitObj;
		foreach (Collider hit in colliders) {
			if (hit.gameObject.tag == "Zombie") {
				nearbyZombieCount += 1;
			}
		}

		return nearbyZombieCount;
	}

	// see if the zombie is time to die and self-destruct
	private bool timeToDie()
	{
		// if already marked as being destroyed, don't do it again...
		if (isDestroying)
			return true;
		
		timeAlive += Time.deltaTime;
		if (timeAlive >= lifeSpan)
			// NOW, destroy the zombie
			die();
		
		return isDestroying;
	}

	#region ICanBeInfluenced

	public void BeInfluenced (Vector3 directionOfInfluence)
	{
		if (!readyToBeInfluenced || isDestroying) return;

	}

	#endregion ICanBeInfluenced
	
	public override void playSound (string action, string target)
	{
		audio.Play();
	}
}
//