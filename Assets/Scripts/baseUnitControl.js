public var health : float = 100;
public var healthDrainRate : float = 20;

public
function Update () { 
	takeDamage (healthDrainRate * Time.deltaTime);
}


function takeDamage(damage:int) {
	health -= damage;
	
	if (health <= 0 ) {
		Destroy (gameObject);
	}
}
