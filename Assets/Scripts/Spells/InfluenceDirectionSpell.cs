using UnityEngine;
using System.Collections;

public class InfluenceDirectionSpell : SpellBase {

	public GameObject magicalLightPrefab;

	// Use this for initialization
	void Start () {
	
		if (magicalLightPrefab == null)
		{
			Debug.LogError("MagicalLight Prefab must be specified for the InfluenceDirectionSpell");
		}
	}

	bool cancelled = false;
	bool firstMovementComplete = false;
	Vector3 lastTouchPosition = new Vector3();
	Vector3 firstDirection = new Vector3();

	public override void StartTouch (SceneControl.TouchEvent args)
	{
		cancelled = false;
		firstMovementComplete = false;

		// Now, what have we targeted from this click (if anything)
		Ray r = Camera.main.ScreenPointToRay (args.currentPosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			// Only concerned with terrains for the touch down
			if (r_hit.collider is TerrainCollider || r_hit.collider.name == "Playable Area")
			{
				lastTouchPosition = r_hit.point;
			}
			else
			{
				//Debug.Log ("Cancelling click");
				cancelled = true;
			}
		}
	}

	public override void MoveTouch (SceneControl.TouchEvent args)
	{
		if (cancelled) return;

		Ray r = Camera.main.ScreenPointToRay (args.currentPosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			Vector3 groundPoint = new Vector3(r_hit.point.x, lastTouchPosition.y, r_hit.point.z);
			if (!firstMovementComplete)
			{
				firstDirection = (groundPoint - lastTouchPosition).normalized;
				firstMovementComplete = true;
			}
			if ((groundPoint - lastTouchPosition).magnitude > 7.0f)
			{
				// Instantiate a magical light "spell"
				GameObject go = Instantiate( magicalLightPrefab, lastTouchPosition, Quaternion.Euler(-90.0f, 0.0f, 0.0f) ) as GameObject;
				Destroy(go, go.particleSystem.duration);

				lastTouchPosition = groundPoint;
			}
		}
	}

	public override void EndTouch (SceneControl.TouchEvent args)
	{
		if (cancelled || !firstMovementComplete) return;
		// End of the touch. Perform the influence now.
		Collider[] objs = Physics.OverlapSphere( args.initialPoint, 7.0f );
		foreach( Collider obj in objs)
		{
			commonAI gameObj = obj.GetComponent<commonAI>();
			if (gameObj != null)
			{
				ICanBeInfluenced inf = gameObj as ICanBeInfluenced;
				if (inf != null)
				{
					//Debug.Log ("Found object: " + obj.name);
					inf.BeInfluenced(firstDirection);
				}
			}
		}
		
	}
}
