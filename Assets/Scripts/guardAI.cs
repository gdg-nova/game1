using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guardAI : commonAI
{
	// publicly expose some properties so we can pass them to the Attack object class
	public float attackInterval;
	public float attackRadius;
	public float attackDamage;

	// is the guard a stationary unit or roaming unit
	public bool stationary;

	public override void Start()
	{
		// upon first creation, create the common "attackAI" instance
		Attack = this.gameObject.AddComponent<attackAI>();

		Attack.attackInterval = attackInterval;
		Attack.attackRadius = attackRadius;
		Attack.damage = attackDamage;

		// Guard_on_Zombie = GetComponent<AudioSource>();

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
		defaultNavTargets.Add(eNavTargets.Werewolf);
		//defaultNavTargets.Add(eNavTargets.SafeZone);
		//defaultNavTargets.Add(eNavTargets.Finish);
		// if all else fails, find SOME place in the playable area of the board
		//defaultNavTargets.Add(eNavTargets.Playable);

		// do baseline start actions first...
		// also grabs default animation component too.
		base.Start();

		// special animation flags uses in commonAI when attacked and dying
		// so as to not duplicate code at this or zombie levels.
		hasHurtAnimation = true;
		hasDieAnimation = true;

		// Guards (walk, idle, idle_lookaround, hurt, die, R2L_swipe)
		AnimationClip ac = animComponent.GetClip ("idle");
		ac.wrapMode = WrapMode.Loop;
		ac = animComponent.GetClip ("idle_lookaround");
		ac.wrapMode = WrapMode.Loop;
		ac = animComponent.GetClip ("hurt");
		ac.wrapMode = WrapMode.Once;
		ac = animComponent.GetClip ("die");
		ac.wrapMode = WrapMode.Once;
		ac = animComponent.GetClip ("R2L_swipe");
		ac.wrapMode = WrapMode.Once;

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
	}

	public void Update()
	{
		// if being destroyed, don't do anything else
		if (isDestroying)
			return;

		// check to see if not stationary, and reached its target,
		// to check/move to another target
		if (stationary)
		{
	//		if( animComponent["walk"].enabled )
	//			animComponent.Play ( "idle" );
		}
		else
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
		//if (!stationary)
			//animComponent.Play ("walk");

		checkAnimation ();
	}

	public override void playSound (string action, string target)
	{
		audio.Play ();
	}
}
