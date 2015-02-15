using UnityEngine;
using System.Collections;

public class SceneControl : MonoBehaviour, globalEvents.ICharacterCreationService, globalEvents.IManaController {

	public CameraControl cameraControl = null;
	public PlayerSettings playerSettings = null;

	public GameObject	zombiePrefab = null;
	public GameObject	humanPrefab = null;
	public GameObject	werewolfPrefab = null;
	public GameObject	guardPrefab = null;

	public float		zombieCostInMana = 1f;

	void Awake()
	{
		// Called before Start is called on any object
		globalEvents.characterCreator = this;
		globalEvents.manaControllerService = this;

		globalEvents.EnemyElementHit += HandleEnemyElementHit;
		globalEvents.shouldZombiesMoveOnTheirOwn = true;
	}

	// Use this for initialization
	void Start () {
		if (cameraControl == null)
		{
			if ((cameraControl = this.gameObject.GetComponent<CameraControl>()) == null)
			{
				Debug.LogError("Unable to find the CameraControl to use with the SceneControl");
			}
		}

		if (playerSettings == null)
		{
			// Try to find it within the scene as a tag
			GameObject go = GameObject.FindGameObjectWithTag("PlayerSettings");
			playerSettings = go.GetComponent<PlayerSettings>();
		}
	}

	public void ZoomIn(float speed)
	{
		cameraControl.ZoomIn(speed);
	}

	public void PanRight(float speed)
	{
		cameraControl.PanLeft(-speed);
	}

	public void PanForward(float speed)
	{
		cameraControl.PanForward(-speed);
	}

	public class TouchEvent
	{
		public Vector3 currentPosition;
		public Vector3 initialPoint;
		public Vector3 distanceSinceStart;
		public Vector3 deltaDistance;
	}

	// Single touch events are passed onto the active spell
	public void StartTouch(TouchEvent args)
	{
		if (playerSettings != null && playerSettings.ActiveSpell != null)
		{
			playerSettings.ActiveSpell.StartTouch(args);			
		}
	}

	public void MoveTouch(TouchEvent args)
	{
		if (playerSettings != null && playerSettings.ActiveSpell != null)
		{
			playerSettings.ActiveSpell.MoveTouch(args);			
		}
	}

	public void EndTouch(TouchEvent args)
	{
		if (playerSettings != null && playerSettings.ActiveSpell != null)
		{
			playerSettings.ActiveSpell.EndTouch(args);			
		}
	}

	public void CancelTouch(TouchEvent args)
	{
		if (playerSettings != null && playerSettings.ActiveSpell != null)
		{
			playerSettings.ActiveSpell.CancelTouch(args);			
		}
	}

	#region ICharacterCreationService
	
	//Create zombie at position & rotation
	public GameObject createZombie( Vector3 position, Quaternion rotation) 
	{
		GameObject gobj = (GameObject)Instantiate (zombiePrefab, position, rotation); 
		return gobj;
	}
	
	public GameObject createFastZombie( Vector3 position, Quaternion rotation)
	{
		GameObject gObj = createZombie(position, rotation);
		zombieAI zombie = gObj.GetComponent<zombieAI>();
		if (zombie != null)
			zombie.MakeFastZombie();
		return gObj;
	}
	
	public werewolfAi createWerewolf( Vector3 position, Quaternion rot) 
	{
		GameObject gobj = (GameObject)Instantiate (werewolfPrefab, position, rot); 
		werewolfAi wAI = gobj.GetComponentInChildren<werewolfAi>();
		return wAI;
	}
	
	public humanAI createHuman(Vector3 referencePoint, Quaternion rotation)
	{
		GameObject gobj = (GameObject)Instantiate (humanPrefab, referencePoint, rotation); 
		humanAI hAI = gobj.GetComponentInChildren<humanAI>();
		return hAI;
	}
	
	//Create Knight, such as exiting a building
	public guardAI createGuard( Vector3 referencePoint, Quaternion rotation) 
	{
		GameObject gobj = (GameObject)Instantiate (guardPrefab, referencePoint, rotation); 
		guardAI gAI = gobj.GetComponentInChildren<guardAI>();
		return gAI;
	}
	
	#endregion ICharacterCreationService
	
	#region IManaControllerService
	
	public void ChangeMana (float manaDelta)
	{
		playerSettings.PlayerMana += manaDelta;
	}
	
	public bool CanIBuyAZombie() 
	{ 
		return zombieCostInMana <= playerSettings.PlayerMana; 
	}
	
	public GameObject RequestBuyZombie(Vector3 position, Quaternion rotation) 
	{
		if (zombieCostInMana <= playerSettings.PlayerMana) 
		{
			GameObject zombie = createZombie(position, rotation);
			playerSettings.PlayerMana -= zombieCostInMana;
			return zombie;
		}
		
		return null;
	}
	
	#endregion IManaControllerService

	void HandleEnemyElementHit (object sender, globalEvents.GameObjectHitEventArgs e)
	{
		playerSettings.PlayerScore = playerSettings.PlayerScore + 100;
	}
}
