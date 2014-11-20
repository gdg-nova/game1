using UnityEngine;
using System.Collections;

public class backToMenuScript : MonoBehaviour {

	public void JumpToMenuScene()
	{
		Debug.Log ("Test");
		Application.LoadLevel ("menu_Scene");
	}
}
