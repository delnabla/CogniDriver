using UnityEngine;
using System.Collections;

public class CarCameraScript : MonoBehaviour {

	public Transform car;
	public float distance = 31;
	public float height = 5.0f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	public float zoomRatio = 2.5f;
	public float defaultFOV = 20.5f;
	private Vector3 rotationVector;
	private float initialCarRotation;

	// Use this for initialization
	void Start () {
		initialCarRotation = car.eulerAngles.y;
	}

	// LateUpdate is called once per frame after Update.
	void LateUpdate () {
		// Calculate the current rotation angles
		float wantedAngle = rotationVector.y-initialCarRotation;
		float wantedHeight = car.position.y + height;

		float currentAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentAngle = Mathf.LerpAngle (currentAngle, wantedAngle, rotationDamping * Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentAngle, 0); 
		transform.position = car.position;

		// Set the position of the camera on the x-z plane to 10 distance meters behind the target
		transform.position -= currentRotation * Vector3.forward * distance; 

		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);

		// Always look at the car.
		transform.LookAt (car);
	}

	void FixedUpdate() {
		// If the car is in reverse, move the camera to the front of the car so we can see where we drive to.
		if (Input.GetAxis ("Vertical") < 0) {
			rotationVector.y = car.eulerAngles.y + 180;
		} else {
			rotationVector.y = car.eulerAngles.y;
		}

		float speed = car.rigidbody.velocity.magnitude;

		//Set the field of view of the camera.
		camera.fieldOfView = defaultFOV + speed * zoomRatio * Time.deltaTime;
	}
}
