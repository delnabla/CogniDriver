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
	public float highestSpeed = 50;
	public float lowSpeedSteerAngle = 10;
	public float highSpeedSteerAngle = 1;
	public Vector3 eulerTest;

	// Use this for initialization
	void Start () {
		Vector3 temp = rigidbody.centerOfMass;
		temp.y += -25.8f;
		rigidbody.centerOfMass = temp;
	}
	
	// FixedUpdate is called multiple times per frame
	void FixedUpdate () {
		WheelBR.motorTorque = maxTorque * Input.GetAxis("Vertical");
		WheelBL.motorTorque = maxTorque * Input.GetAxis("Vertical");
		float speedFactor = rigidbody.velocity.magnitude;
		float currentSteerAngle = Mathf.Lerp (lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor);
		currentSteerAngle *= Input.GetAxis ("Horizontal");
		WheelFL.steerAngle = 10 * Input.GetAxis("Horizontal");
		WheelFR.steerAngle = 10 * Input.GetAxis("Horizontal");
	}

	void Update() {
		WheelFLTransform.Rotate (WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelFRTransform.Rotate (WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelBLTransform.Rotate (WheelBL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		WheelBRTransform.Rotate (WheelBR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		/*eulerTest = WheelFLTransform.localEulerAngles;
		Vector3 temp = WheelFLTransform.localEulerAngles;
		temp.y = WheelFL.steerAngle - WheelFLTransform.localEulerAngles.z;
		WheelFLTransform.localEulerAngles = temp;
		temp = WheelFRTransform.localEulerAngles;
		temp.y = WheelFR.steerAngle - WheelFRTransform.localEulerAngles.z;
		WheelFRTransform.localEulerAngles = temp;*/

	}
}
