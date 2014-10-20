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
	public bool isTopCamera;
	private int cameraRoll = 0;	

	// Use this for initialization
	void Start () {
		initialCarRotation = car.eulerAngles.y;
	}

	// LateUpdate is called once per frame after Update.
	void LateUpdate () {
		float currentHeight = 0;
		Quaternion currentRotation = Quaternion.Euler(0, 0, 0);
		
		if (!isTopCamera)
		{
			// Calculate the current rotation angle
			float wantedAngle = rotationVector.y - initialCarRotation;
			float currentAngle = transform.eulerAngles.y;
	
			// Damp the rotation around the y-axis
			currentAngle = Mathf.LerpAngle (currentAngle, wantedAngle, rotationDamping * Time.deltaTime);
			
			// Convert the angle into a rotation
			currentRotation = Quaternion.Euler (0, currentAngle, 0); 

			//Calculate the wanted height.
			float wantedHeight = car.position.y + height;
			currentHeight = transform.position.y;
	
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		}		

		// Set the position of the camera on the x-z plane to 10 distance meters behind the target if this is the main camera.
		// or 10 distance meters above the target if this is the top camera.
		if (isTopCamera)
		{
			currentHeight = transform.position.y;
			transform.position = car.position - Vector3.up * distance;
		}
		else
		{
			if (Input.GetKeyDown (KeyCode.C)) // if 'c' key is pressed, change camera position.
			{
				cameraRoll++;
				if (cameraRoll == 3) //if we reached a bigger index than the number of camera positions, reset to 0.
					cameraRoll = 0; 
			}
			switch(cameraRoll)
			{
				//back of the car view
				case 0:
					transform.position = car.position - currentRotation * Vector3.forward * distance; 	
					Debug.Log ('0');
					break; 

				//steering wheel view
				case 1: 
					transform.position = car.position - currentRotation * Vector3.forward * 2;
					Debug.Log('1');
					break;
						
				//over the board view
				case 2: 
					transform.position = car.position - currentRotation * Vector3.forward * 1;
					Debug.Log ('2');
					break;						
			}
		}
		
		if (cameraRoll != 0)
			currentHeight = transform.position.y;

		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);

		// Always look at the car.
		transform.LookAt (car);
	}

	void FixedUpdate() {
		if (!isTopCamera)
		{
			// If the car is in reverse, move the camera to the front of the car so we can see where we drive to.
			if (Input.GetAxis ("Vertical") < 0) {
				rotationVector.y = car.eulerAngles.y + 180;
			} else {
				rotationVector.y = car.eulerAngles.y;
			}
		
			//Get the current speed of the car.
			float speed = car.rigidbody.velocity.magnitude;
			
			//Set the field of view of the camera.
			camera.fieldOfView = defaultFOV + speed * zoomRatio * Time.deltaTime;
		}
	}

} // class 
