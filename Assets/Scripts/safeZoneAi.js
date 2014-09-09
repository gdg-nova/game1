
public var Health:float = 100f;
public var Capacity:int = 50;
public var humanCount:int =0;

public var humanCountText:GameObject;	

public var coolDownSec:float = 30;
private var timeSinceLastEntry : float = 0;

//private var targetTime : float;
//private var timeSinceTargetStart : float = 0;
			
public var targetMaterial : Material;
		
private var defaultMaterial:Material;

public var spawnArea : GameObject;

	
function Start () {
	defaultMaterial = this.renderer.material;
}

function Update () {
	timeSinceLastEntry += Time.deltaTime;
	humanCountText.GetComponent(TextMesh).text = humanCount.ToString();


	if (timeSinceLastEntry >= coolDownSec && humanCount > 0) {
		breakOpen();
	}
}

function addHuman() {
	humanCount +=1;
	timeSinceLastEntry = 0;
	
	//humanCountText.SendMessage("updateValue", humanCount);
	
	
}

function takeDamage(damage:int) {
	Health -= damage;
	
	if (Health <= 0 ) {
		breakOpen();
		Destroy (gameObject);
	}
}

function breakOpen() {
	
	for (var i:int = 0; i < humanCount; i++) {
		var x :float= Random.Range(spawnArea.renderer.bounds.min.x, spawnArea.renderer.bounds.max.x);
		var z:float= Random.Range(spawnArea.renderer.bounds.min.z, spawnArea.renderer.bounds.max.z);
			//Camera.main.SendMessage("createHuman", transform.position);
			Camera.main.SendMessage("createZombie", new Vector3(x, 1, z));
			
	}
	humanCount = 0;
		
}

function infected() {
	Debug.Log("House infected");
	

	breakOpen();
	Destroy (gameObject);

}

function makeZombieTarget() {
	//targetTime = time;
	//GetComponent("Halo").active = true;
}

function removeZombieTarget() {
	//GetComponent("Halo").active = false;
}
