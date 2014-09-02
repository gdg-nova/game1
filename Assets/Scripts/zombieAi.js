#pragma strict
	public var attackInterval:float;
	private var timeSinceAttack:float = 0;
	public var attackSize:float;

	public var fearInterval:float;
	private var timeSinceFear:float;
	public var fearSize:float;

	//public var infectChance:float;
	
	public var damage:float;
	public var navAgent:NavMeshAgent ;
	private var currentTarget:GameObject;
	
	//private var currentAnimation;

	private var animComponent : Animation;
	
	public var lifeSpan:float;
	private var timeAlive : float =  0;
	
function Start () {
	//load NavAgent
		
		//Destroy(gameObject, lifeSpan);
		
		navAgent = GetComponent(NavMeshAgent);
		animComponent = GetComponent(Animation);
		
		//Pick a random target from the designated objects
		var g : GameObject = getRandomNavTarget ("SafeZone");		
		if (g == null) {g = getRandomNavTarget("Finish");}
		
		if (g != null) {
				setTarget(g);
		}
		//Go to new target
		//navAgent.SetDestination(currentTarget.transform.position);

		castFear();
		
		animComponent.wrapMode = WrapMode.Loop;
		animComponent.Play("walk");
		
}

//function PlayAnimation(animationName) {			
//			a.wrapMode = WrapMode.Loop;
//		
//			a.Play(animationName.ToString());
//			currentAnimation = animationName;
//							
//}


function FixedUpdate () {
//	if (navAgent.velocity.magnitude >0 && currentAnimation != "walk") {
//			PlayAnimation("LM_walk");
//			
//		}
	timeAlive += Time.deltaTime;
	
	timeSinceAttack += Time.deltaTime;
	timeSinceFear += Time.deltaTime;

	if (timeAlive >= lifeSpan) {
	die();
	}
		//check if time to attack again
	if (timeSinceAttack > attackInterval) {
		
		attack();
		timeSinceAttack = 0;

		//castFeat to scare other nearby humans
		//castFear();
	} else

	if (timeSinceFear > fearInterval) {
		castFear ();
		timeSinceFear = 0;
	}

	
						
		//If close to nav destination, send damage and die
//		if (navAgent.remainingDistance < 5 && currentTarget.tag == "SafeZone") {
		//	currentTarget.SendMessage("takeDamage", attackDamage);
			
		//	Destroy(gameObject);
			//currentTarget = getRandomNavTarget ("SafeZone");
			//navAgent.SetDestination (currentTarget.transform.position);
//		}


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
	
//Destroy zombie object
function die() {
	//PlayAnimation("LM_die");
	
	navAgent.Stop();
	
	animComponent.wrapMode = WrapMode.Once;
	
	animComponent.Play("die");
	
	yield WaitForSeconds (animation["die"].length);
		
	Destroy (gameObject);
}	
	
//function attack() {
//						
//		var r:RaycastHit;
//
//		//sphere cast in front of zombie
//		if (Physics.SphereCast (new Ray (transform.position, transform.forward), attackSize,  r, 3)) {		
//				
//				//check if hits are human
//				if (r.collider.gameObject.tag == "Human") {		
//					//Destroy human
//					r.collider.gameObject.SendMessage("die");
//					//if (Random.Range(0f, 1f) <= infectChance) {
//					Camera.main.SendMessage("createZombie", r.collider.transform.position);
//				//}
//					
//				} else if (r.collider.gameObject.tag == "Guard") {		
//				//	Destroy(gameObject);
//				}
//				else if (r.collider.gameObject.tag == "SafeZone") {
//					r.collider.gameObject.SendMessage("takeDamage", attackDamage);
//					
//				}
//				
//
//		}
//	}

function attack() {
		//Debug.Log("Guard attacking!");
		
		
				
				
		var colliders : Collider[] = Physics.OverlapSphere(gameObject.transform.position, attackSize);
										
		for (var hit : Collider in colliders) {
			Debug.Log(hit.collider.gameObject);
			
			if (hit.gameObject.tag == "Human") {
				//Debug.Log("Zombie hit Human!");
				//hit.collider.gameObject.SendMessage("die");
				//engageTarget(hit.gameObject);
				//hit.collider.gameObject.SendMessage("takeDamage", damage);
				hit.gameObject.SendMessage("die");
					//if (Random.Range(0f, 1f) <= infectChance) {
					
				
				Camera.main.SendMessage("createZombie", hit.transform.position);
				
				animComponent.wrapMode = WrapMode.Once;
				animComponent.Play("L2R_swipe");
						
				animComponent.PlayQueued("walk");
				
				
				return;
			} else if (hit.gameObject.tag == "SafeZone") {
					hit.gameObject.SendMessage("takeDamage", damage);
			} else if (hit.gameObject.tag == "Guard") {
					hit.gameObject.SendMessage("takeDamage", damage);
					}		
		}
		
}

function stop() {
	navAgent.Stop();
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


