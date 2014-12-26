using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellController : MonoBehaviour {

	public PlayerSettings playerSettings = null;

	// Use this for initialization
	void Start () {

		if (playerSettings == null)
		{
			// Try to find it within the scene as a tag
			GameObject go = GameObject.FindGameObjectWithTag("PlayerSettings");
			playerSettings = go.GetComponent<PlayerSettings>();
		}

		GameObject[] objects = GameObject.FindGameObjectsWithTag("SpellRenderer");
		ToggleGroup toggleGroup = GetComponent<ToggleGroup>();
		foreach(GameObject go in objects)
		{
			// Set up each renderer
			RenderSpells rs = go.GetComponent<RenderSpells>();
			if (rs != null && playerSettings != null)
			{
				Debug.Log ("Setting spells on " + go.name);
				rs.toggleGroupToUse = toggleGroup;
				rs.SetSpells(playerSettings.AvailableSpells);
			}
		}
	}
	
}
