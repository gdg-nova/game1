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

	void Start() 
	{
		loadHumans();
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
		print("GameOver!");
		
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
	void clickObject() 
	{
		//send raycast to get hit
		GameObject g;
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		if (Physics.Raycast (r,  out r_hit, Mathf.Infinity)) 
		{
			g = r_hit.collider.gameObject;

			//Handle click based on clicked object tag:
			if (g.tag == "Human") 
			{
				//Destroy human, create zombie
				createZombie(g.transform.position);
				Destroy(g);
			} 
			else if (g.tag == "SafeZone") 
			{
				if (   currentZombieTarget != null 
				    && currentZombieTarget.tag == "SafeZone") 
					currentZombieTarget.SendMessage("removeZombieTarget");

				g.SendMessage("makeZombieTarget");
				currentZombieTarget = g;
				
				assignCurrentZombieTarget();		
				
			} 
			else if (g.tag == "Graveyard")
				g.SendMessage("CreateZombie");

			else
				//Move CurrentZombieTarget to location
				createZombieTargetFlag(r_hit.point);
		}
	}

	void createZombieTargetFlag (Vector3 targetPoint)
	{
		if (currentZombieTarget == null)
			currentZombieTarget = (GameObject)Instantiate(zombieTargetPrefab, targetPoint, Quaternion.Euler(0,0,0));
		else 
		{
			if (currentZombieTarget.tag == "SafeZone")
				currentZombieTarget.SendMessage("removeZombieTarget");
			else
				currentZombieTarget.transform.position = targetPoint;
		}
		assignCurrentZombieTarget();		
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
			float x = Random.Range(spawn.renderer.bounds.min.x, spawn.renderer.bounds.max.x);
			float z = Random.Range(spawn.renderer.bounds.min.z, spawn.renderer.bounds.max.z);
			
			Instantiate(Human, new Vector3(x, .5f, z), q);
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
	void createZombie( Vector3 position) 
	{ Instantiate (Zombie, position, q); }

	void createWerewolf( Vector3 position) 
	{ Instantiate (Werewolf, position, q); }

	void createHuman( Vector3 position) 
	{ Instantiate (Human, position, q); }

	int getHumansAliveCount() 
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

	Vector3 getRandomLocationinBounds(GameObject target) 
	{
		float x = Random.Range(target.renderer.bounds.min.x, target.renderer.bounds.max.x);
		float z = Random.Range(target.renderer.bounds.min.z, target.renderer.bounds.max.z);
		return new Vector3(x, target.transform.position.y, z);		
	}

	void assignCurrentZombieTarget() 
	{
		if (currentZombieTarget != null) 
		{
			foreach( GameObject zombie in GameObject.FindGameObjectsWithTag ("Zombie") )
				zombie.SendMessage("setTarget", currentZombieTarget);
		}
	}
}
