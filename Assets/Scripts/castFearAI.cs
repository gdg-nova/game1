using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// some monsters can cast fear in humans.
// like that of attack, keep this a more generic
// that can be attached to zombies or werewolves or whatever
public class castFearAI : MonoBehaviour 
{
	// how frequent do we allow scare during game time	
	public float fearInterval;
	// sphere radius to look for possible targets within range
	// to attempt scaring..
	public float fearRadius;
	
	// internally, to keep track of when last scare was
	// so we don't over-attack every frame
	private float timeSinceScare;

	// scare with spherecast
	public void CheckScare()
	{
		//update attack timer
		timeSinceScare += Time.deltaTime;
		
		// if not yet time to scare, get out
		if (timeSinceScare < fearInterval)
			return;
		
		// reset timer to allow for next scare check
		timeSinceScare = 0;
		
		//sphere cast in front of attacking game object.  
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, fearRadius);

		GameObject scareObj;
		foreach (Collider hit in colliders)
		{
			scareObj = hit.collider.gameObject;

			commonAI o = scareObj.GetComponent<commonAI>();
			if( o != null && o is ICanBeScared )
				((ICanBeScared)o).Afraid();

			// unlike attacking.  A group of humans in a given
			// area can ALL be scared away, don't stop at first one.
		}
	}
}
