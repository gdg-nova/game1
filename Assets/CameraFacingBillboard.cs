//cameraFacingBillboard.cs v02
//by Neil Carter (NCarter)
//modified by Juan Castaneda (juanelo)
//
//added in-between GRP object to perform rotations on
//added auto-find main camera
//added un-initialized state, where script will do nothing
using UnityEngine;
using System.Collections;


public class CameraFacingBillboard : MonoBehaviour
{
	
	public Camera m_Camera;
	public bool amActive =false;
	public bool autoInit =false;
	GameObject myContainer;	
	
	void Awake(){
		if (autoInit == true){
			m_Camera = Camera.main;
			amActive = true;
		}
		
		myContainer = new GameObject();
		myContainer.name = "GRP_"+transform.gameObject.name;
		myContainer.transform.position = transform.position;
		transform.parent = myContainer.transform;
	}
	
	
	void Update(){
		if(amActive==true){
			myContainer.transform.LookAt(myContainer.transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
		}
	}
}