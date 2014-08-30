#pragma strict
	public var navAgent: NavMeshAgent ;
	private var currentTarget:GameObject ;

	public var runSpeed:float ;
	public var speedVariation:float = .2;
	
	
	public var fearMaterial:Material;
	
	
	private var baseSpeed:float ;

	//private var goingtoSafe:boolean  = false;

	public var stayInSafe:boolean ;
	private var isAfraid : boolean = false;
	
	private var currentAnimation;
	
	
function Start () {
	//set Random rotation for visual interest
		transform.rotation = getRandomRotation ();

		//load NavAgent
		navAgent = GetComponent("NavMeshAgent");

		var speedVar : float = navAgent.speed * speedVariation;
		
		//Debug.Log(speedVar);
		
		navAgent.speed = Random.Range(navAgent.speed - speedVar, navAgent.speed + speedVar);
		
		
		//store base navagent speed in variable
		baseSpeed = navAgent.speed;

		var g : GameObject = getRandomNavTarget ("SafeZone");		
		if (g == null) {g = getRandomNavTarget("Finish");}
		
		setTarget(g);
		
		
}

function PlayAnimation(animationName) {
			var a : Animation = GetComponent("Animation");
			a.wrapMode = WrapMode.Loop;
		
			a.Play(animationName.ToString());
			currentAnimation = animationName;

}

function FixedUpdate () {
	if (navAgent.velocity.magnitude >0 && currentAnimation != "walk") {
			PlayAnimation("walk");
			
		}	
	
	//check if target destination has been destroyed
	if (currentTarget == null) {	
			var t : GameObject = getRandomNavTarget ("SafeZone");
			if (t == null) {t = getRandomNavTarget("Finish");}
			
			if (t != null) setTarget(t);
			
			navAgent.speed = baseSpeed;
			//navAgent.SetDestination (currentTarget.transform.position);	
	} else {

		if (navAgent.remainingDistance < 5) {
					if (goingtoSafe() && isAfraid && currentTarget != null) {
						Debug.Log(goingtoSafe);
						enterSafeZone();
					}else {
					
					navAgent.speed = baseSpeed;
					
					var g : GameObject = getRandomNavTarget ("SafeZone");		
					if (g == null) {g = getRandomNavTarget("Finish");}
		
					setTarget(g);
				}
			
			}		
		}
}



function getRandomRotation() {
		var y:int = Random.Range (0, 360);		
		var q:Quaternion = Quaternion.Euler (0, y, 0);		
		return q;
		
	}
	
		
function setTarget (g:GameObject) {
	currentTarget = g;
	navAgent.SetDestination(currentTarget.transform.position);
	
	}	
	
	
function getRandomNavTarget(tagName:String ) {
	var targets: GameObject[]  = GameObject.FindGameObjectsWithTag (tagName);

	if (targets.Length > 0 ) {
		var x:int = Random.Range (0, targets.Length);
	
		return targets [x];
	}
	return null;
}

//Destroy human object
function die() {
		Destroy (gameObject);
		}
		
//Sprint to random safe zone
function Afraid() {
	isAfraid = true;

	//navAgent.Stop ();
	navAgent.speed = runSpeed;
	//navAgent.SetDestination (new Vector3 (0, 0, 0));
	//goingtoSafe = true;
	
	//gameObject.renderer.material = fearMaterial;
	
	var g : GameObject = getRandomNavTarget ("SafeZone");		
	if (g == null) {g = getRandomNavTarget("Finish");}
		
	if (navAgent != null && g != null)  setTarget(g);
		
	
	
}


function goingtoSafe() {
	if (currentTarget.tag == "SafeZone") 
		{return true;}
	else 
		{return false;}
}
	
function enterSafeZone() {
	currentTarget.SendMessage("addHuman");
	
	die();
}
