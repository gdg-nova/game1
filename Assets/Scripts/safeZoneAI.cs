using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class safeZoneAI : MonoBehaviour 
{
	public float Health = 100f;
	public int Capacity  = 50;
	public int humanCount = 0;
	
	public GameObject humanCountText;	
	
	public float coolDownSec = 30;
	private float timeSinceLastEntry = 0;

	public Material targetMaterial ;
	private Material defaultMaterial;
	public GameObject spawnArea ;
	
	void Start() 
	{ 
		defaultMaterial = this.renderer.material; 
	}
	
	void Update() 
	{
		timeSinceLastEntry += Time.deltaTime;
//		humanCountText.GetComponent(TextMesh).text = humanCount.ToString();
		
		if (timeSinceLastEntry >= coolDownSec && humanCount > 0)
			breakOpen();
	}
	
	void addHuman() 
	{
		humanCount++;
		timeSinceLastEntry = 0;
	}
	
	void takeDamage( int damage) 
	{
		Health -= damage;
		
		if (Health <= 0 ) 
		{
			breakOpen();
			Destroy (gameObject);
		}
	}

	void breakOpen() 
	{
		float x, z;

		for (int i = 0; i < humanCount; i++) 
		{
			x = Random.Range(spawnArea.renderer.bounds.min.x, spawnArea.renderer.bounds.max.x);
			z = Random.Range(spawnArea.renderer.bounds.min.z, spawnArea.renderer.bounds.max.z);
			//Camera.main.SendMessage("createHuman", transform.position);
			Camera.main.SendMessage("createZombie", new Vector3(x, 1, z));
		}
		humanCount = 0;
	}
	
	void infected() 
	{
		Debug.Log ( "Safe-zone infected" );
		breakOpen();
		Destroy (gameObject);
	}
	
	void makeZombieTarget() 
	{
	}
	
	void removeZombieTarget() 
	{
	}
}
