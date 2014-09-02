#pragma strict
	public var attackInterval:float;
	private var timeSinceAttack:float;
	public var attackSize:float;

	public var fearInterval:float;
	private var timeSinceFear:float;
	public var fearSize:float;

	public var infectChance:float;
	
	public var navAgent:NavMeshAgent ;
	private var currentTarget:GameObject;
	
	
function Start () {
	//load NavAgent
		navAgent = GetComponent(NavMeshAgent);
		
		//Pick a random target from the designated objects
		currentTarget = getRandomNavTarget ("Finish");
		
		//Go to new target
		navAgent.SetDestination(currentTarget.transform.position);

}

function Update () {
		timeSinceAttack += Time.deltaTime;

		//check if time to attack again
		if (timeSinceAttack > attackInterval) {
			
			attack();
			timeSinceAttack = 0;

			//castFeat to scare other nearby humans
			//castFear();
		}

		timeSinceFear += Time.deltaTime;
		if (timeSinceFear > fearInterval) {
			castFear ();
			timeSinceFear = 0;
		}
		
		//If close to nav destination, pick new random destination to keep moving
		if (navAgent.remainingDistance < 5) {
			currentTarget = getRandomNavTarget ("Finish");
			navAgent.SetDestination (currentTarget.transform.position);
		}
}


function getRandomNavTarget (tagName:String) {
		var targets:GameObject[]  = GameObject.FindGameObjectsWithTag (tagName);

		var  x:int = Random.Range (0, targets.Length);

		return targets [x];
	}
	
	//attack with spherecast
	function attack() {
						
		var r:RaycastHit;

		//sphere cast in front of werewolf
		if (Physics.SphereCast (new Ray (transform.position, transform.forward), attackSize,  r, 3)) {		
				
				//check if hits are uhman
				if (r.collider.gameObject.tag == "Human") {		

					if (Random.Range(0f, 1f) <= infectChance) {
					Camera.main.SendMessage("createWerewolf", r.collider.transform.position);
				}
					//Destroy human
					r.collider.gameObject.SendMessage("die");
				}
		}
	}

	//send spherecast to find humans to scare
	function castFear() {

		//get all hits in range
		for (var r:RaycastHit  in  Physics.SphereCastAll (new Ray (transform.position, transform.forward), fearSize, fearSize)) {

			//check if hits are uhman
			if (r.collider.gameObject.tag == "Human") {

				//tell human to run
				r.collider.gameObject.SendMessage("Afraid");
			}
				
		}
	}
