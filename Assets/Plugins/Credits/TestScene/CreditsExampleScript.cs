using UnityEngine;
using System.Collections;

public class CreditsExampleScript : MonoBehaviour
{
	void Start()
	{
		// Starting manually the credit roll if "Play On Awake" is false:
		//Camera.main.GetComponent<Credits>().beginCredits();
		// or:
		//Camera.main.SendMessage("beginCredits");

		// Callback
		Camera.main.GetComponent<Credits>().endListeners += new Credits.CreditsEndListener(creditsEnded); // creditsEnded is the name of the function
	}
	
	void creditsEnded(Credits c)
	{
		Debug.Log("Credit roll finished!");
	}
}
