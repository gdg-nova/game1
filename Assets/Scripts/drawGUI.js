	//public var  zombieUITexture : Texture2D ;
	//public var humanUITexture : Texture2D;
	//public var barBackTexture : Texture2D;
	
	
	public var zombieUIStyle : GUIStyle;
	public var humanUIStyle :GUIStyle;
	public var barUIBackStyle : GUIStyle;

	public var gameControlInstance : gameControl;

function Start() {
	gameControlInstance = GetComponentInParent(gameControl);
	
	
}

function OnGUI() {
	Debug.Log(gameControlInstance.timeLimit);
			
	//Show remaining time		
	GUI.Box( Rect(10, 10, 100, 25), (gameControlInstance.timeLimit - gameControlInstance.elapsedTime).ToString("00") );
	
	
	drawStatusBar();
	
	if (gameControlInstance.gameEnded) {
		//Buttons like Angry Birds:
		drawGameEnd();
		
	}
}

function drawGameEnd() {
		//Retry
			//Continue (if win)
			//Main menu
	
		var UIwidth:int = Screen.width /2;
		var UIheight:int  = Screen.height /2;
	
		var UItop = (Screen.height /2) - (UIheight/2);
		var UIleft = (Screen.width /2) - (UIwidth/2);
	
		GUI.Box( Rect(UIleft,UItop, UIwidth, UIheight), "Game Over!");
}

function drawStatusBar() {

	//var humansAlive : int = GameObject.FindGameObjectsWithTag ("Human").Length;
	var zombiesAlive : int = GameObject.FindGameObjectsWithTag ("Zombie").Length;
		
	//var zombieStyle : GUIStyle = new GUIStyle();
	//zombieStyle.normal.background = zombieUITexture;
	//zombieStyle.padding = 10;
	
		
	//var h :GUIStyle = new GUIStyle();
	//h.normal.background = humanUITexture;
	
	//var barBackStyle : GUIStyle = new GUIStyle();
	//barBackStyle.normal.background = barBackTexture;
	
	var barWidth : float = Screen.width /2;
		
	var pixelValue : float =  barWidth/ gameControlInstance.humanCount;
	
	
	GUILayout.BeginArea(Rect(Screen.width/3, 0, barWidth, 85));
	
	//apportion based on # of humans, zombies, deceased
		GUILayout.BeginHorizontal(barUIBackStyle);
				//GUILayout.TextArea("Humans", GUIStyle(zombieUIStyle), GUILayout.Width(humansAlive * pixelValue));
				//GUILayout.Label("Zombies", GUILayout.Width(zombiesAlive * pixelValue));
			
			//GUILayout.Box(zombieUITexture, GUILayout.Width(100));
			
			
			GUILayout.Box("", humanUIStyle, GUILayout.Width(gameControlInstance.getHumansAliveCount() * pixelValue), GUILayout.Height(40));
			GUILayout.Box("", zombieUIStyle, GUILayout.Width(zombiesAlive * pixelValue), GUILayout.Height(40));
		
			GUILayout.EndHorizontal();
	GUILayout.EndArea();
	
	//begin horizontal layout
	
	//end horizontal layout
}
