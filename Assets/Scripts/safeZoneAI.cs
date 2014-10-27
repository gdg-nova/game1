using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class safeZoneAI : MonoBehaviour 
{
	public float Health = 100f;

	private float initialHealth;

	public int Capacity  = 50;
	public int humanCount = 0;
	
	public GameObject humanCountText;	
	
	public float coolDownSec = 30;
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
		initialHealth = Health;
		if (HealthBarUI != null)
			HealthBarFullWidth = HealthBarUI.transform.localScale.x;

		//	defaultMaterial = this.renderer.material; 
	}
	
	void Update() 
	{
		//timeSinceLastEntry += Time.deltaTime;
//		humanCountText.GetComponent(TextMesh).text = humanCount.ToString();
		
		//if (timeSinceLastEntry >= coolDownSec && humanCount > 0)
			//breakOpen();

		if (HealthTextUI != null)
			HealthTextUI.GetComponent<Text> ().text = Health.ToString ();

		if (HealthBarUI != null)
			HealthBarUI.transform.localScale = 
				new Vector3 ((Health / initialHealth) * HealthBarFullWidth, 
				             HealthBarUI.transform.localScale.y, 
				             HealthBarUI.transform.localScale.z);
	
		exitAsGuard();
	}

	// DVR, new feature... when a human enters a building, typically just add to the
	// counter and done.  However, new request is that a human comed in for a slight
	// delay, then gets returned outside as a knight, or advanced human to attack zombies.
	int pendingGuards = 0;
	float newGuardDelay = 0.0f;


	void addHuman() 
	{
		humanCount++;
		// Now, only if random.. do we want a human to return out, but only after
		// some determined time, such as 2 seconds after entering building.
		// assume only from 60% or more, make as a knight to depart building
		if( Random.Range( 0f, 1f ) > .6f )
		{
			pendingGuards++;
			newGuardDelay = 2.0f;
		}
	}

	void exitAsGuard()
	{
		// Now, if pending knights, timer before decreasing human and night count
		// then create new knight (or advanced human later)... if none, get out
		if( pendingGuards == 0 )
			return;

		// decrease timere... if still more time, get out.  Not time to generate knight
		newGuardDelay -= Time.deltaTime;
		if( newGuardDelay > 0f )
			return;

		// decrease knight count first, then the human count...
		pendingGuards--;
		humanCount--;
		// if STILL any pending knights, reset the counter
		if( pendingGuards > 0 )
			newGuardDelay = 2.0f;

		
		// NOW, we had a knight and time was reached...
		// generate one outside the safe house vicinity,
		// then, decrease the knight AND human count.  Since the human count
		// is in case it breaks open the building, the total "people" inside.
		// as the human count is a basis of other points and such within.
		float x, z;
		
		// get gamecontrol object from main camera
		GameObject go = GameObject.FindWithTag("GameController"); 
		gameControl gc = go.GetComponent<gameControl>();
		// if not actually found, get out
		if( gc == null )
			return;

		// Since the building is NOT destroyed, we want our spawning OUTSIDE the
		// building and NOT within it...  Just go min values respectively since no
		// spwan, go based on the house's position less the 1/2 of x or z dimensions
		Transform t = gameObject.transform;
		x = t.position.x - ( t.localScale.x / 2.0f ) -1.0f;
		z = t.position.z + ( t.localScale.z / 2.0f ) +1.0f;
		guardAI gAI = gc.createGuard(new Vector3(x, 1, z), new Quaternion(0f, 0f, 0f, 0f));
		gAI.stationary = false;
		gAI.moveAfterCombat = true;
		((commonAI)gAI).animation.Play("walk");
		((commonAI)gAI).baseSpeed = 4.5f;
		((commonAI)gAI).runSpeed = 7.0f;
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
