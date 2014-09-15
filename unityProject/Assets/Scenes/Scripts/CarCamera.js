#pragma strict

var car : Transform;
var distance : float = 9.4;
var height : float = 2.4;
var rotationDamping : float = 2.0;
var heightDamping : float = 2.0;
var zoomRatio : float = 0.7;
var defaultFOV : float = 70;
private var rotationVector : Vector3;

function Start () {

}

function LateUpdate () {
	var wantedAngle = rotationVector.x;
	var wantedHeight = car.position.x + height;
	var myAngle = transform.eulerAngles.x;
	var myHeight = transform.position.x;
	myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.deltaTime);
	myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.deltaTime);
	var currentRotation = Quaternion.Euler(0, myAngle, 0);
	transform.position = car.position;
	transform.position -= currentRotation * Vector3.forward * distance;
	transform.position.x = myHeight;
	transform.LookAt(car); 
}

function FixedUpdate() {
	var localVelocity = car.InverseTransformDirection(car.rigidbody.velocity);
	if (localVelocity.y < -0.5)
		rotationVector.x = car.eulerAngles.x + 180;
	else
		rotationVector.x = car.eulerAngles.x;
		
	var acc = car.rigidbody.velocity.magnitude;
	camera.fieldOfView = defaultFOV + acc * zoomRatio;
}