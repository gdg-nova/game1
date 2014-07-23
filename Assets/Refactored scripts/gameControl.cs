using UnityEngine;
using System.Collections;

public class gameControl : MonoBehaviour {
	public GameObject Human;
	public GameObject Zombie;
	public GameObject Werewolf;
	//public GameObject spawn;

	public int humanCount;

	Quaternion q = Quaternion.Euler (0,0,0);

	// Use this for initialization
	void Start () {
		loadHumans ();
	}
	
	// Update is called once per frame
	void Update () {
			//check for mouse input
			if (Input.GetMouseButton(0)) {
					clickObject();
				}

		if (Input.GetMouseButton(1)) {
			Debug.Log ("z");

			rightclickObject();
		}
	}

	//update human/zombie counter
	void FixedUpdate() {
		Debug.Log ("Humans: " + GameObject.FindGameObjectsWithTag ("Human").Length);
		Debug.Log ("Zombies: " + GameObject.FindGameObjectsWithTag ("Zombie").Length);
	}

	//mouse click handler
	void clickObject() {
		//send raycast to get hit
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;

		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) {
			//Check if human
			if (r_hit.collider.gameObject.tag == "Human") {
				//Destroy human, create zombie
				createZombie(r_hit.collider.gameObject.transform.position);
				Destroy(r_hit.collider.gameObject);
			}
		}

	}

	void rightclickObject() {
		//send raycast to get hit
		/*
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit r_hit;
		
		if (Physics.Raycast (r, out r_hit, Mathf.Infinity)) {
			//Check if human
			if (r_hit.collider.gameObject.tag == "Human") {
				//Destroy human, create zombie
				createWerewolf(r_hit.collider.gameObject.transform.position);
				Destroy(r_hit.collider.gameObject);
			}
		}*/

		GameObject[] zombies = GameObject.FindGameObjectsWithTag ("Zombie");

		foreach (GameObject z in zombies) {
			z.SendMessage("setDestination", Camera.main.ScreenToWorldPoint(Input.mousePosition));
				}
	}

	//Create humans per HumanCount
	void loadHumans() {
	
		for (int i = 0; i<humanCount; i++) {

			GameObject spawn = getRandomSpawn();

			//Find random position within Spawn plane
			float x= Random.Range(spawn.renderer.bounds.min.x, spawn.renderer.bounds.max.x);
			float z= Random.Range(spawn.renderer.bounds.min.z, spawn.renderer.bounds.max.z);

			Instantiate(Human, new Vector3(x, .5f, z), q);
		}
	}

	GameObject getRandomSpawn() {
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");

		return spawns [Random.Range (0, spawns.Length)];
	}

	//Create zombie at position
	void createZombie(Vector3 position) {
		Instantiate (Zombie, position, q);
	}

	void createWerewolf(Vector3 position) {
		Instantiate (Werewolf, position, q);
	}
}
