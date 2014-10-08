using UnityEngine;
using System.Collections;

public class touchController : MonoBehaviour {

	public cameraController mainCameraController = null;
	public bool enablePinchToZoom = true;

	void Start () {
		if (mainCameraController == null && enablePinchToZoom)
		{
			if ((mainCameraController = this.gameObject.GetComponent<cameraController>()) == null)
			{
				Debug.LogError("Unable to find the cameraController to use with the touchController");
				enablePinchToZoom = false;
			}
		}

		if (!Input.multiTouchEnabled)
		{
			enablePinchToZoom = false;
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
				// Check for pinch 
				if (enablePinchToZoom)
				{
					hasDonePinchToZoom = true;
					float currentDistance = Vector2.Distance(touches[0].position, touches[1].position);
					if (currentDistance < originalPinchDistance)
					{
						mainCameraController.ZoomOut( (originalPinchDistance-currentDistance)*touches[0].deltaTime );
						originalPinchDistance = currentDistance;
					}
					else if (currentDistance > originalPinchDistance)
					{
						mainCameraController.ZoomIn( (currentDistance-originalPinchDistance)*touches[0].deltaTime );
						originalPinchDistance = currentDistance;
					}
				}
			}
		} 
		else
		{
			// No touches so reset it
			lastTouchCount = 0;
			hasDonePinchToZoom = false;
		}
	}
}
