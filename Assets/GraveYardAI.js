#pragma strict

public var zombieSpawnInterval : float = 1;
private var timeSinceZombieSpawn : float;

function Start () {

}

function Update () {
	timeSinceZombieSpawn += Time.deltaTime;
	
	
}

function CreateZombie () {
	if (timeSinceZombieSpawn >= zombieSpawnInterval){

	var x :float= Random.Range(gameObject.collider.bounds.min.x, gameObject.collider.bounds.max.x);
	var z:float= Random.Range(gameObject.collider.bounds.min.z, gameObject.collider.bounds.max.z);
	
	Camera.main.SendMessage("createZombie",  (new Vector3(x, transform.position.y, z)));	
	
	timeSinceZombieSpawn = 0;
	}
}