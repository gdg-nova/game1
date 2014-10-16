using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

	public GameObject HealthTextUI;
	public GameObject HealthBarUI;


	private float HealthBarFullWidth;

	// when a building gets infected with zombies from attacking,
	// break down the building and scatter the humans inside.
	// SOME of the humans will be infected, so don't make EVERYONE
	// come out as zombies, but only SOME, and have the rest of the
	// humans created start in a "sprint" state to get away.
	public float BreakOutPctZombies = 3.0f;
	
	void Start() 
	{ 
		HealthBarFullWidth = HealthBarUI.transform.localScale.x;

	//	defaultMaterial = this.renderer.material; 
	}
	
	void Update() 
	{
		//timeSinceLastEntry += Time.deltaTime;
//		humanCountText.GetComponent(TextMesh).text = humanCount.ToString();
		
		//if (timeSinceLastEntry >= coolDownSec && humanCount > 0)
			//breakOpen();

		HealthTextUI.GetComponent<Text> ().text = Health.ToString ();
		HealthBarUI.transform.localScale = new Vector3 ((Health / 100) * HealthBarFullWidth, HealthBarUI.transform.localScale.y, HealthBarUI.transform.localScale.z);

	}
	
	void addHuman() 
	{
		humanCount++;
		timeSinceLastEntry = 0;
	}
	
	public void takeDamage( float damage) 
	{
		Debug.Log ("Safezone damage");

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
		
		// get gamecontrol object from main camera
		GameObject go = GameObject.FindWithTag("GameController"); 
		gameControl gc = go.GetComponent<gameControl>();
		// if not actually found, get out
		if( gc == null )
			return;
		
		
		// how many zombies should we create of the total human count...
		// take the integer count... PERCENTS, so divide by 100.
		int makeZombies = (int)(humanCount * BreakOutPctZombies / 100.0f );
		
		for (int i = 0; i < humanCount; i++) 
		{
			x = Random.Range(spawnArea.renderer.bounds.min.x, spawnArea.renderer.bounds.max.x);
			z = Random.Range(spawnArea.renderer.bounds.min.z, spawnArea.renderer.bounds.max.z);
			//Camera.main.SendMessage("createHuman", transform.position);
			if( i < makeZombies )
			{
				gc.createZombie( new Vector3(x, 1, z));
			}
			else
			{
				humanAI hAI = gc.createHuman( new Vector3(x, 1, z));
				if( hAI != null )
					hAI.Afraid();
			}
			
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
