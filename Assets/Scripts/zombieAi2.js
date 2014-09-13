﻿#pragma strict
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
	
	public var health : float = 100;
	public var healthDrainRate : float = 20;

		
	//private var timeAlive : float =  0;
	
function Start () {
	//load NavAgent
		
		//Destroy(gameObject, lifeSpan);
		
		navAgent = GetComponent(NavMeshAgent);
		animComponent = GetComponent(Animation);
//		
//		//Pick a random target from the designated objects
//		var g : GameObject = getRandomNavTarget ("SafeZone");		
//		if (g == null) {g = getRandomNavTarget("Finish");}
//		
//		if (g != null) {
//				setTarget(g);
//		}
		//Go to new target
		//navAgent.SetDestination(currentTarget.transform.position);

	//	castFear();
		
		//animComponent.wrapMode = WrapMode.Loop;
		//animComponent.Play("walk");
		
		
		//Check if mainCamera has currentZombie target
		var currentZombieTarget : GameObject  =getCurrentAssignedZombieTarget();
		
		if (currentZombieTarget != null) {
			setTarget(currentZombieTarget);
		} else {
			var playableArea : GameObject =  GameObject.FindGameObjectWithTag("Playable");
			
			var randomPoint : Vector3 = getRandomLocationinBounds(playableArea);
			
			//Debug.Log("Zombie starting random point:" + randomPoint);
			
			setTargetVector(randomPoint);
			
		}
		
		//If not, pick a destination, walk slowly towards it
		
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
	
	timeSinceAttack += Time.deltaTime;
	timeSinceFear += Time.deltaTime;
	
//	if (health <= 0) {
//	die();
//	}
		//check if time to attack again
	if (timeSinceAttack > attackInterval) {
		
		attack();
		timeSinceAttack = 0;

		//castFeat to scare other nearby humans
		//castFear();
	} else {

	if (timeSinceFear > fearInterval) {
		castFear ();
		timeSinceFear = 0;
		}
	}
}

function getCurrentAssignedZombieTarget() {
	var c : gameControl   = Camera.main.GetComponent(gameControl);
		return c.currentZombieTarget;
		
}

function getRandomNavTarget (tagName:String) {
		//var c : gameControl   = Camera.main.GetComponent(gameControl);
		
		if (getCurrentAssignedZombieTarget != null) {
			return getCurrentAssignedZombieTarget;
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
	
//Destroy zombie object
function die() {
	//PlayAnimation("LM_die");
	
	if (navAgent != null)  navAgent.Stop();
	gameObject.GetComponent(CapsuleCollider).enabled = false;
	
	animComponent.wrapMode = WrapMode.Once;
	
	animComponent.Play("die");
	
	yield WaitForSeconds (animation["die"].length *2);
		
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
				
				//animComponent.wrapMode = WrapMode.Loop;
				
				//health -= healthCostperAttack;
				return;
			} else if (hit.gameObject.tag == "SafeZone") {
			
					//hit.gameObject.SendMessage("takeDamage", damage);
					Debug.Log("hit house");
					hit.gameObject.SendMessage("infected", damage);
					
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
			Debug.Log("Fear hit:"  + r.collider.gameObject.tag);
			
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

function getRandomLocationinBounds(target : GameObject) {
		var x :float= Random.Range(target.renderer.bounds.min.x, target.renderer.bounds.max.x);
		var z:float= Random.Range(target.renderer.bounds.min.z, target.renderer.bounds.max.z);
		
		return new Vector3(x, target.transform.position.y, z);
		
}

function takeDamage(damage:int) {
	health -= damage;
	
	if (health <= 0 ) {
		die();
	}
}

