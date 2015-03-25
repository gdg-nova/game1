using UnityEngine;
using System.Collections;

public class Kart : MonoBehaviour {

	public WheelCollider Fleft;
	public WheelCollider Fright;

	public WheelCollider Rleft;
	public WheelCollider Rright;

	public GameObject fLeftModel;
	public GameObject fRightModel;
	public GameObject rLeftModel;
	public GameObject rRightModel;

	public GameObject volant;

	public float AntiRoll;

	private Vector3 startPosition;
	private Vector3 startRotation;
	private float power;
	private float steerPower;
	private float speed; 

	void Start(){
		startPosition = transform.position;
		startRotation = transform.rotation.eulerAngles;
	}

	void OnGUI(){
		GUI.color = Color.red;
		GUILayout.Label("Arrow key to move");
		GUILayout.Label("Esc to restart");
		GUILayout.Label("Space to reset");
		GUILayout.Label("It's an example, you must implement your own physic");
	}
	void  Update(){

		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Escape)){
			GetComponent<Rigidbody>().freezeRotation = true;
			GetComponent<Rigidbody>().transform.eulerAngles = Vector3.zero;
			GetComponent<Rigidbody>().freezeRotation = false;

			if (Input.GetKey(KeyCode.Escape)){
				transform.position = startPosition;
				transform.rotation= Quaternion.Euler( startRotation );
			}
		}
	}

	void OnCollisionEnter(){
		GetComponent<AudioSource>().pitch = 0.5f;
	}

	// Update is called once per frame
	void FixedUpdate () {



		speed = ((Fleft.rpm /60f) * 2 * Mathf.PI) * 0.45f * 3.6f;
		GetComponent<AudioSource>().pitch = 0.5f + speed/200f;

		float throttle = Input.GetAxis("Vertical");
		float steer = Input.GetAxis("Horizontal");

		if (throttle!=0){
			power = power + 10 * Time.deltaTime;
			power = Mathf.Clamp( power,-50,60);
			Rleft.brakeTorque = 0;
			Rright.brakeTorque =0;


		}
		else{
			power -=  10 * Time.deltaTime;
			power = Mathf.Clamp( power,0,60);
			Rleft.brakeTorque = 10;
			Rright.brakeTorque = 10;
		}
	
		Rleft.motorTorque = throttle*power;
		Rright.motorTorque = throttle*power;


		if (steer!=0){
			steerPower += 0.3f;
		}
		else{
			steerPower=0;	
		}
		if (speed>0){
			Fleft.steerAngle = steer * steerPower  ;
			Fright.steerAngle =steer*steerPower ;
		}
		
		GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity,70);

		if (steer!=0){
			GetComponent<Rigidbody>().AddForceAtPosition(Fleft.transform.up * -AntiRoll , Fleft.transform.position);
			GetComponent<Rigidbody>().AddForceAtPosition(Fright.transform.up * -AntiRoll, Fright.transform.position);
			GetComponent<Rigidbody>().AddForceAtPosition(Rleft.transform.up * -AntiRoll, Rleft.transform.position);
			GetComponent<Rigidbody>().AddForceAtPosition(Rright.transform.up * -AntiRoll, Rright.transform.position);
		}

		fLeftModel.transform.Rotate( Vector3.right * Fleft.rpm * Time.deltaTime);
		fRightModel.transform.Rotate( Vector3.right * Fright.rpm * Time.deltaTime);
		rLeftModel.transform.Rotate( Vector3.right * Rleft.rpm * Time.deltaTime);
		rRightModel.transform.Rotate( Vector3.right * Rright.rpm * Time.deltaTime);

		GetComponent<Rigidbody>().centerOfMass= new Vector3(0,-2f * throttle);
		//rigidbody.centerOfMass = new Vector3(0,0,-2f);
		volant.transform.Rotate( Vector3.up * steer , Space.Self);

	}
}

