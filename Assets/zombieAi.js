#pragma strict
	public var attackInterval:float;
	private var timeSinceAttack:float;
	public var attackSize:float;

	public var fearInterval:float;
	private var timeSinceFear:float;
	public var fearSize:float;

	//public var infectChance:float;
	
	public var attackDamage:float;
	public var navAgent:NavMeshAgent ;
	private var currentTarget:GameObject;
	
	
	
	public var lifeSpan:float;
function Start () {
	//load NavAgent
		navAgent = GetComponent(NavMeshAgent);
		
		//Pick a random target from the designated objects
		var g : GameObject = getRandomNavTarget ("SafeZone");		
		if (g == null) {g = getRandomNavTarget("Finish");}
		
		setTarget(g);
		
		//Go to new target
		//navAgent.SetDestination(currentTarget.transform.position);

		Destroy(gameObject, lifeSpan);
		
		castFear();
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
		
		//If close to nav destination, send damage and die
		if (navAgent.remainingDistance < 5 && currentTarget != null) {
			currentTarget.SendMessage("takeDamage", attackDamage);
			
			Destroy(gameObject);
			//currentTarget = getRandomNavTarget ("SafeZone");
			//navAgent.SetDestination (currentTarget.transform.position);
		}
}


function getRandomNavTarget (tagName:String) {
		var c : gameControl   = Camera.main.GetComponent(gameControl);
		if (c.currentZombieTarget != null) {
			return c.currentZombieTarget;
		}	else {	

			var targets: GameObject[]  = GameObject.FindGameObjectsWithTag (tagName);

			if (targets.Length > 0 ) {
				var x:int = Random.Range (0, targets.Length);
		
				return targets [x];		
			}
		
		}
		
		return null;
	}
	
function setTarget (g:GameObject) {
	currentTarget = g;
	navAgent.SetDestination(currentTarget.transform.position);

	}	
	
function attack() {
						
		var r:RaycastHit;

		//sphere cast in front of werewolf
		if (Physics.SphereCast (new Ray (transform.position, transform.forward), attackSize,  r, 3)) {		
				
				//check if hits are uhman
				if (r.collider.gameObject.tag == "Human") {		

					//if (Random.Range(0f, 1f) <= infectChance) {
					Camera.main.SendMessage("createZombie", r.collider.transform.position);
				//}
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

function goingtoSafe() {
	if (currentTarget.tag == "SafeZone") 
		{return true;}
	else 
		{return false;}
}


