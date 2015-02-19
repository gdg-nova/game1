using UnityEngine;
using System.Collections;

public class LightningSpell : SpellBase {

	public GameObject lightningPrefab;
	public float lightningRange = 3.0f;

	// Use this for initialization
	void Start () {
	
		if (lightningPrefab == null)
		{
			Debug.LogError("Lightning Prefab must be specified for the LightningSpell");
		}
	}
	
	public override void EndTouch (SceneControl.TouchEvent args)
	{
		// Now, what have we targeted from this click (if anything)
		Ray r = Camera.main.ScreenPointToRay (args.currentPosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			GameObject go = Instantiate( lightningPrefab, r_hit.point, Quaternion.Euler(-90.0f, 0.0f, 0.0f) ) as GameObject;

			Collider[] objs = Physics.OverlapSphere( r_hit.point, lightningRange );
			foreach( Collider obj in objs)
			{
				commonAI gameObj = obj.GetComponent<commonAI>();
				if (gameObj != null)
				{
					ICanBeStruckDown strike = gameObj as ICanBeStruckDown;
					if (strike != null)
					{
						//Debug.Log ("Found object: " + obj.name);
						strike.StrikeDown();
					}
				}
			}

			Destroy(go, go.particleSystem.duration);

		}
	}
}
