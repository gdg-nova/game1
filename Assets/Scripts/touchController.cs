using UnityEngine;
using System.Collections;

public class touchController : MonoBehaviour {

	public cameraController mainCameraController = null;
	public bool enablePinchToZoom = true;
	public bool enableMultiTouchSwipe = true;

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
				if (enablePinchToZoom || enableMultiTouchSwipe)
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
				// Rule is: If the horizontal or veritical swipe speed exceeds the rate at which the
				// fingers come together, then it's a swipe otherwise it's a pinch to zoom
				Debug.Log (string.Format("H: {0}, V: {1}, Dis: {2}", horizontalSwipe, veriticalSwipe, deltaDistance));
				if (Mathf.Abs(horizontalSwipe) > absDeltaDistance * 1.5 || 
				    Mathf.Abs(veriticalSwipe) > absDeltaDistance * 1.5)
				{
					deltaDistance = 0.0f;
				}
				else
				{
					horizontalSwipe = 0.0f;
					veriticalSwipe = 0.0f;
				}

				if (deltaDistance < 0.0f)
				{
					mainCameraController.ZoomOut( -deltaDistance*touches[0].deltaTime );
					originalPinchDistance = currentDistance;
				}
				else if (deltaDistance > 0.0f)
				{
					mainCameraController.ZoomIn( deltaDistance*touches[0].deltaTime );
					originalPinchDistance = currentDistance;
				}
				mainCameraController.panLeft( horizontalSwipe*touches[0].deltaTime );
				mainCameraController.panForward( veriticalSwipe*touches[0].deltaTime );
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
