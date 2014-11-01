using UnityEngine;
using System.Collections;

public class camStats : MonoBehaviour 
{
	private float mana = 0;
	private int score = 0;
	private float gameTime = 0.0f;

	private TextMesh showScore;
	private TextMesh showMana;
	private TextMesh showGameTime;

	private GameObject showGameOver;

	gameControl gc;

	// Use this for initialization
	void Start () 
	{
		GameObject go = GameObject.FindWithTag ("GameController");
		gc = go.GetComponent<gameControl> ();

		// See IF there is a "GameController" object
		foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>())
		{
			switch(g.name)
			{
				case "btnGameOver":
					showGameOver = g;
					break;

				case "txtUpperLeft":
					showMana = (TextMesh)g.GetComponent<TextMesh>();
					break;

				case "txtUpperRight":
					showScore = (TextMesh)g.GetComponent<TextMesh>();
					break;

				case "txtLowerLeft":
					showGameTime = (TextMesh)g.GetComponent<TextMesh>();
					break;
			}


		}
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
		showMana.text = "Mana: " + mana.ToString("0");
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
		if( showGameOver == null )
			return;

		showGameOver.gameObject.SetActive( gc.gameEnded );
	}
}

