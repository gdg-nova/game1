#pragma strict
	public var attackInterval:float;
	private var timeSinceAttack:float;
	public var attackSize:float;

	public var stationary:boolean;
		
	//public var attackDamage:float;
	public var navAgent:NavMeshAgent ;
	private var currentTarget:GameObject;
	
	
	
	public var lifeSpan:float;
function Start () {
	//load NavAgent
		navAgent = GetComponent(NavMeshAgent);
		
		if (!stationary) {
		//Pick a random target from the designated objects
		var g : GameObject = getRandomNavTarget ("SafeZone");		
		if (g == null) {g = getRandomNavTarget("Finish");}
		
		setTarget(g);
		}
		//Go to new target
		//navAgent.SetDestination(currentTarget.transform.position);

		
}

function Update () {
	timeSinceAttack += Time.deltaTime;

		//check if time to attack again
		if (timeSinceAttack > attackInterval) {
			
			attack();
			timeSinceAttack = 0;

		}
		
		//If close to nav destination, send damage and die
		if (!stationary && navAgent.remainingDistance < 5 && currentTarget != null) {
			var g :GameObject  = getRandomNavTarget ("SafeZone");

			setTarget(g);
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
				if (r.collider.gameObject.tag == "Zombie") {		

					//Destroy human
					r.collider.gameObject.SendMessage("die");
				}
		}
	}




function goingtoSafe() {
	if (currentTarget.tag == "SafeZone") 
		{return true;}
	else 
		{return false;}
}


