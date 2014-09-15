#pragma strict

var wheelFL : WheelCollider;
var wheelFR : WheelCollider;
var wheelBL : WheelCollider;
var wheelBR : WheelCollider;
var wheelFLTrans : Transform;
var wheelFRTrans : Transform;
var wheelBLTrans : Transform;
var wheelBRTrans : Transform;
var maxTorque : float = 50;
var lowestSteerAtSpeed : float = 50;
var lowSpeedSteerAngle : float = 10;
var highSpeedSteerAngle : float = 1;
var decelerationSpeed : float = 30;
var currentSpeed : float = 150;
var topSpeed : float = 150;
var maxReverseSpeed : float = 40;
var braked: boolean = false;
var maxBrakeTorque : float = 100;
private var mySidewayFriction : float;
private var myForwardFriction : float; 
private var slipSidewayFriction : float;
private var slipForwardFriction : float;


function Start () {
	rigidbody.centerOfMass.y = -0.9;
	rigidbody.centerOfMass.z = 0.5;
	SetValues();
}

function SetValues() {
	myForwardFriction = wheelBR.forwardFriction.stiffness;
	mySidewayFriction = wheelBR.sidewaysFriction.stiffness;
	slipForwardFriction = 0.04;
	slipSidewayFriction = 0.08;
}

function FixedUpdate () {
	Controle();
	HandBrake();
}

function Update() {
	wheelFLTrans.Rotate(0, 0, wheelFL.rpm / 60 * 360 * Time.deltaTime);
	wheelFRTrans.Rotate(0, 0, wheelFR.rpm / 60 * 360 * Time.deltaTime);
	wheelBLTrans.Rotate(0, 0, wheelBL.rpm / 60 * 360 * Time.deltaTime);
	wheelBRTrans.Rotate(0, 0, wheelBR.rpm / 60 * 360 * Time.deltaTime);
	wheelFRTrans.localEulerAngles.y = wheelFR.steerAngle - wheelFRTrans.localEulerAngles.x;
	wheelFLTrans.localEulerAngles.y = wheelFL.steerAngle - wheelFLTrans.localEulerAngles.x;
}

function Controle() {
	currentSpeed = 2*22/7*wheelBL.radius*wheelBL.rpm*60/1000;
	currentSpeed = Mathf.Round(currentSpeed);
	if (currentSpeed <= topSpeed && currentSpeed > -maxReverseSpeed && !braked){
		wheelBL.motorTorque = maxTorque * Input.GetAxis("Vertical");
		wheelBR.motorTorque = maxTorque * Input.GetAxis("Vertical");
	}
	else {
		wheelBL.motorTorque = 0;
		wheelBR.motorTorque = 0;
	}
	
	if (Input.GetButton("Vertical") == false) 
	{
		wheelBR.brakeTorque = decelerationSpeed;
		wheelBL.brakeTorque = decelerationSpeed;	
	}
	else 
	{
		wheelBR.brakeTorque = 0;
		wheelBL.brakeTorque = 0;
	}
	var speedFactor = rigidbody.velocity.magnitude/lowestSteerAtSpeed;
	var currentSteerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor);
	currentSteerAngle *= Input.GetAxis("Horizontal");
	wheelFL.steerAngle = 10 * Input.GetAxis("Horizontal");
	wheelFR.steerAngle = 10 * Input.GetAxis("Horizontal");
}

function HandBrake(){
	if (Input.GetButton("Jump"))
		braked = true;
	else
		braked = false;
	if (braked){
		wheelFR.brakeTorque = maxBrakeTorque;
		wheelFL.brakeTorque = maxBrakeTorque;
		wheelBR.motorTorque = 0;
		wheelBL.motorTorque = 0;
		SetSlip(slipForwardFriction, slipSidewayFriction);
	}
	else {
		if (rigidbody.velocity.magnitude > 1)
			wheelFR.brakeTorque = 0;
			wheelFL.brakeTorque = 0;
			SetSlip(myForwardFriction, mySidewayFriction);
		else
			SetSlip(1, 1);
	}
}

function SetSlip(currentForwardFriction : float, currentSidewayFriction : float){
	wheelBR.forwardFriction.stiffness = currentForwardFriction;
	wheelBL.forwardFriction.stiffness = currentForwardFriction;
	wheelBR.sidewaysFriction.stiffness = currentSidewayFriction;
	wheelBL.sidewaysFriction.stiffness = currentSidewayFriction;
}