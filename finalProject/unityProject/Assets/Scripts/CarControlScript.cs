using UnityEngine;
using System.Collections;

public class CarControlScript : MonoBehaviour {

	public WheelCollider WheelFL;
	public WheelCollider WheelBL;
	public WheelCollider WheelBR;
	public WheelCollider WheelFR;
	public float maxTorque = 50;

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
		WheelFL.steerAngle = 10 * Input.GetAxis("Horizontal");
		WheelFR.steerAngle = 10 * Input.GetAxis("Horizontal");
	}
}
