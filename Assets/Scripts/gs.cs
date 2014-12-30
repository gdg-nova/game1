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

		// if we DO have targets, see if any are commonAI and are "infected".
		// if so, don't consider them from either side attraction/attack.
		for( int i=targets.Length -1; i > 0; i--)
		{
			commonAI o = targets[i].GetComponent<commonAI>();
			if( o != null && o.IsInfected )
				targets[i] = (GameObject)null;
		}

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


	public static bool anyTagsInRange(Vector3 fromHere, float withinRadius, eNavTargets anyOfThese )
	{ return anyTagsInRange( fromHere, withinRadius, anyOfThese, true ).Count > 0;	}
	
	public static List<GameObject> anyTagsInRange(Vector3 fromHere, float withinRadius, eNavTargets anyOfThese, bool getList )
	{
		string thisTag = anyOfThese.ToString();
		
		List<GameObject> goList = new List<GameObject>();
		
		foreach( Collider c in Physics.OverlapSphere(fromHere, withinRadius))
		{
			if( c.tag == thisTag )
				goList.Add(c.gameObject);
		}
		
		// nope, went through entire list of possible colliders, 
		// and nothing was what was looked for.
		return goList;
	}


	public static Vector3 RandomVectorInBounds(GameObject target) 
	{
		float x = Random.Range(target.renderer.bounds.min.x, target.renderer.bounds.max.x);
		float z = Random.Range(target.renderer.bounds.min.z, target.renderer.bounds.max.z);
		return new Vector3(x, target.transform.position.y, z);		
	}

	public static IEnumerable<T> GetItemsAsT<T>(this object[] objects) 
		where T:UnityEngine.Component
	{
		object tryObj;
		GameObject gObj;
		foreach(var obj in objects)
		{
			if( obj is GameObject )
			{
				gObj = (GameObject)obj;
				tryObj = (object)gObj.GetComponent<T>();
				
				if( tryObj is T )
				{
					Debug.Log ( "Found via IEnumerable" );
					yield return (T)tryObj;
				}
			}
		}
		yield break;
	}
}


// these are the KNOWN tags of the game at present
// the commented ones are the ones not explicitly needed (yet)
public enum eNavTargets
{
	Playable,
	Finish,
	Guard,
	Human,
	SafeZone,
	Zombie,
	Werewolf,
	HumanGathering

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
