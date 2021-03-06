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

	public float zombieMultiplier = 1;

	public GameObject summonZombieEffect;



	void Start()
	{
	}

	void Update() 
	{
		timeSinceLastSpawn += Time.deltaTime;

		if (ZombieSpawnMode == spawnMode.Automatic) CreateZombie();
	}

	void Click() {
		if (ZombieSpawnMode == spawnMode.Onclick) {
			CreateZombie();
				}
	}

	// this is public so can be invoked by click on the graveyard
	public void CreateZombie() 
	{
		// if not enough time passed, don't generate a new one
		if (timeSinceLastSpawn < zombieSpawnInterval )
			return;

		timeSinceLastSpawn = 0;


		Quaternion lightningAngle = Quaternion.Euler (-90, 0, 0);

		if (globalEvents.manaControllerService.CanIBuyAZombie()) {

			// time to generate a new zombie
			float x = Random.Range (gameObject.GetComponent<Collider>().bounds.min.x, gameObject.GetComponent<Collider>().bounds.max.x);
			float z = Random.Range (gameObject.GetComponent<Collider>().bounds.min.z, gameObject.GetComponent<Collider>().bounds.max.z);

			Vector3 spawnLocation = new Vector3 (x, transform.position.y, z);
			Quaternion r = Quaternion.Euler(0,0,0);
			zombieAI zombie = globalEvents.manaControllerService.RequestBuyZombie (spawnLocation, r).GetComponent<zombieAI>();
			if (zombie != null) {
				GameObject lightning = (GameObject)GameObject.Instantiate (summonZombieEffect, spawnLocation, lightningAngle);

				Destroy (lightning, 2f);
			}

			//if multiplier is set, spawn additional free zombies
			for (int i = 1; i <= zombieMultiplier; i++) {
				float new_x = Random.Range (gameObject.GetComponent<Collider>().bounds.min.x, gameObject.GetComponent<Collider>().bounds.max.x);
				float new_z = Random.Range (gameObject.GetComponent<Collider>().bounds.min.z, gameObject.GetComponent<Collider>().bounds.max.z);

				Vector3 new_spawnLocation = new Vector3 (new_x, transform.position.y, new_z);

				globalEvents.characterCreator.createZombie (new_spawnLocation, r);
				GameObject new_lightning = (GameObject)GameObject.Instantiate (summonZombieEffect, new_spawnLocation, lightningAngle);
						
				Destroy (new_lightning, 2f);
			}
		}
		timeSinceLastSpawn = 0;

	}
}

