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
	
	void OnGUI()
	{
		const int buttonWidth = 120;
		const int buttonHeight = 70;
		
		GUI.skin = skin;
		
		// Draw a button to start the game
		if (
			GUI.Button(
			// Center in X, 2/3 of the height in Y
			new Rect(
			Screen.width / 16 - (buttonWidth / 2),
			(2 * Screen.height / 30) - (buttonHeight / 2),
			buttonWidth,
			buttonHeight
			),
			"Level Select"
			)
			)
		{
			// On Click, load the first level.
			// "Stage1" is the name of the first scene we created.
			Application.LoadLevel("level select scene");
		}
	}
}