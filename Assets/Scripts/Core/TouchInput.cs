using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {
	
	public SceneControl sceneControl = null;
	public KeyMouseInput keyMouseInput = null;
	public bool enablePinchToZoom = true;
	public bool enableMultiTouchSwipe = true;
	public float sensitivity = 0.25f;
	
	void Start () {
		if (sceneControl == null && enablePinchToZoom)
		{
			if ((sceneControl = this.gameObject.GetComponent<SceneControl>()) == null)
			{
				Debug.LogError("Unable to find the SceneControl to use with the TouchInput");
				enablePinchToZoom = false;
			}
		}
		if (keyMouseInput == null)
		{
			if ((keyMouseInput = this.gameObject.GetComponent<KeyMouseInput>()) == null)
			{
				Debug.LogError("Unable to find the KeyMouseInput to use with the TouchInput");
			}
		}

		if (!Input.multiTouchEnabled)
		{
			enablePinchToZoom = false;
			enableMultiTouchSwipe = false;
		}
	}
	
	int lastTouchCount;
	Vector2 touch0StartPosition;
	Vector2 touch1StartPosition;
	float originalPinchDistance;
	
	public int currentTouchCount 
	{
		get { return lastTouchCount; }
	}
	
	public bool hasDonePinchToZoom { get; private set; }
	
	void Update () {
		
		if (!enablePinchToZoom && !enableMultiTouchSwipe)
			return;
		if (Input.touchCount > 0)
		{
			Touch[] touches = Input.touches;
			if (touches.Length != lastTouchCount)
			{
				lastTouchCount = touches.Length;
				touch0StartPosition = touches[0].position;
				if (lastTouchCount > 1)
				{
					touch1StartPosition = touches[1].position;
					originalPinchDistance = Vector2.Distance(touch1StartPosition, touch0StartPosition);
				}
				return; // Processing of the new touch count will begin on the next frame
			}
			
			if (lastTouchCount >= 2)
			{
				keyMouseInput.CancelTouchOperation(); // Make sure single-click operation has been canceled.

				hasDonePinchToZoom = true;
				
				// We need to arbitrate between pinching and swiping.
				// In this case we will need to calculate the determining factors and see which one ones
				float currentDistance = enablePinchToZoom?Vector2.Distance(touches[0].position, touches[1].position):0.0f;
				float deltaDistance = currentDistance - originalPinchDistance;
				float absDeltaDistance = Mathf.Abs(deltaDistance);
				float horizontalSwipe = 0.0f;
				float veriticalSwipe = 0.0f;
				if (enableMultiTouchSwipe)
				{
					if ((touches[0].deltaPosition.x > 0 && touches[1].deltaPosition.x > 0) ||
					    (touches[0].deltaPosition.x < 0 && touches[1].deltaPosition.x < 0))
					{
						horizontalSwipe = (touches[0].deltaPosition.x + touches[1].deltaPosition.x)/2;
					}
					if ((touches[0].deltaPosition.y > 0 && touches[1].deltaPosition.y > 0) ||
					    (touches[0].deltaPosition.y < 0 && touches[1].deltaPosition.y < 0))
					{
						veriticalSwipe = (touches[0].deltaPosition.y + touches[1].deltaPosition.y)/2;
					}
				}

				if (deltaDistance != 0.0f)
				{
					sceneControl.ZoomIn( deltaDistance*sensitivity );
					originalPinchDistance = currentDistance;
				}
				if (horizontalSwipe != 0.0f)
				{
					sceneControl.PanRight( -horizontalSwipe*sensitivity );
				}
				if (veriticalSwipe != 0.0f)
				{
					//Debug.Log ("Vertical: " + veriticalSwipe.ToString() + " Delta:" + touches[0].deltaTime.ToString());
					sceneControl.PanForward( -veriticalSwipe*sensitivity );
				}
			}
		} 
		else if (lastTouchCount > 0)
		{
			// Clear any potentially canceled single touch operations
			keyMouseInput.ClearTouchOperation();
			// No touches so reset it
			lastTouchCount = 0;
			hasDonePinchToZoom = false;
		}
	}
}