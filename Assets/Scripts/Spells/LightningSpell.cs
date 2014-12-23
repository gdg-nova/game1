using UnityEngine;
using System.Collections;

public class LightningSpell : SpellBase {

	public GameObject lightningPrefab;

	// Use this for initialization
	void Start () {
	
		if (lightningPrefab == null)
		{
			Debug.LogError("Lightning Prefab must be specified for the LightningSpell");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void EndTouch (SceneControl.TouchEvent args)
	{
		// Now, what have we targeted from this click (if anything)
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			Instantiate( lightningPrefab, r_hit.point, Quaternion.Euler(-90.0f, 0.0f, 0.0f) );
		}
	}
}
