using UnityEngine;
using System.Collections;

public class SceneControl : MonoBehaviour {

	public CameraControl cameraControl = null;
	public PlayerSettings playerSettings = null;

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
	
	// Update is called once per frame
	void Update () {
	
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
}
