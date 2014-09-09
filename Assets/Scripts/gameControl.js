#pragma strict
	public var Human:GameObject;
	public var Zombie:GameObject;
	public var Werewolf:GameObject;
	
	public var humanCount:int;
	
	public var timeLimit : float = 30;
	public var elapsedTime : float = 0; 

	private var q :Quaternion= Quaternion.Euler (0,0,0);

	public var currentZombieTarget : GameObject;
	
	public var gameEnded:boolean;

	//public var zombieUIStyle : GUIStyle;
	
	
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
		
		elapsedTime += Time.deltaTime;
		
		
		if (elapsedTime >= timeLimit) {
			gameOver();			
		}
		
		checkForWin();
}

function gameOver() {
		print("GameOver!");
		
		Time.timeScale = 0;
		gameEnded = true;
}

//update human/zombie counter
function FixedUpdate() {
	//Debug.Log ("Humans: " + GameObject.FindGameObjectsWithTag ("Human").Length);
	//Debug.Log ("Zombies: " + GameObject.FindGameObjectsWithTag ("Zombie").Length);
}


			
//mouse click handler
function clickObject() {
	//send raycast to get hit
	var r : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	var r_hit : RaycastHit;

	if (Physics.Raycast (r,  r_hit, Mathf.Infinity)) {
		//Check if human
		
		Debug.Log(r_hit.collider.gameObject.tag);
		
		var g : GameObject = r_hit.collider.gameObject;
		
		if (g.tag == "Human") {
			//Destroy human, create zombie
			createZombie(g.transform.position);
			Destroy(g);
		} else if (g.tag == "SafeZone") {
		
			if (currentZombieTarget != null) {
				currentZombieTarget.SendMessage("removeZombieTarget");
				
			}
			g.SendMessage("makeZombieTarget");
			currentZombieTarget = g;
			
			for (var z : GameObject in	GameObject.FindGameObjectsWithTag("Zombie")) {
				z.SendMessage("setTarget", g);
			}
		} else if (g.tag == "Graveyard") {
			g.SendMessage("CreateZombie");
		}
		
	}

}

function rightclickObject() {

//var zombies:GameObject[]  = GameObject.FindGameObjectsWithTag ("Zombie");
/*
	for ( var z:GameObject in zombies) {
	Debug.Log(z);
		z.SendMessage("setDestination", Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
*/				
			
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
function createZombie( position:Vector3) {
	Instantiate (Zombie, position, q);
}

function createWerewolf( position:Vector3) {
	Instantiate (Werewolf, position, q);
}

function createHuman( position:Vector3) {
		Instantiate (Human, position, q);

}

function getHumansAliveCount() {
	var baseCount : int = GameObject.FindGameObjectsWithTag ("Human").Length;
	
	var safeZones : GameObject[] = GameObject.FindGameObjectsWithTag("SafeZone");
	
	for (var zone : GameObject in safeZones) {
		var safeAI : safeZoneAi = zone.GetComponent ("safeZoneAi") as safeZoneAi;
		
		Debug.Log(safeAI);
			
		baseCount = baseCount + safeAI.humanCount;
	}
	
	return baseCount;
}

function checkForWin() {
	if (getHumansAliveCount() == 0 ) {
		gameOver();
	}
}

function getRandomLocationinBounds(target : GameObject) {
		var x :float= Random.Range(target.renderer.bounds.min.x, target.renderer.bounds.max.x);
		var z:float= Random.Range(target.renderer.bounds.min.z, target.renderer.bounds.max.z);
		
		return new Vector3(x, target.transform.position.y, z);
		
}

