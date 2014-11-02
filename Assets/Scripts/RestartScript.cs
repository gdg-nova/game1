using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class RestartScript : MonoBehaviour
{
	private GUISkin skin;
	
	void Start()
	{
		skin = Resources.Load("GUISkin") as GUISkin;
	}
	
	// Removed the Level Select as now part of the camStats U/I
}