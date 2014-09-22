using UnityEngine;
using System.Collections;

public class CarControlScript : MonoBehaviour {

	public WheelCollider WheelFL;
	public WheelCollider WheelBL;
	public WheelCollider WheelBR;
	public WheelCollider WheelFR;
	public Transform WheelFLTransform;
	public Transform WheelFRTransform;
	public Transform WheelBLTransform;
	public Transform WheelBRTransform;
	public float maxTorque = 50;
	public float lowestSteerAtSpeed = 50;
	public float lowSpeedSteerAngle = 10;
	public float highSpeedSteerAngle = 1;
	public float decelerationSpeed = 50;
	public float currentSpeed;
	public float topSpeed = 150;
	
	// Use this for initialization
	void Start () {
		Vector3 temp = rigidbody.centerOfMass;
		temp.y += -1.8f;
		rigidbody.centerOfMass = temp;
	}
	
	// FixedUpdate is called multiple times per frame
	void FixedUpdate () {
		currentSpeed = rigidbody.velocity.magnitude;
		currentSpeed = Mathf.Round (currentSpeed);
		if (currentSpeed < topSpeed) {
			WheelBR.motorTorque = maxTorque * Input.GetAxis ("Vertical");
			WheelBL.motorTorque = maxTorque * Input.GetAxis ("Vertical");
		} else {
			WheelBR.motorTorque = 0;
			WheelBL.motorTorque = 0;
		}

		if (!Input.GetButton ("Vertical")) {
			WheelBR.brakeTorque = decelerationSpeed;
			WheelBL.brakeTorque = decelerationSpeed;
		} else {
			WheelBR.brakeTorque = 0;
			WheelBL.brakeTorque = 0;
		}

		float currentSteerAngle = Mathf.Lerp (lowSpeedSteerAngle, highSpeedSteerAngle, currentSpeed);
		currentSteerAngle *= Input.GetAxis ("Horizontal");
		WheelFL.steerAngle = 10 * Input.GetAxis("Horizontal");
		WheelFR.steerAngle = 10 * Input.GetAxis("Horizontal");
	}

	//Update is called once per frame.
	void Update() {
		WheelFLTransform.Rotate (WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelFRTransform.Rotate (WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelBLTransform.Rotate (WheelBL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelBRTransform.Rotate (WheelBR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelFLTransform.localEulerAngles = new Vector3 (WheelFLTransform.localEulerAngles.x, 
		                                                WheelFL.steerAngle - WheelFLTransform.localEulerAngles.z + 90,
		                                                WheelFLTransform.localEulerAngles.z); 
		WheelFRTransform.localEulerAngles = new Vector3 (WheelFRTransform.localEulerAngles.x,
		                                                WheelFR.steerAngle - WheelFRTransform.localEulerAngles.z + 90,
		                                                WheelFRTransform.localEulerAngles.z);
	}
}
