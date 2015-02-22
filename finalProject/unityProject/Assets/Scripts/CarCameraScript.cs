using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarCameraScript : MonoBehaviour {

	public Transform car;
	public Transform emptyObjectInFrontOfCar;
	public float distance = 31;
	public float height = 5.0f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	public float zoomRatio = 2.5f;
	public float defaultFOV = 20.5f;
	private Vector3 rotationVector;
	private float initialCarRotationY;
	private Quaternion initialRotation;
	public bool isTopCamera;
	private int cameraRoll = 0;	

	// Use this for initialization
	void Start () 
	{
		initialCarRotationY = car.eulerAngles.y;
		initialRotation = car.rotation;
	}

	IEnumerator Blink()
	{
		float endTime = Time.time + 1.4f;
		while (Time.time < endTime)
		{
			car.renderer.enabled = false;
			yield return new WaitForSeconds(0.2f);
			car.renderer.enabled = true;
			yield return new WaitForSeconds(0.2f);
		}
	}

	// LateUpdate is called once per frame after Update.
	void LateUpdate () 
	{
		float currentHeight = 0;

		Quaternion currentRotation = Quaternion.Euler(0, 0, 0);

		//If car is flipped over, put it in normal position.
		if ((Mathf.Round (car.eulerAngles.x) % 180 == 0 && Mathf.Round (car.eulerAngles.x) % 360 != 0) || (Mathf.Round (car.eulerAngles.z) % 180 == 0 && Mathf.Round (car.eulerAngles.z) % 360 != 0))
		{
			Debug.Log("Car is flipped!");
			car.rotation = initialRotation;
			Blink ();
		}

		if (!isTopCamera)
		{
			// Calculate the rotation angles
			float wantedAngle = rotationVector.y - initialCarRotationY;
			float currentAngle = transform.eulerAngles.y;
	
			// Damp the rotation around the y-axis
			if (cameraRoll == 0)
				currentAngle = Mathf.LerpAngle (currentAngle, wantedAngle, rotationDamping * Time.deltaTime);

			// Convert the angle into a rotation
			currentRotation = Quaternion.Euler (0, currentAngle, 0); 

			//Calculate the wanted height.
			float wantedHeight = car.position.y + height;
			currentHeight = transform.position.y;
	
			// Damp the height
			switch(cameraRoll)
			{
				//back of the car view
				case 0:
					currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
					break; 

				//steering wheel view
				case 1: 
					currentHeight = car.position.y + 1.8f;
					break;
					
					//over the board view
				/*case 2: 
					currentHeight = Mathf.Lerp (currentHeight, car.position.y + 1.5f, heightDamping * Time.deltaTime);
					break;			*/			
			}
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
			if (Input.GetKeyDown (KeyCode.C) || EmoExpressiv.isLeftWink) // if 'c' key is pressed, change camera position.
			{
				cameraRoll++;
				if (cameraRoll == 2) //if we reached a bigger index than the number of camera positions, reset to 0.
					cameraRoll = 0; 
			}

			switch(cameraRoll)
			{
				//back of the car view
				case 0:
					transform.position = car.position - currentRotation * Vector3.forward * distance; 	
					break; 

				//steering wheel view
				case 1: 
					transform.position = car.position - currentRotation * Vector3.forward * 1.5f - currentRotation * Vector3.right * 1.7f;
					break;
						
				//over the board view
				/*case 2: 
					transform.position = car.position - currentRotation * Vector3.back * 2.5f;
					break;	*/					
			}
		}
		
		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);

		// Always look at the car.
		if (cameraRoll == 0)
			transform.LookAt (car); 
		else
			transform.LookAt(emptyObjectInFrontOfCar);
	}

	void FixedUpdate() 
	{
		string currentAction = EmoCognitiv.getCurrentAction();
		if (!isTopCamera && cameraRoll == 0)
		{
			// If the car is in reverse, move the camera to the front of the car so we can see where we drive to.
			if (Input.GetAxis ("Vertical") < 0 || currentAction == "COG_PULL") {
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
