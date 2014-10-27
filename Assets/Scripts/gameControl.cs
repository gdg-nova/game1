using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class gameControl : MonoBehaviour 
{
	public GameObject Human;
	public GameObject Zombie;
	public GameObject Werewolf;
	public GameObject Guard;

	public float zombieCost = 1f;

	public int humanCount;
	public float timeLimit = 40;
	public float elapsedTime = 0; 

	private Quaternion q = Quaternion.Euler (0,0,0);
	public GameObject currentZombieTarget;
	public GameObject zombieTargetPrefab;

	public float manaPool = 3f;
	public GameObject manaDisplayUI;


	public touchController touchController = null;

	public bool gameEnded;

	public GUIText scoreGT;

	public int score;

	void Start() 
	{
		loadHumans();

		if (touchController == null)
			this.touchController = GetComponent<touchController>();

		//Get a reference to the ScoreCounter
		GameObject scoreGO = GameObject.Find ("ScoreCounter");
		//Get a reference to the GUIText of ScoreCounter
		scoreGT = scoreGO.GetComponent<GUIText> ();
		//Initiate the score to 0
		scoreGT.text = "0";
	}

	void Update() 
	{
		if (elapsedTime >= timeLimit)
			gameOver();

        updateManaPoolDisplay ();

		//check for mouse input
		CheckForLeftClick();

		CheckForRightMouse();

		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		elapsedTime += Time.deltaTime;
		
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
		if (scoreGT != null) 
		{
			//Convert text of score into an int
			score = int.Parse (scoreGT.text);
			//Add the value for converting a human to a zombie
			score += 100;
			//Convert back to string
			scoreGT.text = score.ToString ();
		}
	}

	private float expandingZombieTime;
	public GameObject ZombieRangePrefab;
	private GameObject zombieRange;

	// The zombie range selection process will now be a two-part process.
	// The FIRST cycle is selection via the circular expansion range is to
	// GET the zombies the player wants to control.  Once that is done, the
	// next single "touch"/mouse is the placement of the final target destination.
	// Also, the target destination, a user might click on a knight and smartly
	// target them even as a knight may move from original position.
	// DEFAULT the set destination mode to false... first time is picking, second is target
	private int zombieSelectionMode = 0;
	private List<GameObject> haloZombies;

	//mouse click handler
	void CheckForLeftClick() 
	{
		// Check first for touch controller.. if so, just get out, it forces ignore of 
		// any single key stroke / touch action.
		if (touchController && touchController.hasDonePinchToZoom)
		{
			// turn off selection zombie mode back to zero
			zombieSelectionMode = 0;
			return;
		}

		// each possible mode of selection is looking for either down or up
		switch( zombieSelectionMode )
		{
			// just starting, only engage IF the mouse is down..
			case 0:
				// if no mouse down, get out
				startSelectionProcess();
				break;

			case 1:
				// we already had down, now see if STILL down to expand range...
				expandSelectionRange();
				break;

			case 2:
				// expansion was finished via mouse-up button.
				// The NEW mouse down is a possible target (Knight to attack)
				// or new destination of specific point.
				finalizeZombieTarget();
				break;
		}
	}

	void startSelectionProcess()
	{
		// if no mouse down to start, just get out
		if( !Input.GetMouseButton(0))
			return;

		// if we HAVE zombies to clear, do so now...
		if( haloZombies != null )
		{
			Component gcomp;
			foreach( GameObject z in haloZombies )
				((Behaviour)z.GetComponent("Halo")).enabled = false;

			// clear once released to prevent auto-turning all back on again
			haloZombies = null;
		}

		// Now, what have we targeted from this click (if anything)
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;

		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			// Yup, what object...
			GameObject g = r_hit.collider.gameObject;
			
			// if graveyard, create new zombie directly there and get out.
			if (g.tag == "Graveyard")
			{
				// Should we do a graveyard click to create a zombie?
				// ONLY if there is enough mana to generate another...
				if( canIBuyAZombie() )
				{
					g.SendMessage ("Click");
					return;
				}
			}

			// test sending human to a safe-zone
			if( g.tag == "Human" )
			{
				commonAI cAI = g.GetComponent<commonAI>();
				if( cAI != null )
					cAI.navStopDistance = 14f;

				g.SendMessage ( "moveToNewTarget" );
			}
		}

		// we did not touch a graveyard, just use the mouse input position
		// as basis to start the circular selection area...
		// we are trying to create a new target location for zombies.
		// don't create the flag yet, but instead, turn on the expanding mode
		// and create a zombieRange object (simple cylinder) that expands based
		// on the duration of the mouse down until let up.
		expandingZombieTime = 0.0f;
		zombieRange = (GameObject)Instantiate( ZombieRangePrefab, r_hit.point, q );

		// set mode for next stage that allows EXPANDING the zombie range...
		zombieSelectionMode = 1;
	}

	// in selection mode already, now we are expanding the radius
	void expandSelectionRange()
	{
		// always calc time and expand the scale radius REGARDLESS of mouse button being up
		expandingZombieTime += Time.deltaTime;
		zombieRange.transform.localScale = new Vector3( expandingZombieTime * 75.0f, .2f, expandingZombieTime * 75.0f );

		// if mouse button is now UP, we need to select the zombies and turn on all their halo's
		if( Input.GetMouseButtonUp(0))
		{
			// now create the particle system in the center 
			// of where the range object was created.
			Transform t = zombieRange.transform;

			// from the central point of the get only zombies within a radius of this point, not ALL zombies...
			// Since the scale is a diameter, we want radius which is 1/2, so divide by 2.
			haloZombies = new List<GameObject>();
			List<GameObject> tmp = gs.anyTagsInRange( t.position, t.localScale.x / 2.0f, eNavTargets.Zombie, true );
			foreach( GameObject z in tmp)
				((Behaviour)z.GetComponent("Halo")).enabled = true;

			// Add zombies to list for turning OFF halo later...
			haloZombies.AddRange( tmp );

			// now, kill off the zombieRange item
			DestroyImmediate( zombieRange );

			// and set flag to perform the set destination
			zombieSelectionMode = 2;
		}
	}

	// third part of zombie selection.  Range expansion is done
	// and user clicking somewhere else for actual move zombies HERE...
	void finalizeZombieTarget()
	{
		// if no mouse down to start, just get out
		if( !Input.GetMouseButton(0))
			return;

		// Yes, we have a new mouse position... any possible taget?
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		// DID we point to anything from the touch?
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
		{
			// Yup, what object...
			GameObject g = r_hit.collider.gameObject;

			// did we hit a knight?? If so, the zombies will be
			// targeting whereever the KNIGHT MOVES TO...  (pending)

		}

		// establish target to move them to...
//		if (currentZombieTarget == null)
//			currentZombieTarget = (GameObject)Instantiate(zombieTargetPrefab, r_hit.point, Quaternion.Euler(0,0,0));
//		else 
//			currentZombieTarget.transform.position = r_hit.point;
		// Disable showing the red particle emitter for the target...
		// just set the target to the raycast hit object for target location
		currentZombieTarget = r_hit.collider.gameObject;

		zombieAI zAi;
		foreach( GameObject z in haloZombies )
		{
			// in case any are destroyed as a result of kill by a knight
			try
			{ 	
				zAi = z.GetComponent<zombieAI>();
				zAi.moveToSpecificGameObj( currentZombieTarget ); }
			catch
			{}
		}

		// set back to zero for next selection mode	
		zombieSelectionMode = 0;
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


	public bool canIBuyAZombie() 
	{ return zombieCost <= manaPool; }

	public bool requestBuyNewZombie(Vector3 position) 
	{
		if (zombieCost <= manaPool) 
		{
			createZombie(position);
			manaPool -= zombieCost;
			return true;
		}

		return false;
	}

	private void updateManaPoolDisplay() 
	{
		manaDisplayUI.GetComponent<Text>().text = manaPool.ToString();
	}

	//Create zombie at position
	public void createZombie( Vector3 position) 
	{
		Instantiate (Zombie, position, q); 
	}


	//Create zombie at position & rotation
	public zombieAI createZombie( Vector3 position, Quaternion rot) 
	{
		GameObject gobj = (GameObject)Instantiate (Zombie, position, rot); 
		zombieAI zAI = gobj.GetComponentInChildren<zombieAI>();
		return zAI;
	}

	void createWerewolf( Vector3 position) 
	{ Instantiate (Werewolf, position, q); }
	
	public humanAI createHuman( Vector3 position) 
	{
		GameObject gobj = (GameObject)Instantiate (Human, position, q); 
		humanAI hAI = gobj.GetComponentInChildren<humanAI>();
		return hAI;
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

	//Create Knight, such as exiting a building
	public guardAI createGuard( Vector3 position, Quaternion rot) 
	{
		GameObject gobj = (GameObject)Instantiate (Guard, position, rot); 
		guardAI gAI = gobj.GetComponentInChildren<guardAI>();
		return gAI;
	}
	
	void checkForWin() 
	{
		if (getHumansAliveCount() == 0 ) 
			gameOver();
	}
}
