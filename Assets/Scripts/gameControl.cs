using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameControl : MonoBehaviour 
{
	public GameObject Human;
	public GameObject Zombie;
	public GameObject Werewolf;
	
	public int humanCount;
	public float timeLimit = 30;
	public float elapsedTime = 0; 

	private Quaternion q = Quaternion.Euler (0,0,0);
	public GameObject currentZombieTarget;
	public GameObject zombieTargetPrefab;

	public bool gameEnded;

	public GUIText scoreGT;

	public int score;

	void Start() 
	{
		loadHumans();
	
		//Get a reference to the ScoreCounter
		GameObject scoreGO = GameObject.Find ("ScoreCounter");
		//Get a reference to the GUIText of ScoreCounter
		scoreGT = scoreGO.GetComponent<GUIText> ();
		//Initiate the score to 0
		scoreGT.text = "0";
	}

	void Update() 
	{
		//check for mouse input
		if (Input.GetMouseButton(0))
			clickObject();

		if (Input.GetMouseButton(1)) 
			rightclickObject();

		elapsedTime += Time.deltaTime;
		
		if (elapsedTime >= timeLimit)
			gameOver();			

		checkForWin();
	}

	void gameOver() 
	{
 //		print("GameOver!");
		
		Time.timeScale = 0;
		gameEnded = true;
	}

	//update human/zombie counter
	void FixedUpdate() 
	{
		//Debug.Log ("Humans: " + GameObject.FindGameObjectsWithTag ("Human").Length);
		//Debug.Log ("Zombies: " + GameObject.FindGameObjectsWithTag ("Zombie").Length);
	}

	//mouse click handler
	//mouse click handler
	void clickObject() 
	{
		//send raycast to get hit
		GameObject g;
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) {
			g = r_hit.collider.gameObject;

			//Handle click based on clicked object tag:
			if (g.tag == "Human") {
					//Destroy human, create zombie
					createZombie (g.transform.position);
			
					Destroy (g);

			}
			// if graveyard, create new zombie directly there.
			else if (g.tag == "Graveyard")
					g.SendMessage ("CreateZombie");
			else {
					// if it did NOT hit any target, then set a target position
					// for any zombies NEAR this position click.
//				if (currentZombieTarget != null)
//				{	
//					currentZombieTarget.SendMessage ("removeZombieTarget");
//				}
					
				createZombieTargetFlag(r_hit.point);
			}				
		}
	}
	//Call this function from other scripts that want to add to the score
	public void scorePoints()
	{
		//Convert text of score into an int
		score = int.Parse(scoreGT.text);
		//Add the value for converting a human to a zombie
		score += 100;
		//Convert back to string
		scoreGT.text = score.ToString();
	}
	void createZombieTargetFlag (Vector3 targetPoint)
	{
		if (currentZombieTarget == null)
			currentZombieTarget = (GameObject)Instantiate(zombieTargetPrefab, targetPoint, Quaternion.Euler(0,0,0));
		else 
			currentZombieTarget.transform.position = targetPoint;
		
		// get only zombies within a radius of this point, not ALL zombies...
		List<GameObject>zombies = gs.anyTagsInRange( targetPoint, 20.0f, eNavTargets.Zombie, true );

//		Debug.Log ("Found zombie:" + zombies.Count);

		foreach( GameObject z in zombies)
		{
			//Debug.Log ("Zombie is:" + z);
			zombieAI zAi = z.GetComponent<zombieAI>();

			zAi.moveToSpecificGameObj( currentZombieTarget );
		}

//		Debug.Log ("current zombie target:" + currentZombieTarget);
	}

	void rightclickObject() 
	{
		//send raycast to get hit
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			if (r_hit.collider.gameObject.tag == "Human") 
			{
				//Destroy human, create zombie
				createZombie(r_hit.collider.gameObject.transform.position);
				Destroy(r_hit.collider.gameObject);
			}
		}
	}

	//Create humans per HumanCount
	void loadHumans() 
	{
		for (int i = 0; i < humanCount; i++) 
		{
			GameObject spawn = getRandomSpawn();
			// if no such object found randomly, get out of loop
			if( spawn == null )
				break;

			//Find random position within Spawn plane
			createHuman( gs.RandomVectorInBounds( spawn )); 
		}
	}

	GameObject getRandomSpawn() 
	{
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
		if( spawns.Length == 0 )
			return null;

		return spawns [Random.Range (0, spawns.Length)];
	}

	//Create zombie at position
	public void createZombie( Vector3 position) 
	{ Instantiate (Zombie, position, q); }
	
	void createWerewolf( Vector3 position) 
	{ Instantiate (Werewolf, position, q); }
	
	public humanAI createHuman( Vector3 position) 
	{
		GameObject go = (GameObject)Instantiate (Human, position, q); 
		return (humanAI)go.GetComponent<humanAI>();
	}
	
	public int getHumansAliveCount() 
	{
		int baseCount = GameObject.FindGameObjectsWithTag ("Human").Length;
		GameObject[] safeZone  = GameObject.FindGameObjectsWithTag("SafeZone");
		foreach( GameObject zone in GameObject.FindGameObjectsWithTag("SafeZone") )
		{
			safeZoneAI safeAI = (safeZoneAI)zone.GetComponent ("safeZoneAI");
			baseCount = baseCount + safeAI.humanCount;
		}
		return baseCount;
	}

	void checkForWin() 
	{
		if (getHumansAliveCount() == 0 ) 
			gameOver();
	}


}
