﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// there are many things that can attack...
// Guards (can attack zombies or werewolves or whatever)
// Zombies and Werewolves can attack guards and humans
// etc...
public class attackAI : MonoBehaviour 
{
	// First, what targets can something attack...
	// it may be multiple things they can attack...
	// such as a Zombie can attack a human, guard and
	// even a "SafeZone"
	private List<string> canAttackThese;

	// All things that can attack can have an amount of
	// damage they can inflict on said target(s)
	public float damage = 1.0f;

	// how frequent do we allow attacks during game time	
	public float attackInterval;
	// sphere radius to look for possible targets within range
	// to attempt attacking..
	public float attackRadius;

	// internally, to keep track of when last attack was
	// so we don't over-attack every frame
	private float timeSinceAttack;

	// each attack object can have an animation
	private Animation animComponent;
	
	private GameObject associatedWith;
	// expose so using game objects holding this object can
	// set the swipe/attack direction.
	private string attackAnimation;

	//Add sound effect to the attacks
	public AudioSource[] soundEffects;
	//Sound effect for zombie on peasant
	public AudioSource Zombie_on_Peasant;



	public void Start()
	{
		// Add all the sound effects to one object
		soundEffects = GetComponents<AudioSource> ();
		Zombie_on_Peasant = soundEffects [0];
	}

	// expose an event for a successful attack in case any custom
	// needs to act upon... Ex: a zombie or werewolf attacks a human,
	// we want to spawn a new zombie or werewolf at the human position
	// and then kill the human instance.
	// Create a new delegate so the parameter must be that of a "GameObject"
	public delegate void WasAttackedDelegate(Collider hit);
	public event WasAttackedDelegate OnWasAttacked;

	public attackAI()
	{
		// instantiate list regardless of the "Start" method
		canAttackThese = new List<string>();
	}

	// expose to pass in the same animation object from the commonAI
	// object.  Because anyone can have a simple "walk" or "die" animation.
	// but others with attack can have addition such as slashing during the
	// actual attack event.  One pointer to the same object.
	public void AssignAnimation( GameObject gameobj, Animation fromCommonAI, string animName )
	{ 
		// passed in the actual game object this class is connected to.
		associatedWith = gameobj;
		animComponent = (fromCommonAI); 
		attackAnimation = animName;
	}

	// don't allow the "commonAI" to update this attack object list directly
	public void AddAttackTarget(eNavTargets newTarget)
	{
		// store the STRING tag version we expect the objects to have
		// for use of comparing the "tag" property of collision objects found
		canAttackThese.Add(newTarget.ToString());
	}

	// simple method to return true/false if a given
	// string IS part of the list of valid targets.
	private bool canAttack( string thisTag )
	{
		string foundTag = canAttackThese.Find(
			(a) => String.Equals( a, thisTag, StringComparison.CurrentCultureIgnoreCase));

		return ( foundTag != null && foundTag.Length > 0 );
	}

	//attack with spherecast
	public void CheckAttack()
	{
		//update attack timer
		timeSinceAttack += Time.deltaTime;

		// if not yet time to attack, get out
		if (timeSinceAttack < attackInterval)
			return;

		// see if we need to reroute to new target if another enemy is nearby
		CheckForNearbyEnemy();

		// reset timer to allow for next attack check
		timeSinceAttack = 0;

		// sphere cast around the attacking game object.
		Collider[] colliders = Physics.OverlapSphere(associatedWith.transform.position, attackRadius);

		// get pointer to this as an type-casted object of commonAI
		commonAI thisAttacker = GetComponent<commonAI>();

		GameObject hitObj;
		foreach (Collider hit in colliders)
		{
			hitObj = hit.collider.gameObject;
			// did we find an object we are allowed to attack
			if( canAttack( hitObj.tag ))
			{
				commonAI o = hitObj.GetComponent<commonAI>();
				// if the object is already infected, such as
				// a zombie or werewolf attempting to attack
				// a human infected, don't attack it again...
				if( o != null && o.IsInfected )
					continue;

				if (o != null && !o.isDestroying) 
				{
					//Attacking a valid live target
				
					o.playSound( "attack", hitObj.tag.ToString() );

					//Play the correct sound:
					switch(hitObj.tag.ToString())
					{
					case "Human":
					{
						Zombie_on_Peasant.Play();
						break;
					}
					case "Zombie":
					{
						Debug.Log("Zombie attacked");
						//Play knight on zombie sound
						break;
					}
					case "Guard":
					{
						Debug.Log("Guard attacked");
						//Play zombie on guard sound
						break;
					}
					}


				
					//Debug.Log ("valid attack hit: " + hitObj);

					// mark this as being engaged in combat attack mode
					thisAttacker.EngagedInCombat = true;

					// turn our game object in the direction of what is being attacked
					transform.LookAt(hitObj.transform);

					// based on the initial animation, what is their respective
					// attack animation name string
					animComponent.wrapMode = WrapMode.Once;
					animComponent.Play(attackAnimation);

					// if there was a subscription to this event, pass the
					// game object hit to the calling source.
					if (OnWasAttacked != null)
						OnWasAttacked(hit);

						// just to track engaged in attack/combat
					o.EngagedInCombat = true;

					// Then, put into queue to get back to walking mode
					// put this to the tail-end of the animation cycle
					if( o.moveAfterCombat )
						PlayWalkAnimation();
						
					
					// stop the game object from moving as we have just engaged attack
					// dont keep walking if it was attacked
					o.stop();

					// Now, apply damage to other
					o.takeDamage(damage, thisAttacker);

					// if we just finished infecting the enemy, 
					// take ourselves out of combat and go to another target
					if( o.IsInfected )
					{
						thisAttacker.moveAfterCombat = true;
						thisAttacker.EngagedInCombat = false;
						return;
					}


					// if we just killed the target,
					// we need to take ourselves OUT of combat mode
					// (unless something else hits IT again)
					if( o.isDestroying )
						thisAttacker.EngagedInCombat = false;


					// set flag we hit a valid expected object and exit the loop.
					// we can't attack more than one thing

			//		break;
					//}			
				} 
				else 
				{
					//attacking a house
					object o2 = hitObj.GetComponent<safeZoneAI>();
					if( o2 is safeZoneAI )
					{
						// a safe-zone or finish never is of a commonAI and 
						// never moves at the end of combat.
						animComponent.wrapMode = WrapMode.Once;
						animComponent.Play(attackAnimation);

						((safeZoneAI)o2).takeDamage( damage );
					}
				}
				return;
			}
		}
	}

	public void PlayWalkAnimation() {
		if (animComponent.GetClip ("walk") != null) {
				animComponent.PlayQueued ("walk", QueueMode.CompleteOthers);
			} else {
				if (animComponent.GetClip("LM_walk") != null) {
				animComponent.PlayQueued ("LM_walk", QueueMode.CompleteOthers);			
			}
		}
	}

	// look for an enemy with three attack distance radius 
	// of where we are moving...
	public void CheckForNearbyEnemy()
	{
		// use sae attack interval on check, but don't decrease/reset it
		if (timeSinceAttack < attackInterval)
			return;
		
		// sphere cast around the attacking game object.
		Collider[] colliders = Physics.OverlapSphere(associatedWith.transform.position, attackRadius * 3.0f);

		// get pointer to this as an type-casted object of commonAI
		commonAI thisAttacker = GetComponent<commonAI>();
		
		GameObject hitObj;
		foreach (Collider hit in colliders)
		{
			hitObj = hit.collider.gameObject;

			commonAI o = hitObj.GetComponent<commonAI>();
			// if already infected, don't falsely keep an attacker
			// such as werewolf to keep tracking it...
			if( o != null && o.IsInfected )
				continue;

			// did we find an object we are allowed to attack
			if( canAttack( hitObj.tag ))
			{
				thisAttacker.moveToSpecificTransform( hitObj.transform );
				return;
			}
		}
	}

}
