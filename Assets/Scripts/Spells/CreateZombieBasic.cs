﻿using UnityEngine;
using System.Collections;

public class CreateZombieBasic : SpellBase {

	public GameObject zombieAppearanceEffect;
	public GameObject zombiePrefab;
	
	// Use this for initialization
	void Start () {
		
		if (zombiePrefab == null)
		{
			Debug.LogError ("Zombie prefab is missing");
		}
	}

	public override eSpellType type { get {	return eSpellType.kCreateMonster; }	}
	
	public override void StartTouch (SceneControl.TouchEvent args)
	{
		// Now, what have we targeted from this click (if anything)
		Ray r = Camera.main.ScreenPointToRay (args.currentPosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			GameObject zai = globalEvents.manaControllerService.RequestBuyZombie(r_hit.point, new Quaternion());
			if (zai != null)
			{
				zombieAI ai = zai.GetComponent<zombieAI>();
				if (ai != null)
				{
					ai.Start ();
				}

				if (zombieAppearanceEffect != null)
				{
					GameObject go = Instantiate( zombieAppearanceEffect, r_hit.point, Quaternion.Euler(-90.0f, 0.0f, 0.0f) ) as GameObject;
					Destroy(go, go.GetComponent<ParticleSystem>().duration);
				}
			}
		}
	}

}
