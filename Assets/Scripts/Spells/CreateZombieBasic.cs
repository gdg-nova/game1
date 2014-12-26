using UnityEngine;
using System.Collections;

public class CreateZombieBasic : SpellBase {

	public GameObject zombieAppearanceEffect;
	public GameObject zombiePrefab;
	
	// Use this for initialization
	void Start () {
		
		if (zombieAppearanceEffect == null)
		{
			Debug.LogError("Appearance Prefab must be specified for the LightningSpell");
		}
		if (zombiePrefab == null)
		{
			Debug.LogError ("Zombie prefab is missing");
		}
	}

	public override eSpellType type { get {	return eSpellType.kCreateMonster; }	}
	
	public override void StartTouch (SceneControl.TouchEvent args)
	{
		// Now, what have we targeted from this click (if anything)
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			GameObject go = Instantiate( zombieAppearanceEffect, r_hit.point, Quaternion.Euler(-90.0f, 0.0f, 0.0f) ) as GameObject;
			Destroy(go, go.particleSystem.duration);

			GameObject zo = Instantiate( zombiePrefab, r_hit.point, new Quaternion() ) as GameObject;
		}
	}

}
