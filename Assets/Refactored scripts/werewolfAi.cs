using UnityEngine;
using System.Collections;

public class werewolfAi : MonoBehaviour {
	public float attackInterval;
	float timeSinceAttack;
	public float attackSize;

	
	public float fearInterval;
	float timeSinceFear;
	public float fearSize;

	public float infectChance;

	public NavMeshAgent navAgent;
	GameObject currentTarget;

	// Use this for initialization
	void Start () {
		
		//load NavAgent
		navAgent = (NavMeshAgent)GetComponent("NavMeshAgent");
		
		//Pick a random target from the designated objects
		currentTarget = getRandomNavTarget ();
		
		//Go to new target
		navAgent.SetDestination(currentTarget.transform.position);

	}
	
	// Update is called once per frame
	void Update () {

		//update attack timer
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
			currentTarget = getRandomNavTarget ();
			navAgent.SetDestination (currentTarget.transform.position);
		}
		
	}



	
	//return randomly selected nav target
	GameObject getRandomNavTarget() {
		GameObject[] targets = GameObject.FindGameObjectsWithTag ("Finish");
		
		int x = Random.Range (0, targets.Length);
		
		return targets [x];
	}


	//attack with spherecast
	void attack() {
						
		RaycastHit r;

		//sphere cast in front of werewolf
		if (Physics.SphereCast (new Ray (transform.position, transform.forward), attackSize, out r, 3)) {		
				
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
	void castFear() {

		//get all hits in range
		foreach (RaycastHit r in  Physics.SphereCastAll (new Ray (transform.position, transform.forward), fearSize, fearSize)) {

			//check if hits are uhman
			if (r.collider.gameObject.tag == "Human") {

				//tell human to run
				r.collider.gameObject.SendMessage("Afraid");
			}
				
		}
	}
}
