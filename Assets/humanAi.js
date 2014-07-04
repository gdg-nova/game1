#pragma strict
	public var navAgent: NavMeshAgent ;
	private var currentTarget:GameObject ;

	public var runSpeed:float ;
	private var baseSpeed:float ;

	private var goingtoSafe:boolean  = false;

	public var stayInSafe:boolean ;
	
	
function Start () {
	//set Random rotation for visual interest
		transform.rotation = getRandomRotation ();

		//load NavAgent
		navAgent = GetComponent("NavMeshAgent");

		//Pick a random target from the designated objects
		currentTarget = getRandomNavTarget ("Finish");

		//store base navagent speed in variable
		baseSpeed = navAgent.speed;

		//Go to new target
		navAgent.SetDestination(currentTarget.transform.position);
}

function Update () {
	if (navAgent.remainingDistance < 5) {
			if (goingtoSafe && stayInSafe) {
				}else {
				currentTarget = getRandomNavTarget ("Finish");
				navAgent.speed = baseSpeed;
				navAgent.SetDestination (currentTarget.transform.position);
			}
		}
}

function getRandomRotation() {
		var y:int = Random.Range (0, 360);		
		var q:Quaternion = Quaternion.Euler (0, y, 0);		
		return q;
		
	}
	
function getRandomNavTarget(tagName:String ) {
	var targets: GameObject[]  = GameObject.FindGameObjectsWithTag (tagName);

	var x:int = Random.Range (0, targets.Length);

	return targets [x];
}

//Destroy human object
function die() {
		Destroy (gameObject);
		}
		
//Sprint to random safe zone
function Afraid() {

	navAgent.Stop ();
	navAgent.speed = runSpeed;
	//navAgent.SetDestination (new Vector3 (0, 0, 0));
	goingtoSafe = true;
	navAgent.SetDestination (getRandomNavTarget ("SafeZone").transform.position);
}
