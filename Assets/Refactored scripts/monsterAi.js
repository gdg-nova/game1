#pragma strict

	public var attackInterval:float;
	public var timeSinceAttack:float;
	public var attackSize:float;

	public var fearInterval:float;
	private var timeSinceFear:float;
	public var fearSize:float;

	public var lifeSpan:float;

	public var infectChance:float;
	
	
	

function Start () {
		
		//Set timer to destroy monster after lifespan expires
		if (lifeSpan > 0) {Destroy (gameObject, lifeSpan);}
}

function Update () {
		//update attack timer
		timeSinceAttack += Time.deltaTime;


		//check if time to attack again
		if (timeSinceAttack > attackInterval) {
			
			//attack();
			timeSinceAttack = 0;

			//castFeat to scare other nearby humans
			//castFear();
		}

		timeSinceFear += Time.deltaTime;
		if (timeSinceFear > fearInterval) {
//			castFear ();
			timeSinceFear = 0;
		}
}