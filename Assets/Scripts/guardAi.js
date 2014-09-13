#pragma strict
	public var attackInterval:float;
	private var timeSinceAttack:float;
	public var attackSize:float;

	public var stationary:boolean;
		
	//public var attackDamage:float;
	public var navAgent:NavMeshAgent ;
	private var currentTarget:GameObject;
	
	
	
	public var damage:float;
	public var lifeSpan:float;
	
	private var animComponent : Animation;

function Start () {
	//load NavAgent
		navAgent = GetComponent(NavMeshAgent);
		animComponent = GetComponent(Animation);

		animComponent.wrapMode = WrapMode.Loop;
		
		animComponent.Play("idle");
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
		//Debug.Log(timeSinceAttack);
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
		
	setTargetVector(currentTarget.transform.position);
	
	
	animComponent["walk"].speed = 1;	
	navAgent.speed = 3.5;
	}	
	
function setTargetVector(target : Vector3) {

	navAgent.SetDestination(target);
	
	navAgent.speed = 1;
	
	animComponent["walk"].speed = .4;	
	
	animComponent.wrapMode = WrapMode.Loop;
	animComponent.Play("walk");
}
	
function attack() {
		//Debug.Log("Guard attacking!");

		var colliders : Collider[] = Physics.OverlapSphere(gameObject.transform.position, attackSize);

		//only swipe if Zombie hit
		if (colliders.Length >0 ) {
				
				//animComponent.wrapMode = WrapMode.Loop;
		}										
					
		var playedSwipeAnim : boolean = false;
		
		var hitZombies : ArrayList = new ArrayList();
																																																											
																																																																																																																																																																							
		for (var hit : Collider in colliders) {
			Debug.Log(hit.collider.gameObject);
						
			if (hit.gameObject.tag == "Zombie" && (hitZombies.Contains(hit.gameObject) == false)) {			
				Debug.Log("Guard hit zombie!");
				hitZombies.Add(hit.gameObject);
				
				if (!playedSwipeAnim ) {
					animComponent.wrapMode = WrapMode.Once;
					animComponent.Play("R2L_swipe");
						
					animComponent.PlayQueued("walk");
					playedSwipeAnim = true;
				}
				//hit.collider.gameObject.SendMessage("die");
				engageTarget(hit.gameObject);
				
				return;
			}		
		}
	}

function engageTarget(target  : GameObject) {
	target.SendMessage("stop");
	//navAgent.SetDestination(target.transform.position);
	
	transform.LookAt(target.transform);
	target.SendMessage("takeDamage", damage);
	//transform.position  = Vector3.Slerp(transform.position, g.transform.position, 10f ); 
		
	
}



function goingtoSafe() {
	if (currentTarget.tag == "SafeZone") 
		{return true;}
	else 
		{return false;}
}


