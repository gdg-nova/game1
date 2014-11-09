using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class werewolfAi : commonAI
{
	// publicly expose some properties so we can pass them to the Attack object class
	public float attackInterval;
	public float attackRadius;
	public float attackDamage;
	
	// is the guard a stationary unit or roaming unit
	public bool stationary;
	
	//Sound effect for guard on zombie
	//private AudioSource Guard_on_Zombie;
	private bool infectedHumanWerewolf;
	private float turnBackIntoHuman = 0.0f;
	public bool InfectedHumanWerewolf
	{	get { return infectedHumanWerewolf; }
		set { infectedHumanWerewolf = value;
			turnBackIntoHuman = 5.0f;
		}
	}
	
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
		
		// Guard_on_Zombie = GetComponent<AudioSource>();
		

		Attack.AddAttackTarget(eNavTargets.Human);
		Attack.AddAttackTarget(eNavTargets.Guard);
		Attack.AddAttackTarget(eNavTargets.SafeZone);
		
		// pass pointer to the one animation object so this and 
		// the attack object are working with the same animator and
		// not thinking of them as two separate instances of an animator
		
		// via the "Hero Guard" prefab, it has a child element
		// of "LittleKnight2_solo" which has animations of 
		// R2L_swipe, walk, die, idle_lookaround, idle, hurt
		Attack.AssignAnimation(gameObject, animComponent, "LM_R2L_swipe");
		
		// if ever moving, look for Zombies or Werewolves.
		// Originally, the java script code was looking for safe-zones and
		// finish entities.  By default, normal targets are safe-zone and finish.
		// however, start by looking for zombies, then move to safe/finish zones
		defaultNavTargets.Clear ();
		//defaultNavTargets.Add(eNavTargets.Zombie);
		defaultNavTargets.Add(eNavTargets.Human);
		defaultNavTargets.Add(eNavTargets.Guard);
		defaultNavTargets.Add(eNavTargets.SafeZone);
		defaultNavTargets.Add(eNavTargets.Finish);
		// if all else fails, find SOME place in the playable area of the board
		defaultNavTargets.Add(eNavTargets.Playable);
		
		// special animation flags uses in commonAI when attacked and dying
		// so as to not duplicate code at this or zombie levels.
		hasHurtAnimation = true;
		hasDieAnimation = true;
		
		// Guards (walk, idle, idle_lookaround, hurt, die, R2L_swipe)
		AnimationClip ac = animComponent.GetClip ("LM_idle");
		ac.wrapMode = WrapMode.Loop;
		ac = animComponent.GetClip ("LM_idle_lookaround");
		ac.wrapMode = WrapMode.Loop;
		ac = animComponent.GetClip ("LM_hurt");
		ac.wrapMode = WrapMode.Once;
		ac = animComponent.GetClip ("LM_die_collapse");
		ac.wrapMode = WrapMode.Once;
		ac = animComponent.GetClip ("LM_R2L_swipe");
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

		// if not stationary, the default animation is always walk,
		// no need to reset it from walk back to walk again...
		reachedTarget();
			
		// in addition, check for being stagnant (clarification in commonAI.cs)
		IsMovementStagnant();

		// go to the Attack object to check for itself
		Attack.CheckAttack();
		
		checkAnimation ();

		changeBackIntoHuman();
	}
	
	public override void playSound (string action, string target)
	{
		// TODO: No sounds
		return;
	}

	public override string getAnimNameforCurrentState() 
	{
		if( navAgent == null)
			return "LM_idle";
		
		if (!navAgent.hasPath) 
		{
			// if no navigation path target, get a new one now.
			// dont know why it was going into idle mode.
			moveToNewTarget();
			return "LM_idle";
		}

		if (navAgent.speed > 0 && navAgent.speed <= 5) 
			return "LM_walk";
		else if (navAgent.speed > 5 && navAgent.speed <= 12) 
			return "LM_run";
		else if (navAgent.speed > 12) 
			return "LM_sprint";

		// all else, return idle
		return "LM_idle";
	}

	// if not infected werewolf swapping back and forth, 
	// don't need to do anything...
	private void changeBackIntoHuman()
	{
		if( ! infectedHumanWerewolf )
			return;

		// subtract from time remaining and get out if not there yet...
		turnBackIntoHuman -= Time.deltaTime;
		if( turnBackIntoHuman > 0f)
			return;

		// Time Hit...  Now, create a human and keep it marked as infected
		// so any other werewolf won't attack it... like they can smell
		// already infected werewolf humans and leave them alone.
		GameObject go = GameObject.FindWithTag ("GameController");
		gameControl gc = go.GetComponent<gameControl> ();
		humanAI hAI = gc.createHuman( gameObject.transform.position );
		if( hAI != null )
		{
			hAI.transform.rotation = gameObject.transform.rotation;
			hAI.IsInfected = true;
		}
		
		// set flag and self-destroy after the werewolf is created...
		isDestroying = true;
		// Destroy the game object of the human now that werewolf is created above
		Destroy ( this.gameObject );
		return;
	}
}
