using UnityEngine;
using System.Collections;

public class Windmill : MonoBehaviour
{
	public float rotationSpeed = 10;

	void Update()
	{
		transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
	}
}
