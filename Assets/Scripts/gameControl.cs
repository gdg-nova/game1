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
		CheckForLeftClick();

		CheckForRightMouse();

		elapsedTime += Time.deltaTime;
		
		if (elapsedTime >= timeLimit)
			gameOver();			

		checkForWin();
	}

	void gameOver() 
	{
		Time.timeScale = 0;
		gameEnded = true;
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

	void createZombieTargetFlag (Transform zRange)
	{
		// DVR, when using mouse click to expand a range to attract zombies,
		// use the transform as basis of new central point location and also
		// it's radius (x and z are same value) as range to include for zombies
		// to be attracted
		if (currentZombieTarget == null)
			currentZombieTarget = (GameObject)Instantiate(zombieTargetPrefab, zRange.position, Quaternion.Euler(0,0,0));
		else 
			currentZombieTarget.transform.position = zRange.position;
		
		// get only zombies within a radius of this point, not ALL zombies...
		// Since the scale is a diameter, we want radius which is 1/2, so divide by 2.
		List<GameObject>zombies = gs.anyTagsInRange( zRange.position, zRange.localScale.x / 2.0f, eNavTargets.Zombie, true );

		foreach( GameObject z in zombies)
		{
			//Debug.Log ("Zombie is:" + z);
			zombieAI zAi = z.GetComponent<zombieAI>();

			zAi.moveToSpecificGameObj( currentZombieTarget );
		}
	}

	private bool expandingZombieRange;
	private float expandingZombieTime;
	public GameObject ZombieRangePrefab;
	private GameObject zombieRange;

	//mouse click handler
	void CheckForLeftClick() 
	{

		// if we are already IN a left-click zombie range implementation,
		// check for the mouse UP event...
		if( expandingZombieRange )
		{
			expandingZombieTime += Time.deltaTime;

			zombieRange.transform.localScale = new Vector3( expandingZombieTime * 20.0f, .2f, expandingZombieTime * 20.0f );
			
			if( Input.GetMouseButtonUp(0))
			{
				// turn off mouse-down mode when mouse comes up...
				expandingZombieRange = false;
				// now create the particle system in the center of where 
				// the range object was created.
				createZombieTargetFlag( zombieRange.transform );
				// now, kill off the zombieRange item
				DestroyImmediate( zombieRange );
			}
		}
		else
		{
			// no mouse down yet... check for it now.
			// if no left-click, get out
			if (! Input.GetMouseButton(0))
				return;
			
			//send raycast to get hit
			GameObject g;
			Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit r_hit;
			
			if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
			{
				g = r_hit.collider.gameObject;
				
				//Handle click based on clicked object tag:
				if (g.tag == "Human") 
					g.SendMessage("die");
				
				// if graveyard, create new zombie directly there.
				else if (g.tag == "Graveyard")
					g.SendMessage ("CreateZombie");
				else
				{
					// we are trying to create a new target location for zombies.
					// don't create the flag yet, but instead, turn on the expanding mode
					// and create a zombieRange object (simple cylinder) that expands based
					// on the duration of the mouse down until let up.
					expandingZombieRange = true;
					expandingZombieTime = 0.0f;
					zombieRange = (GameObject)Instantiate( ZombieRangePrefab, r_hit.point, q );
				}
			}
		}
	}
	
	void CheckForRightMouse() 
	{
		if (! Input.GetMouseButton(1)) 
			return;

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


	//Create zombie at position & rotation
	public void createZombie( Vector3 position, Quaternion rot) 
	{ Instantiate (Zombie, position, rot); }

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
