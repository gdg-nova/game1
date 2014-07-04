#pragma strict
	public var navAgent:NavMeshAgent ;
	public var currentTarget:GameObject;
	
	public var navStopDistance:float = 5;;
	
	public var runSpeed:float;
	public var baseSpeed:float;



function Start () {
		//load NavAgent
		navAgent = GetComponent(NavMeshAgent);
		
		//Pick a random target from the designated objects
		currentTarget = getRandomNavTarget ("Finish");
		
		//Go to new target
		navAgent.SetDestination(currentTarget.transform.position);

}


function Update () {
	
		//If close to nav destination, pick new random destination to keep moving
		if (navAgent.remainingDistance < navStopDistance) {
				currentTarget = getRandomNavTarget ("Finish");
				navAgent.speed = baseSpeed;
				navAgent.SetDestination (currentTarget.transform.position);
				}

		}


function getRandomNavTarget (tagName:String) {
		var targets:GameObject[]  = GameObject.FindGameObjectsWithTag (tagName);

		var  x:int = Random.Range (0, targets.Length);

		return targets [x];
	}
