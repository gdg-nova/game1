using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class camStats : MonoBehaviour 
{
	private float mana = 0;
	private int score = 0;
	private float gameTime = 0.0f;

	private TextMesh showScore;
	private TextMesh showMana;
	private TextMesh showGameTime;

	private GameObject showGameOver;
	private GameObject showSelectLevel;

	gameControl gc;

	// Use this for initialization
	void Start () 
	{
		GameObject go = GameObject.FindWithTag ("GameController");
		Button b;
		if( go != null )
			gc = go.GetComponent<gameControl>();

		// See IF there is a "GameController" object
		foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>())
		{
			switch(g.name)
			{
				case "btnGameOver":
					showGameOver = g;
					// Specifically add "Hook" to the button to restart game method
					// which in-turn, call the reload level option.
					b = g.GetComponent<Button>();
					if( b != null )
						b.onClick.AddListener(restartGame);

					// but disable the button for now as default so it is not shown

					break;

				case "btnSelectLevel":
					showSelectLevel = g;
					// Specifically add "Hook" to the button to restart game method
					// which in-turn, call the reload level option.
					b = g.GetComponent<Button>();
					if( b != null )
						b.onClick.AddListener(selectLevel);
				
					break;

				case "txtUpperLeft":
					showScore = (TextMesh)g.GetComponent<TextMesh>();
					break;

				case "txtUpperRight":
					showMana = (TextMesh)g.GetComponent<TextMesh>();
					break;

				case "txtLowerRight":
					showGameTime = (TextMesh)g.GetComponent<TextMesh>();
					break;
			}


		}
	}
	
	private void restartGame()
	{
		selectLevel();
		// restart animations and activity
		Time.timeScale = 1;
	}

	private void selectLevel()
	{
		try
		{
			Application.LoadLevel("LevelChooseScene");
		}
		catch
		// if on demo screen and no such level, just ignore it.
		{}
	}


	// Update is called once per frame
	float delayShow = 0.0f;
	void Update () 
	{
		delayShow -= Time.deltaTime;
		if( delayShow > 0 )
			return;

		delayShow = .5f;

		if( gc == null )
			return;

		score = gc.score;
		mana = gc.manaPool;

		ShowMana();
		ShowScore ();
		ShowGameTime ();
		ShowGameOver ();
	}

	private void ShowMana()
	{
		if( showMana == null )
			return;

		// Show mana as integer, no floating precision value
		showMana.text = mana.ToString("0");
	}

	private void ShowScore()
	{
		if( showScore == null )
			return;

		showScore.text = "Score: " + score.ToString("D5");
	}

	private void ShowGameTime()
	{
		gameTime += Time.deltaTime;
		if( showGameTime == null )
			return;

		showGameTime.text = "Game Time: " + gameTime;
	}

	private void ShowGameOver()
	{
		if( showGameOver.gameObject == null )
			return;

		showGameOver.gameObject.SetActive( gc.gameEnded );

		if( gc.gameEnded )
			// NOW we can stop the game timer...
			Time.timeScale = 0;
	}
}

