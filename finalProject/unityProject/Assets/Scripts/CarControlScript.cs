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
	public float decelerationSpeed = 500;
	public float currentSpeed;
	public float topSpeed = 320;
	public float maxReverseSpeed = 50;
	public GameObject backLightObject;
	private bool braked = false;
	public float maxBrakeTorque = 100;
	private float sidewayFriction;
	private float forwardFriction;
	private float slipForwardFriction;
	private float slipSidewayFriction;
	public int[] gearRatio;
	public Texture2D speedometerDial;
	public Texture2D speedometerNeedle;

	// Use this for initialization
	void Start () {
		Vector3 temp = rigidbody.centerOfMass;
		temp.y += -2.5f;
		rigidbody.centerOfMass = temp;
		SetValues ();
	}

	void SetValues(){
		forwardFriction = WheelBR.forwardFriction.stiffness;
		sidewayFriction = WheelBR.sidewaysFriction.stiffness;
		slipForwardFriction = 0.04f;
		slipSidewayFriction = 0.08f;
	}

	// FixedUpdate is called multiple times per frame
	void FixedUpdate () {
		currentSpeed = rigidbody.velocity.magnitude;
		currentSpeed = Mathf.Round (currentSpeed);
		if (currentSpeed < topSpeed && !braked) {
			WheelBR.motorTorque = maxTorque * Input.GetAxis ("Vertical");
			WheelBL.motorTorque = maxTorque * Input.GetAxis ("Vertical");
		} else {
			WheelBR.motorTorque = 0;
			WheelBL.motorTorque = 0;
		}
		if ((Input.GetAxis ("Vertical") < 0) && (currentSpeed > maxReverseSpeed) && !braked) {
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
		HandBrake ();

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
		BackLights ();
		WheelPosition ();
		EngineSound ();
	}

	void BackLights() {
		if (currentSpeed > 0 && Input.GetAxis ("Vertical") < 0 && !braked)
			//brake light
			backLightObject.renderer.material.color = new Color(248, 4, 0, 1);
		else if (currentSpeed < 0 && Input.GetAxis ("Vertical") > 0 && !braked)
			//brake light
			backLightObject.renderer.material.color = new Color(248, 4, 0, 1);
		else if (currentSpeed < 0 && Input.GetAxis ("Vertical") < 0 && !braked)
			//reverse
			backLightObject.renderer.material.color = new Color(171, 170, 175, 1);
		else if (!braked)
			//idle
			backLightObject.renderer.material.color = new Color(108, 4, 11, 1);
	}

	void WheelPosition() {
		RaycastHit hit;
		Vector3 wheelPosition;
		WheelCollider[] wheelColliders = new WheelCollider[]{WheelFL, WheelFR, WheelBL, WheelBR};
		Transform[] wheels = new Transform[]{WheelFLTransform, WheelFRTransform, WheelBLTransform, WheelBRTransform};
		for (int i = 0; i < wheelColliders.Length; i++) {
			if (Physics.Raycast (wheelColliders[i].transform.position, -wheelColliders[i].transform.up, out hit, wheelColliders[i].radius + wheelColliders[i].suspensionDistance)) 
				wheelPosition = hit.point + wheelColliders[i].transform.up * wheelColliders[i].radius;
			else
				wheelPosition = wheelColliders[i].transform.position - wheelColliders[i].transform.up * wheelColliders[i].suspensionDistance;
			wheels[i].position = wheelPosition;
		}
	}

	void HandBrake() {
		if (Input.GetButton ("Jump"))
			braked = true;
		else
			braked = false;

		if (braked) {
			WheelFR.brakeTorque = maxBrakeTorque;
			WheelFL.brakeTorque = maxBrakeTorque;
			WheelBR.motorTorque = 0;
			WheelBL.motorTorque = 0;
			if (currentSpeed > 1)
					SetSlip (slipForwardFriction, slipSidewayFriction);
			else
					SetSlip (1, 1);

			if (currentSpeed < 5)
				//idle
				backLightObject.renderer.material.color = new Color (108, 4, 11, 1);
			else
				//brake light
				backLightObject.renderer.material.color = new Color (248, 4, 0, 1);
			} else {
				WheelFR.brakeTorque = 0;
				WheelFL.brakeTorque = 0;
				SetSlip (forwardFriction, sidewayFriction);
			}
	}

	void SetSlip(float currentForwardFriction, float currentSidewayFriction) {
		var temp = WheelBR.forwardFriction;
		temp.stiffness = currentForwardFriction;
		WheelBR.forwardFriction = temp;

		temp = WheelBL.forwardFriction;
		temp.stiffness = currentForwardFriction;
		WheelBL.forwardFriction = temp;

		temp = WheelBR.sidewaysFriction;
		temp.stiffness = currentSidewayFriction;
		WheelBR.sidewaysFriction = temp;

		temp = WheelBL.forwardFriction;
		temp.stiffness = currentSidewayFriction;
		WheelBL.sidewaysFriction = temp;
	}

	void EngineSound() {
		int i;
		for (i = 0; i < gearRatio.Length; i++) {
			if (gearRatio[i] > currentSpeed){
				break;
			}
		}
		float gearMinValue = 0.00f;
		float gearMaxValue = 0.00f;
		if (i == 0) 
			gearMinValue = 0;
		else
			gearMinValue = gearRatio[i-1];
		gearMaxValue = gearRatio [i];

		float enginePitch = ((currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue)) + 1;
		audio.pitch = enginePitch;
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (Screen.width - 200, Screen.height - 125, 250, 125), speedometerDial);
		float speedFactor = currentSpeed / topSpeed;
		float rotationAngle = Mathf.Lerp (0, 252, speedFactor);
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 80, Screen.height - 49));
		GUI.DrawTexture (new Rect (Screen.width - 208, Screen.height - 155, 250, 250), speedometerNeedle);

	}

}
