using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class KeyMouseInput : MonoBehaviour {

	public float speed = 65.0f;
	public SceneControl sceneControl = null;

	// Use this for initialization
	void Start () {
		if (sceneControl == null)
		{
			if ((sceneControl = this.gameObject.GetComponent<SceneControl>()) == null)
			{
				Debug.LogError("Unable to find the SceneControl to use with the KeyMouseInput");
			}
		}
	}

	float timeOfFirstPress = 0.0f;
	float timeToCountFirstTouchDown = 0.05f; // User has to press this time to count as a mouse click
	int touchState = 0; // 0 = no touch; 1 = touch down; 2 = cancel by multi-touch
	Vector3 mouseTouchDownPoint = new Vector3();
	Vector3 mouseLastRecordedPoint = new Vector3();

	SceneControl.TouchEvent touchArgs = new SceneControl.TouchEvent();

	List<RaycastResult> hits = new List<RaycastResult>();
	
	// Update is called once per frame
	void Update () {

		float zoom = Input.GetAxis("Zoom");
		if (zoom != 0.0f)
		{
			sceneControl.ZoomIn(zoom * speed * Time.deltaTime);
		}
		
		float horizontal = Input.GetAxis ("Horizontal");
		if (horizontal != 0.0f)
		{
			sceneControl.PanRight(horizontal * Time.deltaTime);
		}

		float vertical = Input.GetAxis ("Vertical");
		if (vertical != 0.0f)
		{
			sceneControl.PanForward(vertical * Time.deltaTime);
		}

		// Now check for mouse movement
		if (timeOfFirstPress == 0.0f)
		{
			if (Input.GetMouseButton(0))
			{
				// Check if we hit any 
				PointerEventData pe = new PointerEventData(EventSystem.current);
				pe.position = Input.mousePosition;
				
				EventSystem.current.RaycastAll( pe, hits );
				
				if (hits.Count > 0)
				{
					//Debug.Log ("Touched a GUI item");
					// Wait until we lift up
					touchState = 2;
					hits.Clear();
				}

				timeOfFirstPress = Time.time;
			}
			return;
		}
		if (Input.GetMouseButton(0))
		{
			// Mouse was pressed and we have met the threshold time
			if (touchState == 0 && Time.time > timeOfFirstPress + timeToCountFirstTouchDown)
			{
				// Currently not finalized the touch process
				mouseTouchDownPoint = Input.mousePosition;
				mouseLastRecordedPoint = mouseTouchDownPoint;
				touchState = 1;

				touchArgs.initialPoint = mouseTouchDownPoint;
				touchArgs.distanceSinceStart = new Vector3();
				touchArgs.deltaDistance = touchArgs.distanceSinceStart;
				sceneControl.StartTouch(touchArgs);
			}
			// Mouse was already pressed
			else if (touchState == 1)
			{
				touchArgs.distanceSinceStart = Input.mousePosition - mouseTouchDownPoint;
				touchArgs.deltaDistance = Input.mousePosition - mouseLastRecordedPoint;
				mouseLastRecordedPoint = Input.mousePosition;
				sceneControl.MoveTouch(touchArgs);
			}
		}
		else if (touchState == 1)
		{
			// Lifted up the mouse
			touchArgs.distanceSinceStart = Input.mousePosition - mouseTouchDownPoint;
			touchArgs.deltaDistance = Input.mousePosition - mouseLastRecordedPoint;
			sceneControl.EndTouch(touchArgs);
			touchState = 0;
			timeOfFirstPress = 0.0f;
		}
		else
		{
			ClearTouchOperation();
		}
	}

	public void CancelTouchOperation()
	{
		if (touchState == 2)
			return;
		if (touchState == 0)
		{
			if (timeOfFirstPress != 0.0f)
			{
				touchState = 2;
			}
			// Don't send cancel because we didn't get to send start
			return;
		}
		sceneControl.CancelTouch(touchArgs);
		touchState = 2;
	}

	public void ClearTouchOperation()
	{
		// If we were previously canceled then clear that state.
		if (touchState == 2)
		{
			touchState = 0;
			timeOfFirstPress = 0.0f;
		}
	}
}
