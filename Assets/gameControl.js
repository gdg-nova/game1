#pragma strict

	public var Human:GameObject;
	public var Zombie:GameObject;
	public var Werewolf:GameObject;
	//public GameObject spawn;

	public var humanCount:int;

	private var q :Quaternion= Quaternion.Euler (0,0,0);


function Start () {
loadHumans ();
}

function Update () {
		//check for mouse input
			if (Input.GetMouseButton(0)) {
					clickObject();
				}

		if (Input.GetMouseButton(1)) {
			rightclickObject();
		}
}

//update human/zombie counter
	function FixedUpdate() {
		Debug.Log ("Humans: " + GameObject.FindGameObjectsWithTag ("Human").Length);
		Debug.Log ("Zombies: " + GameObject.FindGameObjectsWithTag ("Zombie").Length);
	}
	
//mouse click handler
	function clickObject() {
		//send raycast to get hit
		var r : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		var r_hit : RaycastHit;

		if (Physics.Raycast (r,  r_hit, Mathf.Infinity)) {
			//Check if human
			if (r_hit.collider.gameObject.tag == "Human") {
				//Destroy human, create zombie
				createZombie(r_hit.collider.gameObject.transform.position);
				Destroy(r_hit.collider.gameObject);
			}
		}

	}

	function rightclickObject() {
		//send raycast to get hit
		var r : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		var r_hit : RaycastHit;
		
		if (Physics.Raycast (r,  r_hit, Mathf.Infinity)) {
			//Check if human
			if (r_hit.collider.gameObject.tag == "Human") {
				//Destroy human, create zombie
				createWerewolf(r_hit.collider.gameObject.transform.position);
				Destroy(r_hit.collider.gameObject);
			}
		}
	}
	
	//Create humans per HumanCount
	function loadHumans() {
	
		for (var i = 0; i<humanCount; i++) {

			var spawn:GameObject = getRandomSpawn();

			//Find random position within Spawn plane
			var x :float= Random.Range(spawn.renderer.bounds.min.x, spawn.renderer.bounds.max.x);
			var z:float= Random.Range(spawn.renderer.bounds.min.z, spawn.renderer.bounds.max.z);

			Instantiate(Human, new Vector3(x, .5f, z), q);
		}
	}

	function getRandomSpawn() {
		var spawns : GameObject[] = GameObject.FindGameObjectsWithTag ("Respawn");

		return spawns [Random.Range (0, spawns.Length)];
	}

	//Create zombie at position
	function createZombie( position) {
		Instantiate (Zombie, position, q);
	}

	function createWerewolf( position) {
		Instantiate (Werewolf, position, q);
	}



