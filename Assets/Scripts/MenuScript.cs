using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class MenuScript : MonoBehaviour
{
	private GUISkin skin;
	public Transform	mainCamera;
	public TerrainCollider terrain;
	public float 		speed = 5.0f;
	private float		currentAngle = 0.0f;

	private Vector3		lookAtPoint;
	private Vector3		directionVector;
	private Vector3		centerOfCameraArc;
	private Vector3		originalCameraPosition;
	private Vector3		originalUpVector;
	
	void Start()
	{
		skin = Resources.Load("GUISkin") as GUISkin;

		if (mainCamera == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
			if (go != null)
			{
				mainCamera = go.transform;
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
		directionVector = new Vector3( lookAtPoint.x - mainCamera.position.x, 0, lookAtPoint.z - mainCamera.position.z ).normalized;
		centerOfCameraArc = new Vector3(
			lookAtPoint.x,
			0,
			lookAtPoint.z );
		Debug.Log ("Center of camera:" + centerOfCameraArc);
		Debug.Log ("Original pos:" + mainCamera.position);
		
		originalCameraPosition = mainCamera.position;
		originalUpVector = mainCamera.up;
	}

	void UpdateCameraPosition(float newAngle)
	{		
		float x = originalCameraPosition.x, y = originalCameraPosition.y, z = originalCameraPosition.z;
		float a = centerOfCameraArc.x, b = centerOfCameraArc.y, c = centerOfCameraArc.z;
		float u = 0.0f, v = 1.0f, w = 0.0f;
		float theta = Mathf.PI * ( newAngle / 180.0f );
		float cosineTheta = Mathf.Cos ( theta );
		float sineTheta = Mathf.Sin ( theta );
		
		Vector3 newPosition = new Vector3(
			(a*(v*v + w*w) - u*(b*v + c*w - u*x - v*y - w*z))*(1-cosineTheta) + x*cosineTheta + (-c*v + b*w - w*y + v*z)*sineTheta,
			(b*(u*u + w*w) - v*(a*u + c*w - u*x - v*y - w*z))*(1-cosineTheta) + y*cosineTheta + ( c*u - a*w + w*x - u*z)*sineTheta,
			(c*(u*u + v*v) - w*(a*u + b*v - u*x - v*y - w*z))*(1-cosineTheta) + z*cosineTheta + (-b*u + a*v - v*x + u*y)*sineTheta);

		Debug.Log ("New position: " + newPosition);
		mainCamera.position = newPosition;
		mainCamera.LookAt( lookAtPoint, Vector3.up );
	}
	
	void OnGUI()
	{
		const int buttonWidth = 84;
		const int buttonHeight = 60;
		
		GUI.skin = skin;
		
		// Draw a button to start the game
		if (
			GUI.Button(
			// Center in X, 2/3 of the height in Y
			new Rect(
			Screen.width / 2 - (buttonWidth / 2),
			(12 * Screen.height / 16) - (buttonHeight / 2),
			buttonWidth,
			buttonHeight
			),
			"Start!"
			)
			)
		{
			// On Click, load the first level.
			// "Stage1" is the name of the first scene we created.
			Application.LoadLevel("Tutorial_Scene");
		}
	}
	void Update()
	{
		currentAngle += speed * Time.deltaTime;
		UpdateCameraPosition(currentAngle);

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

}