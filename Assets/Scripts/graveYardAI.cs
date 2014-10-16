using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class graveYardAI : MonoBehaviour 
{
	public enum spawnMode  {Automatic, Onclick};

	public spawnMode ZombieSpawnMode = spawnMode.Onclick;

	// wait 1 second before allowing automatic zombie spawning...
	public float zombieSpawnInterval = 1.0f;
	private float timeSinceLastSpawn;

	gameControl gameController = null;

	public GameObject summonZombieEffect;



	void Start()
	{
		GameObject controller = GameObject.FindWithTag("GameController");
		gameController = controller.GetComponent<gameControl>();
	}

	void Update() 
	{
		if (ZombieSpawnMode == spawnMode.Automatic) CreateZombie();
	}

	void Click() {
		if (ZombieSpawnMode == spawnMode.Onclick) {
			CreateZombie();
				}
	}

	// this is public so can be invoked by click on the graveyard
	void CreateZombie() 
	{
		Debug.Log ("Createzombie");
		timeSinceLastSpawn += Time.deltaTime;

		// if not enough time passed, don't generate a new one
		if (timeSinceLastSpawn < zombieSpawnInterval )
			return;

		timeSinceLastSpawn = 0;

		// time to generate a new zombie
		float x = Random.Range(gameObject.collider.bounds.min.x, gameObject.collider.bounds.max.x);
		float z = Random.Range(gameObject.collider.bounds.min.z, gameObject.collider.bounds.max.z);

		gameController.requestBuyNewZombie(new Vector3(x, transform.position.y, z));	
			
		timeSinceLastSpawn = 0;
	}
}

