using UnityEngine;
using System.Collections;

public class houseLighting : MonoBehaviour 
{
	Light[] houseLights;

	// Use this for initialization
	void Start () 
	{
		houseLights = gameObject.GetComponentsInChildren<Light>();
		Debug.Log ( "House Light Count:" + houseLights.Length );
		// default, turn off all the lights...
//		foreach( Light hl in houseLights )
//			hl.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach( Light hl in houseLights )
		{
//			hl.gameObject.SetActive(false);
//			hl.intensity = .6f;
//			hl.color = Color.white;
			hl.color = new Color( 255f, 255f, 0f );
//			hl.gameObject.SetActive(true);
		}
	}
}
