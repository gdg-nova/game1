using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class camStats : MonoBehaviour 
{
	private float mana = 0;
	private int score = 0;

	private Text txtScore;
	private Text txtZombieMana;

	private GameObject btnGameOver;
	private GameObject btnSelectLevel;
	private GameObject btnSpawnZombie;
	private GameObject btnSpawnWerewolf;
	private GameObject btnSpawnMinotaur;

	private gameControl gc;

	// Use this for initialization
	void Start () 
	{
		GameObject go = GameObject.FindWithTag ("GameController");
		Button b;

		if( go != null )
			gc = go.GetComponent<gameControl>();


		GameObject g;
		// Only look for other OBJECTS in this prefab.
		// I don't care about any controls OUTSIDE this prefab...
		foreach( Transform t1 in this.GetComponentInChildren<Transform>())
		{
			g = t1.gameObject;
			switch(g.name)
			{
				case "btnGameOver":
					btnGameOver = g;
					// Specifically add "Hook" to the button to restart game method
					// which in-turn, call the reload level option.
					b = g.GetComponent<Button>();
					if( b != null )
						b.onClick.AddListener(restartGame);

					// but disable the button for now as default so it is not shown

					break;

				case "btnSelectLevel":
					btnSelectLevel = g;
					// Specifically add "Hook" to the button to restart game method
					// which in-turn, call the reload level option.
					b = g.GetComponent<Button>();
					if( b != null )
						b.onClick.AddListener(selectLevel);
					
					break;
					
				case "btnSpawnZombie":
					btnSpawnZombie = g;
					// Specifically add "Hook" to the button to restart game method
					// which in-turn, call the reload level option.
					b = g.GetComponent<Button>();
					if( b != null )
						b.onClick.AddListener(spawnZombie);
					
					break;

				case "btnSpawnWerewolf":
					btnSpawnWerewolf = g;
					b = g.GetComponent<Button>();
					if( b != null )
					{
						b.onClick.AddListener(spawnWerewolf);
						// Disable for now until we ge werewolves
						b.gameObject.SetActive( false );
					}
					break;

				case "btnSpawnMinotaur":
					btnSpawnMinotaur = g;
					// Specifically add "Hook" to the button to restart game method
					// which in-turn, call the reload level option.
					b = g.GetComponent<Button>();
					if( b != null )
					{
						b.onClick.AddListener(spawnMinotaur);
						// Disable for now until we ge werewolves
						b.gameObject.SetActive( false );
					}
					break;


				case "txtScore":
					txtScore = (Text)g.GetComponent<Text>();
					txtScore.text = "";
					break;

				case "txtZombieMana":
					txtZombieMana = (Text)g.GetComponent<Text>();
					txtZombieMana.text = "";
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

	void Update() 
	{
		if( gc == null )
			return;
		
		delayShow -= Time.deltaTime;
		if( delayShow > 0 )
			return;

		delayShow = .5f;

		score = gc.score;
		mana = gc.manaPool;

		ShowMana();
		ShowScore ();
		ShowGameOver ();
	}

	private void ShowMana()
	{
		if( txtZombieMana == null )
			return;

		// Show mana as integer, no floating precision value
		txtZombieMana.text = mana.ToString("0");
	}

	private void ShowScore()
	{
		if( txtScore == null )
			return;

		txtScore.text = "Score: " + score.ToString("D5");
	}

	private void ShowGameOver()
	{
		if( btnGameOver.gameObject == null )
			return;

		btnGameOver.gameObject.SetActive( gc.gameEnded );

		if( gc.gameEnded )
			// NOW we can stop the game timer...
			Time.timeScale = 0;
	}

	
	private void spawnZombie()
	{
		graveYardAI gyAI = (graveYardAI)GameObject.FindObjectOfType<graveYardAI>();
		if( gyAI != null )
			gyAI.CreateZombie();
	}
	
	private void spawnWerewolf()
	{
		Debug.Log ( "Hook for spawning werewolf." );
	}
	
	private void spawnMinotaur()
	{
		Debug.Log ( "Hook for spawning minotaur." );
	}
}

