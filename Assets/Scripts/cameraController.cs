using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	public float startAngle = 0.0f;
	public float maxAngle = 65.0f;
	public float speed = 65.0f;
	public Transform mainCamera = null;
	public TerrainCollider terrain = null;

	private float minAngle;

	private Vector3 lookAtPoint;
	private Vector3 centerOfCameraArc;

	private Vector3 axisOfRotation;

	private float currentAngle;
	private Vector3 originalCameraPosition;
	private Vector3 originalUpVector; // Do we need this?

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
		Vector3 directionVector = new Vector3( lookAtPoint.x - mainCamera.position.x, 0, lookAtPoint.z - mainCamera.position.z );
		centerOfCameraArc = new Vector3(
			lookAtPoint.x,
			mainCamera.position.y + Mathf.Tan ( startAngle ) * directionVector.magnitude,
			lookAtPoint.z );
			
		directionVector = directionVector.normalized;
		Vector3 normalVector = new Vector3( 0, 1, 0); // Always facing upright above the lookAtPoint.

		axisOfRotation = Vector3.Cross( normalVector, directionVector ); // Should have length 1, no need to normalize again.

		// Start angle is the minimum angle.
		minAngle = startAngle;
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
		float theta = Mathf.PI * ( (newAngle - minAngle) / 180.0f );
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

	public void ZoomOut(float angle)
	{
		UpdateCameraPosition( currentAngle + 0.16f * angle * speed );
	}

	public void ZoomIn(float angle)
	{
		UpdateCameraPosition( currentAngle - 0.16f * angle * speed );
	}
	
	void Update () {
		float vertical = Input.GetAxis("Vertical");

		if (vertical != 0.0f)
		{
			UpdateCameraPosition( currentAngle + speed * vertical * Time.deltaTime );
		}
	}
}
