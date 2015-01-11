using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

	public Vector3 axisSpeed;

	void Update(){
		transform.Rotate( axisSpeed * Time.deltaTime);
	}
}
