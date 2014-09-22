using UnityEngine;
using System.Collections;

public class CarCameraScript : MonoBehaviour {

	public Transform car;
	public float distance = 1.4f;
	public float height = 2.4f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	public float zoomRatio = 2.5f;
	public float defaultFOV = 7.5f;
	private Vector3 rotationVector;

	// Use this for initialization
	void Start () {
	
	}

	// LateUpdate is called once per frame after Update.
	void LateUpdate () {
		float wantedAngle = rotationVector.y;
		float wantedHeight = car.position.y + height;
		float myAngle = transform.eulerAngles.y;
		float myHeight = transform.position.y;
		myAngle = Mathf.LerpAngle (myAngle, wantedAngle, rotationDamping * Time.deltaTime);
		myHeight = Mathf.Lerp (myHeight, wantedHeight, heightDamping * Time.deltaTime);
		Quaternion currentRotation = Quaternion.Euler (0, myAngle, 0); 
		transform.position = car.position;
		transform.position -= currentRotation * Vector3.forward * distance; //put the camera at the back of the car.
		transform.position = new Vector3 (transform.position.x, myHeight, transform.position.z-90);
		transform.LookAt (car);
	}

	void FixedUpdate() {
		Vector3 localVelocity = car.InverseTransformDirection (car.rigidbody.velocity);
		if (localVelocity.z < -0.5f) {
			rotationVector.y = car.eulerAngles.y + 180;
		} else {
			rotationVector.y = car.eulerAngles.y;
		}

		float speed = car.rigidbody.velocity.magnitude;
		camera.fieldOfView = defaultFOV + speed * zoomRatio * Time.deltaTime;
	}
}
