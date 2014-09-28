using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class drawGUI : MonoBehaviour 
{
	public GUIStyle zombieUIStyle;
	public GUIStyle humanUIStyle;
	public GUIStyle barUIBackStyle;
	
	public gameControl gameControlInstance;
	
	void Start() 
	{
		gameControlInstance = (gameControl)GetComponentInParent<gameControl>();
	}

	void OnGUI() 
	{
		//Show remaining time		
		GUI.Box( new Rect(10, 10, 100, 25), 
		        (gameControlInstance.timeLimit - gameControlInstance.elapsedTime).ToString("00") );
		
		drawStatusBar();

		if (gameControlInstance.gameEnded)
			drawGameEnd();
	}
	
	void drawGameEnd() 
	{
		//Retry
		//Continue (if win)
		//Main menu
		
		int UIwidth = Screen.width /2;
		int UIheight = Screen.height /2;
		
		int UItop = (Screen.height /2) - (UIheight/2);
		int UIleft = (Screen.width /2) - (UIwidth/2);
		
//		int conversions = gameControlInstance.score / 100;
		
		GUI.Box (new Rect (UIleft, UItop, UIwidth, UIheight), 
		         "\nStarting number of humans: " + gameControlInstance.humanCount
		         + "\nNumber of humans remaining: "// + (gameControlInstance.humanCount - conversions)
		         + "\nNumber of zombies converted: "// + conversions
		         + "\nYour final score is: "// + gameControlInstance.score
		         );
		GUI.enabled = false;
			}
	
	void drawStatusBar() 
	{
		float barWidth = Screen.width /2;
		float pixelValue = 1;
//		float pixelValue =  barWidth/ gameControlInstance.humanCount;

		// visual indentation only for layout 
		// begin/end then again for horizontal
		GUILayout.BeginArea(new Rect(Screen.width/3, 0, barWidth, 85));
			GUILayout.BeginHorizontal(barUIBackStyle);
				//GUILayout.TextArea("Humans", GUIStyle(zombieUIStyle), GUILayout.Width(humansAlive * pixelValue));
				//GUILayout.Label("Zombies", GUILayout.Width(zombiesAlive * pixelValue));
				//GUILayout.Box(zombieUITexture, GUILayout.Width(100));
				

				// Since there may be humans inside safe zones, we need the game controller to gather
				// all safe zones plus any roaming the grounds
//				GUILayout.Box("", humanUIStyle, GUILayout.Width( gameControlInstance.getHumansAliveCount() * pixelValue), GUILayout.Height(40));

				int zombiesAlive = GameObject.FindGameObjectsWithTag("Zombie").Length;
				GUILayout.Box("", zombieUIStyle, GUILayout.Width(zombiesAlive * pixelValue), GUILayout.Height(40));
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
