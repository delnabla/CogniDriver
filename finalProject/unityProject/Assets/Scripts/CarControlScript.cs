using UnityEngine;
using System.Collections;

public class CarControlScript : MonoBehaviour {
	
	private bool braked = false;
	public float currentSpeed;
	private float sidewayFriction;
	private float forwardFriction;
	private float slipForwardFriction;
	private float slipSidewayFriction;
	public Texture2D speedometerDial;
	public Texture2D speedometerNeedle;
	public Car chosenCar;
	private float countdown = 3.0f;
	private bool hideLabel = false;

	// Use this for initialization
	void Start () {
		chosenCar.setCenterOfMass (0, -2.3f, -0.5f);
		SetValues ();
	}

	void SetValues(){
		forwardFriction = chosenCar.WheelBR.forwardFriction.stiffness;
		sidewayFriction = chosenCar.WheelBR.sidewaysFriction.stiffness;
		slipForwardFriction = 0.04f;
		slipSidewayFriction = 0.5f;
	}

	
	// FixedUpdate is called multiple times per frame
	void FixedUpdate () {
		if ( Mathf.Round(countdown) <= 0)
		{
			currentSpeed = rigidbody.velocity.magnitude;
			currentSpeed = Mathf.Round (currentSpeed);
	
			//By making motorTorque = 0, we achieve same effect as when the user is not pressing any vertical keys.
			//That means the car will begin to decelerate.
	
			//If current speed is less than the maximum speed achievable by the car and we are not in a braked state,
			// multiply the current speed by the maxTorque constant.
			if (currentSpeed < chosenCar.topSpeed && !braked) {
				chosenCar.WheelBR.motorTorque = chosenCar.maxTorque * Input.GetAxis ("Vertical");
				chosenCar.WheelBL.motorTorque = chosenCar.maxTorque * Input.GetAxis ("Vertical");
			} else {
				chosenCar.WheelBR.motorTorque = 0;
				chosenCar.WheelBL.motorTorque = 0;
			}
	
			//If the car is in reverse and the current speed exceeds the maxReverseSpeed, make motorTorque 0.
			if ((Input.GetAxis ("Vertical") < 0) && (currentSpeed > chosenCar.maxReverseSpeed) && !braked) {
				chosenCar.WheelBR.motorTorque = 0;
				chosenCar.WheelBL.motorTorque = 0;
			}
	
			//If no vertical button is pressed, decelerate speed by increasing brakeTorque.
			if (!Input.GetButton ("Vertical")) {
				chosenCar.WheelBR.brakeTorque = chosenCar.decelerationSpeed;
				chosenCar.WheelBL.brakeTorque = chosenCar.decelerationSpeed;
			} else {
				chosenCar.WheelBR.brakeTorque = 0;
				chosenCar.WheelBL.brakeTorque = 0;
			}

			//Deal with car steering by rotating the front wheels a certain degree.
			float currentSteerAngle = Mathf.Lerp (chosenCar.lowSpeedSteerAngle, chosenCar.highSpeedSteerAngle, currentSpeed);
			currentSteerAngle *= Input.GetAxis ("Horizontal");
			chosenCar.WheelFL.steerAngle = 10 * Input.GetAxis("Horizontal");
			chosenCar.WheelFR.steerAngle = 10 * Input.GetAxis("Horizontal");

			//If the car has reached the finish sign, stop.
			if (transform.position.x >= 1850 && transform.position.z > 1770)
			{
				chosenCar.WheelBR.motorTorque = 0;
				chosenCar.WheelBL.motorTorque = 0;
				chosenCar.WheelBR.brakeTorque = chosenCar.topSpeed/2;
				chosenCar.WheelBL.brakeTorque = chosenCar.topSpeed/2;
			} 	

			HandBrake ();
		}
	}

	//Update is called once per frame.
	void Update() {	
		
		countdown -= Time.deltaTime;				
	
		//Wheel rotation while the car is moving.
		chosenCar.WheelFLTransform.Rotate (chosenCar.WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		chosenCar.WheelFRTransform.Rotate (chosenCar.WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		chosenCar.WheelBLTransform.Rotate (chosenCar.WheelBL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		chosenCar.WheelBRTransform.Rotate (chosenCar.WheelBR.rpm / 60 * 360 * Time.deltaTime, 0, 0);

		//Front wheels might be already rotated due to the steering.
		chosenCar.WheelFLTransform.localEulerAngles = new Vector3 (chosenCar.WheelFLTransform.localEulerAngles.x, 
		                                                           chosenCar.WheelFL.steerAngle - chosenCar.WheelFLTransform.localEulerAngles.z + 90,
		                                                           chosenCar.WheelFLTransform.localEulerAngles.z); 
		chosenCar.WheelFRTransform.localEulerAngles = new Vector3 (chosenCar.WheelFRTransform.localEulerAngles.x,
		                                                 chosenCar.WheelFR.steerAngle - chosenCar.WheelFRTransform.localEulerAngles.z + 90,
		                                                 chosenCar.WheelFRTransform.localEulerAngles.z);	
 
		BackLights ();
		WheelPosition ();
		EngineSound ();
	}

	//Method to deal with the backlights of a car in brake, reverse or idle states.
	void BackLights() {
		if (currentSpeed > 0 && Input.GetAxis ("Vertical") < 0 && !braked)
			//brake light
			chosenCar.backLightObject.renderer.material.color = new Color(248, 4, 0, 1);
		else if (currentSpeed < 0 && Input.GetAxis ("Vertical") > 0 && !braked)
			//brake light
			chosenCar.backLightObject.renderer.material.color = new Color(248, 4, 0, 1);
		else if (currentSpeed < 0 && Input.GetAxis ("Vertical") < 0 && !braked)
			//reverse
			chosenCar.backLightObject.renderer.material.color = new Color(171, 170, 175, 1);
		else if (!braked)
			//idle
			chosenCar.backLightObject.renderer.material.color = new Color(108, 4, 11, 1);
	}

	void WheelPosition() {
		RaycastHit hit;
		WheelCollider[] wheelColliders = new WheelCollider[]{chosenCar.WheelFL, chosenCar.WheelFR, chosenCar.WheelBL, chosenCar.WheelBR};
		Transform[] wheels = new Transform[]{chosenCar.WheelFLTransform, chosenCar.WheelFRTransform, chosenCar.WheelBLTransform, chosenCar.WheelBRTransform};
		for (int i = 0; i < wheelColliders.Length; i++) {
			//Cast a ray starting against the collider and return information on what was hit. Then modify the wheel position to simulate suspension.
			if (Physics.Raycast (wheelColliders[i].transform.position, -wheelColliders[i].transform.up, out hit, wheelColliders[i].radius + wheelColliders[i].suspensionDistance)) 
				wheels[i].position = hit.point + wheelColliders[i].transform.up * wheelColliders[i].radius;
			else
				wheels[i].position = wheelColliders[i].transform.position - wheelColliders[i].transform.up * wheelColliders[i].suspensionDistance;
		}
	}

	void HandBrake() {
		if (Input.GetButton ("Jump")) //spacebar
			braked = true;
		else
			braked = false;

		if (braked) {
			chosenCar.WheelFR.brakeTorque = chosenCar.maxBrakeTorque;
			chosenCar.WheelFL.brakeTorque = chosenCar.maxBrakeTorque;
			chosenCar.WheelBR.motorTorque = 0;
			chosenCar.WheelBL.motorTorque = 0;
			if (currentSpeed > 1)
					SetSlip (slipForwardFriction, slipSidewayFriction);
			else
					SetSlip (1, 1);

			if (currentSpeed < 5)
				//idle
				chosenCar.backLightObject.renderer.material.color = new Color (108, 4, 11, 1);
			else
				//brake light
				chosenCar.backLightObject.renderer.material.color = new Color (248, 4, 0, 1);
			} else {
				chosenCar.WheelFR.brakeTorque = 0;
				chosenCar.WheelFL.brakeTorque = 0;
				SetSlip (forwardFriction, sidewayFriction);
			}
	}

	void SetSlip(float currentForwardFriction, float currentSidewayFriction) {
		var temp = chosenCar.WheelBR.forwardFriction;
		temp.stiffness = currentForwardFriction;
		chosenCar.WheelBR.forwardFriction = temp;

		temp = chosenCar.WheelBL.forwardFriction;
		temp.stiffness = currentForwardFriction;
		chosenCar.WheelBL.forwardFriction = temp;

		temp = chosenCar.WheelBR.sidewaysFriction;
		temp.stiffness = currentSidewayFriction;
		chosenCar.WheelBR.sidewaysFriction = temp;

		temp = chosenCar.WheelBL.forwardFriction;
		temp.stiffness = currentSidewayFriction;
		chosenCar.WheelBL.sidewaysFriction = temp;
	}

	void EngineSound() {
		int i;
		for (i = 0; i < chosenCar.gearRatio.Length; i++) {
			if (chosenCar.gearRatio[i] > currentSpeed){
				break;
			}
		}
		float gearMinValue = 0.00f;
		float gearMaxValue = 0.00f;
		if (i == 0) 
			gearMinValue = 0;
		else
			gearMinValue = chosenCar.gearRatio[i-1];
		gearMaxValue = chosenCar.gearRatio [i];

		float enginePitch = ((currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue)) + 1;
		audio.pitch = enginePitch;
	}

	void OnGUI()
	{
		if (!hideLabel)
		{
			//Store backup values for the text style.
			int backUpLabelFontSize = GUI.skin.label.fontSize;
			
			//Update skin values to desired ones.
			GUI.skin.label.fontSize = 64;	
			GUI.skin.label.clipping = TextClipping.Overflow;	
			
			if (countdown <= 3 && countdown > 0.5f)
				DrawOutline.DrawTheOutline (new Rect (Screen.width / 2 - 10, Screen.height / 2 - 45, 125, 25), Mathf.Round(countdown).ToString(), GUI.skin.label, Color.black, Color.white, 4);
			if (Mathf.Round(countdown) == 0)
				DrawOutline.DrawTheOutline (new Rect (Screen.width / 2 - 10, Screen.height / 2 - 45, 125, 25), "GO!", GUI.skin.label, Color.black, Color.white, 4);
			
			if (countdown < 0)
				hideLabel = true;
	
			//Restore to previous skin values.
			GUI.skin.label.fontSize = backUpLabelFontSize;
		}

		// Draw the speedometer dial.
		GUI.DrawTexture (new Rect (Screen.width - 200, Screen.height - 125, 250, 125), speedometerDial);

		//Draw the box which gives the digital reading of the current speed.
		GUI.Box (new Rect (Screen.width - 110, Screen.height - 30, 60, 25), currentSpeed.ToString() + " km/h");

		float speedFactor = currentSpeed / chosenCar.topSpeed;

		//Calculate the rotation angle of the speedometer needle.
		float rotationAngle = Mathf.Lerp (0, 252, speedFactor);

		//Calculate the elapset time since the start of the game and display it in the right hand side corner.
		float elapsedTimeSeconds = Time.time;
		float elapsedTimeMinutes = Mathf.Floor (elapsedTimeSeconds / 60);
		elapsedTimeSeconds = Mathf.Round(elapsedTimeSeconds - elapsedTimeMinutes * 60);
		GUI.Label (new Rect(Screen.width - 130, 0, 360, 25), "<color=orange>Elapsed Time: " + string.Format("{0:00}:{1:00}", elapsedTimeMinutes, elapsedTimeSeconds) + "</color>");

		//Rotate and draw the speedometer needle.
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 80, Screen.height - 49));
		GUI.DrawTexture (new Rect (Screen.width - 208, Screen.height - 155, 250, 250), speedometerNeedle);

	}

}
