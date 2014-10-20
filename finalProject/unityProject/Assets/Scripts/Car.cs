using UnityEngine;
using System.Collections;

public abstract class Car : MonoBehaviour {
	public WheelCollider WheelFL;
	public WheelCollider WheelBL;
	public WheelCollider WheelBR;
	public WheelCollider WheelFR;
	public Transform WheelFLTransform;
	public Transform WheelFRTransform;
	public Transform WheelBLTransform;
	public Transform WheelBRTransform;
	public float maxTorque;
	public float lowestSteerAtSpeed;
	public float lowSpeedSteerAngle;
	public float highSpeedSteerAngle;
	public float decelerationSpeed;
	public float maxReverseSpeed;
	public GameObject backLightObject;
	public float maxBrakeTorque;
	public float topSpeed;

	public int[] gearRatio;

	public Car ()
	{
		maxTorque = 50;
		lowestSteerAtSpeed = 50;
		lowSpeedSteerAngle = 14;
		highSpeedSteerAngle = 5;
		decelerationSpeed = 50;
		maxReverseSpeed = 50;
		maxBrakeTorque = 100;
	}

	public void setCenterOfMass(float x, float y, float z)
	{
		Vector3 temp = rigidbody.centerOfMass;
		temp.x += x;
		temp.y += y;
		temp.z += z;
		rigidbody.centerOfMass = temp;
	}
}
