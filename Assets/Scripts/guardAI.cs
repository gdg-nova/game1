using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guardAI : commonAI
{
	// create instance of the ATTACK object which will have
	// all the attack components...
	public attackAI Attack ;

	// publicly expose some properties so we can pass them to the Attack object class
	public float attackInterval;
	public float attackRadius;
	public float attackDamage;


	// is the guard a stationary unit or roaming unit
	public bool stationary;

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
		
		// At present, can only attack zombies and werewolves
		Attack.AddAttackTarget(eNavTargets.Zombie);

		// pass pointer to the one animation object so this and 
		// the attack object are working with the same animator and
		// not thinking of them as two separate instances of an animator

		// via the "Hero Guard" prefab, it has a child element
		// of "LittleKnight2_solo" which has animations of 
		// R2L_swipe, walk, die, idle_lookaround, idle, hurt
		Attack.AssignAnimation(gameObject, animComponent, "R2L_swipe");

		// if ever moving, look for Zombies or Werewolves.
		// Originally, the java script code was looking for safe-zones and
		// finish entities.  By default, normal targets are safe-zone and finish.
		// however, start by looking for zombies, then move to safe/finish zones
		defaultNavTargets.Clear ();
		defaultNavTargets.Add(eNavTargets.Zombie);
		defaultNavTargets.Add(eNavTargets.SafeZone);
		defaultNavTargets.Add(eNavTargets.Finish);

		if (stationary)
		{
			// if ever in combat, retain your post if marked as stationary.
			moveAfterCombat = false;

			// default animation is to be idle...
			animComponent.wrapMode = WrapMode.Loop;
			animComponent.Play("idle");
		}
		else
		{
			// When moving, a guard has a base speed of 3.5
			baseSpeed = 3.5f;
			animComponent.Play("walk");
			// animation speed for walking
			animComponent["walk"].speed = .4f;
			moveToNewTarget();
		}
	}

	void Update()
	{
		// if being destroyed, don't do anything else
		if (isDestroying)
			return;

		// check to see if not stationary, and reached its target,
		// to check/move to another target
		if (!stationary)
		{
			// if not stationary, the default animation is always walk,
			// no need to reset it from walk back to walk again...
			reachedTarget();

			// in addition, check for being stagnant (clarification in commonAI.cs)
			IsMovementStagnant();
		}

		// go to the Attack object to check for itself
		Attack.CheckAttack();

		// try to begin walking again after any attack in case the
		// enemy was destroyed, we have something to again move towards
		if (!stationary)
			animComponent.Play ("walk");
	}
}
