using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public float minAngle = -30.0f;
	public float startAngle = 0.0f;
	public float maxAngle = 65.0f;
	public Transform mainCamera = null;
	public TerrainCollider terrain = null;
	
	public float panningAreaMinX = -50.0f;
	public float panningAreaMaxX = 50.0f;
	public float panningAreaMinZ = -25.0f;
	public float panningAreaMaxZ = 50.0f;
	
	private Vector3 lookAtPoint;
	private Vector3 centerOfCameraArc;
	
	private Vector3 directionVector;
	private Vector3 axisOfRotation; // Bi-normal to directionVector and centerOfCameraArc-lookAtPoint
	
	private float currentAngle;
	private Vector3 originalCameraPosition;
	private Vector3 originalUpVector; // Do we need this?
	
	private float	currentXPointInPanningArea = 0.0f;
	private float   currentZPointInPanningArea = 0.0f;
	
	void Start () {
		
		if (mainCamera == null)
		{
			GameObject gameObj =  GameObject.FindWithTag("MainCamera");
			if (gameObj == null || (mainCamera = gameObj.GetComponent<Transform>()) == null)
			{
				Debug.LogError("Unable to find camera to attach to");
				return;
			}
		}
		
		if (terrain == null)
		{
			Terrain terrainObj = GameObject.FindObjectOfType<Terrain>();
			if (terrainObj == null || (terrain = terrainObj.GetComponent<TerrainCollider>()) == null)
			{
				Debug.LogError("Unable to find terrain with collider to use");
			}
		}
		
		RaycastHit hit;
		if (!terrain.Raycast(new Ray(mainCamera.position, mainCamera.forward), out hit, Mathf.Infinity))
		{
			Debug.LogError("Camera is not looking at the terrain");
			return;
		}
		
		lookAtPoint = hit.point;
		directionVector = new Vector3( lookAtPoint.x - mainCamera.position.x, 0, lookAtPoint.z - mainCamera.position.z );
		centerOfCameraArc = new Vector3(
			lookAtPoint.x,
			mainCamera.position.y + Mathf.Tan ( startAngle ) * directionVector.magnitude,
			lookAtPoint.z );
		
		directionVector = directionVector.normalized;
		Vector3 normalVector = new Vector3( 0, 1, 0); // Always facing upright above the lookAtPoint.
		
		axisOfRotation = Vector3.Cross( normalVector, directionVector ); // Should have length 1, no need to normalize again.
		
		// Start angle is the minimum angle.
		currentAngle = startAngle;
		originalCameraPosition = mainCamera.position;
		originalUpVector = mainCamera.up;
	}
	
	void UpdateCameraPosition(float newAngle)
	{
		if (newAngle < minAngle) newAngle = minAngle;
		if (newAngle > maxAngle) newAngle = maxAngle;
		
		float x = originalCameraPosition.x, y = originalCameraPosition.y, z = originalCameraPosition.z;
		float a = centerOfCameraArc.x, b = centerOfCameraArc.y, c = centerOfCameraArc.z;
		float u = axisOfRotation.x, v = axisOfRotation.y, w = axisOfRotation.z;
		float theta = Mathf.PI * ( (newAngle - startAngle) / 180.0f );
		float cosineTheta = Mathf.Cos ( theta );
		float sineTheta = Mathf.Sin ( theta );
		
		Vector3 newPosition = new Vector3(
			(a*(v*v + w*w) - u*(b*v + c*w - u*x - v*y - w*z))*(1-cosineTheta) + x*cosineTheta + (-c*v + b*w - w*y + v*z)*sineTheta,
			(b*(u*u + w*w) - v*(a*u + c*w - u*x - v*y - w*z))*(1-cosineTheta) + y*cosineTheta + ( c*u - a*w + w*x - u*z)*sineTheta,
			(c*(u*u + v*v) - w*(a*u + b*v - u*x - v*y - w*z))*(1-cosineTheta) + z*cosineTheta + (-b*u + a*v - v*x + u*y)*sineTheta);
		
		mainCamera.position = newPosition;
		currentAngle = newAngle;
		mainCamera.LookAt( lookAtPoint, originalUpVector );
	}
	
	private void UploadInternalCameraPoints(Vector3 translateDirection, float distance)
	{
		Vector3 deltaPos = translateDirection * distance;
		mainCamera.position = mainCamera.position + deltaPos;
		
		// Translate all points by this.
		originalCameraPosition = originalCameraPosition + deltaPos;
		centerOfCameraArc = centerOfCameraArc + deltaPos;
		lookAtPoint = lookAtPoint + deltaPos;
		mainCamera.LookAt( lookAtPoint, originalUpVector );
	}
	
	public void ZoomOut(float angle)
	{
		UpdateCameraPosition( currentAngle + angle );
	}
	
	public void ZoomIn(float angle)
	{
		UpdateCameraPosition( currentAngle - angle );
	}
	
	public void PanLeft(float distance)
	{
		//Debug.Log(string.Format("Panning left: {0}", distance));
		float newDistance = -distance;
		
		float newPos = currentXPointInPanningArea + newDistance;
		if (newPos < panningAreaMinX) newDistance -= newPos-panningAreaMinX; 
		if (newPos > panningAreaMaxX) newDistance -= newPos-panningAreaMaxX;
		
		currentXPointInPanningArea += newDistance;
		UploadInternalCameraPoints( axisOfRotation, newDistance );
	}
	
	static Vector3 verticalUp = new Vector3( 0.0f, 1.0f, 0.0f );
	public void PanForward(float distance)
	{
		//Debug.Log(string.Format("Panning forward: {0}", distance));
		float newDistance = -distance;
		
		float newPos = currentZPointInPanningArea + newDistance;
		if (newPos < panningAreaMinZ) newDistance -= newPos-panningAreaMinZ; 
		if (newPos > panningAreaMaxZ) newDistance -= newPos-panningAreaMaxZ;
		
		currentZPointInPanningArea += newDistance;
		UploadInternalCameraPoints(directionVector, newDistance);
	}
}
