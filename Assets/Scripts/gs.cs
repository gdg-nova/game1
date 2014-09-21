using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class gs
{
	public static Quaternion getRandomRotation() 
	{
		int y = Random.Range (0, 360);		
		Quaternion q = Quaternion.Euler (0, y, 0);		
		return q;
	}

	// look for a SPECIFIC SINGLE nav target.
	public static GameObject getRandomNavTarget(eNavTargets tagName) 
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag (tagName.ToString());

		// if there was not anything, can't return an indexed object of something doesn't exist
		if (targets.Length == 0)
			return (GameObject)null;

		// Yup, we have something, get a random instance from one available
		return targets [ Random.Range (0, targets.Length) ];
	}

	// if an object allows for multiple, try to find a target in the order
	// they are in their list.  Ex: a human look first for "SafeZone", then
	// looks for "Finish".  Return the first one where a target IS found...
	public static GameObject getRandomNavTarget(List<eNavTargets> tags)
	{
		// in case object instance destroyed somehow inbetween activities
		// and tags no longer available...
		if( tags == null || tags.Count == 0 )
			return null;

		GameObject gObj = null;
		foreach( eNavTargets oneTag in tags )
		{
			// look for a target based on first in the list available.
			gObj = getRandomNavTarget(oneTag);

			// if we got one, exit the loop, we are done
			if (gObj != null)
				break;
		}

		// return what, if any, was found
		return gObj;
	}
}


// these are the KNOWN tags of the game at present
// the commented ones are the ones not explicitly needed (yet)
public enum eNavTargets
{
	Finish,
	Guard,
	Human,
	SafeZone,
	Zombie 

	// Untagged
	// Respawn
	// EditorOnly
	// MainCamera
	// Player
	// GameController
	// Structure
	// Graveyard
	// Playable
	// ZombieTargetFlag
	// ZombieTarget
}
