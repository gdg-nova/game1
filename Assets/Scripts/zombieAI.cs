using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zombieAI : commonAI
{
	// default life-span time for a zombie.
	// self-die after given amount of time.
	public float lifeSpan;
	private float timeAlive = 0.0f;
	
	// Zombies can attack...
	attackAI Attack;

	// publicly expose some properties so we can pass them to the Attack object class
	public float attackInterval;
	public float attackRadius;
	public float attackDamage;


	// similar for casting fear into a human
	public float fearInterval;
	// what sphere range from the zombie radius 
	// to allow impact to casting fear.
	public float fearSize;
	// internal to track how frequent time checks
	private float timeSinceFear;


	public override void Start()
	{
		// do baseline start actions first...
		// also grabs default animation component too.
		base.Start();

		// upon first creation, create the common "attackAI" instance
		Attack = new attackAI();
		Attack.attackInterval = attackInterval;
		Attack.attackRadius = attackRadius;
		Attack.damage = attackDamage;
		
		// What can a Zombie attack... Guards and Humans
		// and don't forget about the SafeZone houses too!
		Attack.AddAttackTarget(eNavTargets.Human);
		Attack.AddAttackTarget(eNavTargets.Guard);
		Attack.AddAttackTarget(eNavTargets.SafeZone);
		
		// When an attack is successful, we need to do something
		// very specific if attacking a human (change them into a zombie)
		// attach to the public exposed event handler created
		Attack.OnWasAttacked += HandleSpecificHitTarget;

		// via the "Hero Zombie" prefab, it has a child element
		// of "LittleHero_solo" which has animations of
		// L2R_swipe, walk, die, idle_lookaround, hurt
		Attack.AssignAnimation( gameObject, animComponent, "L2R_swipe" );

		// Zombies have same default targets of SafeZone and Finish
		// only directly target zombies to safe or finish zones.
		// however, why dont the zombies try to target humans and guards too
		// maybe later
		defaultNavTagets.Add(eNavTargets.SafeZone);
		defaultNavTagets.Add(eNavTargets.Finish);

		// pick a new target from either safe vs finish possibilities
		moveToNewTarget();

		// Initiate the zombie to walking animation
		animComponent.wrapMode = WrapMode.Loop;
		animComponent.Play("walk");
	}
	
	// What is difference between Update and FixedUpdate???
	void FixedUpdate() 
	{
		// if time to die, get out, nothing more to do
		// zombies have a self-destruct time interval...
		if (timeToDie())
			return;

		//check if time to attack again
		Attack.CheckAttack();

		// check if zombie cast fear in human object
		checkCastFear();

		// check if target was reached, if so, get new target
		reachedTarget();
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

	// When anything is attacked by a zombie, this method will be called.
	private void HandleSpecificHitTarget(Collider hit)
	{
		// ONLY if the object is a human do we create a newly spawned zombie.
		if (hit.gameObject.tag.Equals("Human"))
			Camera.main.SendMessage("createZombie", hit.transform.position);
	}

	private void checkCastFear()
	{
		// update time since, and if not time, get out
		timeSinceFear += Time.deltaTime;
		if (timeSinceFear < fearInterval)
			return;

		// yup, time interval reached, try to cast fear.
		// reset timer variable
		timeSinceFear = 0;

		//get all objects that are hit within the range
		foreach (RaycastHit r in Physics.SphereCastAll(new Ray(transform.position, transform.forward), fearSize, fearSize))
		{
			// Only if hits a human, if so, invoke it's "Afraid" method via SendMessage
			if (r.collider.gameObject.tag == "Human")
				r.collider.gameObject.SendMessage("Afraid");
		}
	}
}
