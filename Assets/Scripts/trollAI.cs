using UnityEngine;
using System.Collections;

public class trollAI : commonAI
{
	gameControl gameController = null;
	
	public float updateSpeedInterval = .5f;
	private float timeSinceSpeedUpdate;
	
	//Add sound effects to the attacks
	private AudioSource[] soundEffects;

	public override void Start()
	{
		GameObject go = GameObject.FindWithTag("GameController");
		gameController = go.GetComponent<gameControl>();
		
		//Add all the sound effects to one object
		soundEffects = GetComponents<AudioSource> ();

		// do baseline start actions first...
		// also grabs default animation component too.
		base.Start();

		// upon first creation, create the common "attackAI" instance
		// Attack = this.gameObject.AddComponent<attackAI>();
		Attack = this.gameObject.GetComponent<attackAI>();

		// What can we attack... Guards and Humans
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

		completeInit();
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
		
		//check if time to attack again
		Attack.CheckAttack();
		
		// check if target was reached, if so, get new target
		if (reachedTarget ()) 
		{
			navAgent.Stop();
			navAgent.ResetPath();
			moveToNewTarget();
		}

		// in addition, check for being stagnant (clarification in commonAI.cs)
		IsMovementStagnant();
		
		checkAnimation ();
	}

	// see if the zombie is time to die and self-destruct
	private bool timeToDie()
	{
		// if already marked as being destroyed, don't do it again...
		if (isDestroying)
			return true;
		
		return isDestroying;
	}
	
	// When anything is attacked by a zombie, this method will be called.
	private void HandleSpecificHitTarget(Collider hit)
	{
		// ONLY if the object is a human do we create a newly spawned zombie.
		//Get points for every time you attack
		gameController.scorePoints();
	}
	
	public override void playSound (string action, string target)
	{
		//Play the correct sound:
		switch(target)
		{
			case "Human":
			{
				break;
			}
			case "Zombie":
			{
				//Debug.Log("Zombie attacked");
				//Guard_on_Zombie.Play();
				break;
			}
			case "Guard":
			{
				//	Debug.Log("Guard attacked");
				break;
			}
		}
		
	}
}
