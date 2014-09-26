using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// like that of the commonAI, a scary monster is something that
// has both attack AND scare capacities
public class scaryMonsterAI : commonAI
{
	private attackAI Attack;
	private castFearAI CastFear;
	
	// publicly expose some properties so we can pass them to the Attack object class
	public float attackInterval;
	public float attackRadius;
	public float attackDamage;
	
	// similar for casting fear into a human
	public float fearInterval;
	// what sphere range from the zombie radius 
	// to allow impact to casting fear.
	public float fearRadius;
	
	public override void Start()
	{
		// do baseline start actions first...
		// also grabs default animation component too.
		base.Start();
		
		// upon first creation, create the common "attackAI" instance
		Attack = this.gameObject.AddComponent<attackAI>();
		
		Attack.attackInterval = attackInterval;
		Attack.attackRadius = attackRadius;
		Attack.damage = attackDamage;
		
		// What can scary monsters attack... Guards and Humans
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
		
		// Now, for scaring humans (or whatever else may be scared away.
		CastFear = this.gameObject.AddComponent<castFearAI>();
		CastFear.fearInterval = fearInterval;
		CastFear.fearRadius = fearRadius;
		
		// Zombies have same default targets of SafeZone and Finish
		// only directly target zombies to safe or finish zones.
		// however, why dont the zombies try to target humans and guards too
		// maybe later
		defaultNavTargets.Clear();
		defaultNavTargets.Add(eNavTargets.SafeZone);
		defaultNavTargets.Add(eNavTargets.Finish);
		
		// pick a new target from either safe vs finish possibilities
		moveToNewTarget();
		
		// Initiate the zombie to walking animation
		animComponent.wrapMode = WrapMode.Loop;
		animComponent.Play("walk");
	}
	
	// What is difference between Update and FixedUpdate???
	void FixedUpdate() 
	{
		//check if time to attack again
		Attack.CheckAttack();
		
		// check if zombie cast fear in human object
		CastFear.CheckScare();
		
		// check if target was reached, if so, get new target
		if( ! reachedTarget())
			// in addition, check for being stagnant (clarification in commonAI.cs)
			IsMovementStagnant();
	}

	// When anything is attacked by a zombie, this method will be called.
	private void HandleSpecificHitTarget(Collider hit)
	{
		// ONLY if the object is a human do we create a newly spawned zombie.
		//if (hit.gameObject.tag.Equals("Human"))
		//	Camera.main.SendMessage("createZombie", hit.transform.position);
	}
}
