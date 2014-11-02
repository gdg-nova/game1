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

	private Quaternion q = Quaternion.Euler (0,0,0);
	public GameObject zombieTargetPrefab;

	public float manaPool = 3f;

	public touchController touchController = null;

	public bool gameEnded;

	public int score;

	public GameObject throwingAnchorPrefab;
	private GameObject throwingAnchor;

	private float timeSinceCheckMouseInput = 0;
	public float  MouseCheckInputInterval = 1;

	void Start() 
	{
		loadHumans();

		if (touchController == null)
			this.touchController = GetComponent<touchController>();
	}

	void Update() 
	{
		//timeSinceCheckMouseInput += Time.deltaTime;

		//if (timeSinceCheckMouseInput >= MouseCheckInputInterval) {
			CheckForZombiePickup();
		//	timeSinceCheckMouseInput = 0;
		//		}

		CheckForRightMouse();

		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		checkForWin();
	}

	void gameOver() 
	{	gameEnded = true; }
	
	//Call this function from other scripts that want to add to the score
	public void scorePoints()
	{
		//Add the value for converting a human to a zombie
		score += 100;
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

	void CheckForZombiePickup ()
	{
		//if (! Input.GetMouseButton(0)) 
		//	return;

		if (Input.GetMouseButton (0)) {
//			Vector3 pos = Input.mousePosition;
//
//			//pos = Camera.main.ScreenToWorldPoint(pos);
//			pos.z = throwingAnchor.transform.position.z- Camera.main.transform.position.z;
//
//			//pos.y = 10;
//			throwingAnchor.transform.position = Camera.main.ScreenToWorldPoint(pos);
//

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 1000)) {
				Debug.Log ("Hit:  " +hit.collider.gameObject.tag);

						
				if (hit.collider.gameObject.tag == "Zombie" && throwingAnchor == null) {
					GameObject z = hit.collider.gameObject;

					throwingAnchor = (GameObject)Instantiate(throwingAnchorPrefab, z.transform.position + (Vector3.up * 2), Quaternion.Euler(0,0,0));

					//throwingAnchor.transform.position = hit.collider.gameObject.transform.position + (Vector3.up * 2);							
					throwingAnchor.GetComponent<SpringJoint> ().connectedBody = z.rigidbody;						

					z.SendMessage("PickedUp");

				} 
					if (hit.collider.gameObject.tag != "ThrowingAnchor" && throwingAnchor != null) {
							Vector3 newPos = hit.point;
							newPos.y = 8;
							throwingAnchor.transform.position = newPos;
					}
				}
				
		}else {
				if(throwingAnchor != null) {
					throwingAnchor.GetComponent<SpringJoint>().connectedBody.gameObject.SendMessage("Thrown");

					throwingAnchor.GetComponent<SpringJoint>().connectedBody = null;
				Destroy(throwingAnchor);	
			}

		}
		
		//throwingAnchor.transform.position = Input.mousePosition;

		//send raycast to get hit
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
//		
//		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) 
//		{
//			if (r_hit.collider.gameObject.tag == "Zombie") 
//			{
//				//Destroy human, create zombie
//				//createZombie(r_hit.collider.gameObject.transform.position);
//				//Destroy(r_hit.collider.gameObject);
//				r_hit.collider.gameObject.SendMessage("PickedUp");
//
//			}
//		}
	}

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
		bool ClickByEvent = Event.current.type == EventType.MouseDown
			&& Event.current.button == 0
				&& GUIUtility.hotControl == 0;
		
		// if no mouse down to start, just get out
		if( !ClickByEvent )
			return;

		// if we HAVE zombies to clear, do so now...
		if( haloZombies != null )
		{
			foreach( GameObject z in haloZombies )
			{
				try
				{
					((Behaviour)z.GetComponent("Halo")).enabled = false;
				}
				catch
				{}
			}

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

			// If we want to implement this then let's move it to the humanAI
			// and not to the game control area which is mostly for user input stuff.
			// test sending human to a safe-zone
			/*if( g.tag == "Human" )
			{
				commonAI cAI = g.GetComponent<commonAI>();
				if( cAI != null )
					cAI.navStopDistance = 14f;

				g.SendMessage ( "moveToNewTarget" );
			}*/
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
		zombieRange.transform.localScale = new Vector3( expandingZombieTime * 100.0f, .2f, expandingZombieTime * 100.0f );
		
		bool ClickByEvent = Event.current.type == EventType.MouseUp
			&& Event.current.button == 0
				&& GUIUtility.hotControl == 0;
		
		// if no mouse down to start, just get out
		if( !ClickByEvent )
			return;

		// if mouse button is now UP, we need to select the zombies and turn on all their halo's
		// now create the particle system in the center 
		// of where the range object was created.
		Transform t = zombieRange.transform;

		// from the central point of the get only zombies within a radius of this point, not ALL zombies...
		// Since the scale is a diameter, we want radius which is 1/2, so divide by 2.
		// Actually diving by 2.2 because the texture is smaller than the object, so it's more inline with texture which player can see.
		haloZombies = new List<GameObject>();
		List<GameObject> tmp = gs.anyTagsInRange( t.position, t.localScale.x / 2.2f, eNavTargets.Zombie, true );
		foreach( GameObject z in tmp)
			((Behaviour)z.GetComponent("Halo")).enabled = true;

		// Add zombies to list for turning OFF halo later...
		haloZombies.AddRange( tmp );

		// now, kill off the zombieRange item
		DestroyImmediate( zombieRange );

		// and set flag to perform the set destination
		if( haloZombies.Count == 0 )
			zombieSelectionMode = 0;
		else
			zombieSelectionMode = 2;
	}

	// third part of zombie selection.  Range expansion is done
	// and user clicking somewhere else for actual move zombies HERE...
	void finalizeZombieTarget()
	{
		// Down click on second is actual selection...
		bool ClickByEvent = Event.current.type == EventType.MouseDown
			&& Event.current.button == 0
				&& GUIUtility.hotControl == 0;
		
		// if no mouse down to start, just get out
		if( !ClickByEvent)
			return;
		
		// Yes, we have a new mouse position... any possible taget?
		Vector3 targetPos = Input.mousePosition;
		Ray r = Camera.main.ScreenPointToRay (targetPos);
		RaycastHit r_hit;
		Transform currentZombieTarget;

		if (Physics.Raycast(r, out r_hit, Mathf.Infinity)) 
			currentZombieTarget = r_hit.transform;

		GameObject czt = (GameObject)Instantiate(zombieTargetPrefab, r_hit.point, Quaternion.Euler(0,0,0));

		zombieAI zAi;
		foreach( GameObject z in haloZombies )
		{
			// in case any are destroyed as a result of kill by a knight
			try
			{
				zAi = z.GetComponent<zombieAI>();
				zAi.moveToSpecificTransform( czt.gameObject.transform );
			}
			catch(System.Exception)
			{ }
		}

		// clear the zombie target particle emitter delayed 1/2 second
		Destroy ( czt, .5f );

		// set to zero for next selection mode	
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
	
	public bool NoMoreZombiesOrMana() 
	{
		// Game should be over if we have no more zombies
		// AND not enough mana to generate new ones
		int baseCount = GameObject.FindGameObjectsWithTag ("Zombie").Length;
		return ( baseCount == 0 && !canIBuyAZombie()) ;
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
		// since zombies are our heros, the game is actually over
		// when we have no more zombies to convert humans into
		// AND there is no more mana to generate zombies.
		if( NoMoreZombiesOrMana() )
			gameOver();
	}
}
