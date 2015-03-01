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
	private float startCountdown = 3.0f;
	private bool hideLabel = false;
	private Vector3 originalSteeringWheelRotation;
	private float initialTime;
	private string currentAction;
	private float currentActionPower;
	private string controlBy;
	private bool showGameOver = false;
	private float totalTimeSeconds;
	private static bool gameOver = false;
	private static string playerName = "";
	private Quaternion originalRotation;
	private float torqueForMotor;
	private static int countCheckpoints = 0;
	private static bool saved = false;

	public static int coinCounter = 0;

	private const string topCogPrefix = "Top10Cog"; // For saving the top 10 scorers in Cognitiv mode.
	private const string topKeyPrefix = "Top10Key"; // For saving the top 10 scorers in Keyboard mode.

	// Use this for initialization
	void Start () 
	{
		initialTime = Time.time;
		chosenCar.setCenterOfMass (0, -2.3f, -0.5f);
		SetValues ();
		originalSteeringWheelRotation = chosenCar.SteeringWheel.transform.localEulerAngles; 
		gameOver = false;
		if (PlayerPrefs.HasKey("CurrentPlayerName"))
			playerName = PlayerPrefs.GetString ("CurrentPlayerName");
		originalRotation = chosenCar.transform.rotation;
		torqueForMotor = chosenCar.maxTorque;
		countCheckpoints = 0;
		coinCounter = 0;
		saved = false;
	}

	void SetValues()
	{
		forwardFriction = chosenCar.WheelBR.forwardFriction.stiffness;
		sidewayFriction = chosenCar.WheelBR.sidewaysFriction.stiffness;
		slipForwardFriction = 0.04f;
		slipSidewayFriction = 0.05f;
	}
	
	void OnCollisionStay(Collision hit){
		if (hit.gameObject.tag == "Road")
		{
			torqueForMotor = chosenCar.maxTorque;
		}
		else
		{
			torqueForMotor = chosenCar.maxTorque / 4;
			chosenCar.WheelBR.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBL.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBR.motorTorque = 0;
			chosenCar.WheelBL.motorTorque = 0;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Checkpoint" + (countCheckpoints + 1))
		{
			countCheckpoints++;
			Debug.Log ("Reached " + countCheckpoints);
		}
	}

	void CarUpdateFromKeyboard()
	{
		if (currentSpeed < chosenCar.topSpeed && !braked) 
		{
			chosenCar.WheelBR.motorTorque = torqueForMotor * Input.GetAxis ("Vertical");
			chosenCar.WheelBL.motorTorque = torqueForMotor * Input.GetAxis ("Vertical");
		} else {
			chosenCar.WheelBR.motorTorque = 0;
			chosenCar.WheelBL.motorTorque = 0;
		}

		//If the car is in reverse and the current speed exceeds the maxReverseSpeed, apply brakes to slow down.
		if (Input.GetAxis ("Vertical") < 0 && (currentSpeed > chosenCar.maxReverseSpeed) && !braked) {				
			chosenCar.WheelBR.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBL.brakeTorque = chosenCar.topSpeed;
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
	}

	void CarUpdateFromCognitiv()
	{
		float multiplyBy = 1;
		float multiplyByActionPower = currentActionPower; 
		if (currentAction == "COG_PULL")
			multiplyBy = -1;
		if (currentActionPower > 0.5f)
			multiplyByActionPower = 1;
		
		if (currentSpeed < chosenCar.topSpeed && !braked) 
		{
			if (currentAction == "COG_PUSH" || currentAction == "COG_PULL")
			{
				chosenCar.WheelBR.motorTorque = torqueForMotor * multiplyByActionPower * multiplyBy;
				chosenCar.WheelBL.motorTorque = torqueForMotor * multiplyByActionPower * multiplyBy;
			}
		} else {
			chosenCar.WheelBR.motorTorque = 0;
			chosenCar.WheelBL.motorTorque = 0;
		}

		//If the car is in reverse and the current speed exceeds the maxReverseSpeed, apply brakes to slow down.
		if (currentAction == "COG_PULL" && (currentSpeed > chosenCar.maxReverseSpeed) && !braked) {				
			chosenCar.WheelBR.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBL.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBR.motorTorque = 0;
			chosenCar.WheelBL.motorTorque = 0;
		}
		
		//If no vertical button is pressed, decelerate speed by increasing brakeTorque. 
		//Wait 3 seconds before deceleration to compensate for the lack of the ability to do two action at the same time.
		if (currentAction != "COG_PUSH" && currentAction != "COG_PULL") {
			Wait3Seconds();
			chosenCar.WheelBR.brakeTorque = chosenCar.decelerationSpeed;
			chosenCar.WheelBL.brakeTorque = chosenCar.decelerationSpeed;
		} else {
			chosenCar.WheelBR.brakeTorque = 0;
			chosenCar.WheelBL.brakeTorque = 0;
		}
	}

	IEnumerator Wait3Seconds() {
		yield return new WaitForSeconds(3);
	}

	void CarUpdateFromGyro()
	{
		int posX = EmoGyroData.GyroX;
		int posY = EmoGyroData.GyroY;
		Status headPosition = EmoGyroData.headPosition; //Center, Left, Right, Up, Down, Deny
		
		Debug.Log("posX: " + posX + " posY: " + posY + " headPosition: " + headPosition);		

		if (currentSpeed < chosenCar.topSpeed && !braked)
		{
			//if (headPosition == Status.Up || headPosition == Status.Down || headPosition == Status.Center) 
			//{
				chosenCar.WheelBR.motorTorque = torqueForMotor * posY / 180;
				chosenCar.WheelBL.motorTorque = torqueForMotor * posY / 180;
		//	} else {
		//		chosenCar.WheelBR.motorTorque = 0;
		//		chosenCar.WheelBL.motorTorque = 0;
		//	}
		}
		
		//If the car is in reverse and the current speed exceeds the maxReverseSpeed, apply brakes to slow down.
		if (/*headPosition == Status.Down && */(currentSpeed > chosenCar.maxReverseSpeed) && !braked) {				
			chosenCar.WheelBR.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBL.brakeTorque = chosenCar.topSpeed;
			chosenCar.WheelBR.motorTorque = 0;
			chosenCar.WheelBL.motorTorque = 0;
		}
		
		//If no vertical button is pressed, decelerate speed by increasing brakeTorque.
		if (!((headPosition == Status.Up) || (headPosition == Status.Down) || (headPosition == Status.Center))) {
			chosenCar.WheelBR.brakeTorque = chosenCar.decelerationSpeed;
			chosenCar.WheelBL.brakeTorque = chosenCar.decelerationSpeed;
		} else {
			chosenCar.WheelBR.brakeTorque = 0;
			chosenCar.WheelBL.brakeTorque = 0;
		}
	}
	
	// FixedUpdate is called multiple times per frame
	void FixedUpdate () 
	{
		currentAction = EmoCognitiv.getCurrentAction();
		currentActionPower = EmoCognitiv.getCurrentActionPower();
		controlBy = MainMenuScript.selectedControlTool;
		float multiplyBy = 1;
		
		if ( Mathf.Round(countdown) <= 0)
		{
			currentSpeed = rigidbody.velocity.magnitude;
			currentSpeed = Mathf.Round (currentSpeed);
	
			//By making motorTorque = 0, we achieve same effect as when the user is not pressing any vertical keys.
			//That means the car will begin to decelerate.
	
			//If current speed is less than the maximum speed achievable by the car and we are not in a braked state,
			// multiply the current speed by the maxTorque constant.

			if (controlBy == "Keyboard")
				CarUpdateFromKeyboard();	
			else if (controlBy == "Cognitiv")
				CarUpdateFromCognitiv();
			else if (controlBy == "Gyro")
				CarUpdateFromGyro();

			//Deal with car steering by rotating the front wheels a certain degree.
			float currentSteerAngle = Mathf.Lerp (chosenCar.lowSpeedSteerAngle, chosenCar.highSpeedSteerAngle, currentSpeed);
		
			if (controlBy == "Keyboard")
				currentSteerAngle *= Input.GetAxis ("Horizontal");
			else if (controlBy == "Cognitiv")
			{
				if (currentAction == "COG_LEFT")
					multiplyBy = -1;
				if (currentAction == "COG_LEFT" || currentAction == "COG_RIGHT")
					currentSteerAngle *= currentActionPower * multiplyBy;
				else 
					currentSteerAngle = 0;
			}
			else if (controlBy == "Gyro")
			{
				Status headPosition = EmoGyroData.headPosition; //Center, Left, Right, Up, Down, Deny
				if (headPosition == Status.Left || headPosition == Status.Right || headPosition == Status.Center) 
					currentSteerAngle *= EmoGyroData.GyroX / 180;
				else
					currentSteerAngle = 0;
			}

			chosenCar.WheelFL.steerAngle = currentSteerAngle;
			chosenCar.WheelFR.steerAngle = currentSteerAngle;
			
			if (gameOver == false)
				StopAfterFinish();

			//If the car has reached the finish sign, slow the car down until stop.
			if (transform.position.x >= 1850 && transform.position.z > 1770)
			{
				chosenCar.WheelBR.motorTorque = 0;
				chosenCar.WheelBL.motorTorque = 0;
				chosenCar.WheelBR.brakeTorque = chosenCar.topSpeed;
				chosenCar.WheelBL.brakeTorque = chosenCar.topSpeed;
			}

			HandBrake ();

			SteeringWheel(currentSteerAngle);
		}
	}

	//Update is called once per frame.
	void Update() 
	{	
		//Restart game if R is pressed.
		if (Input.GetKeyDown (KeyCode.R))
		{
			chosenCar.transform.position = new Vector3(1729.277f, 265.7542f, 11.81225f);
			chosenCar.transform.rotation = originalRotation;		
			initialTime = Time.time;
			countCheckpoints = 0;
			coinCounter = 0;
		}
		
		if (Input.GetKeyDown (KeyCode.T))
		{
			GameObject checkpoint = GameObject.FindWithTag("Checkpoint" + countCheckpoints);
			chosenCar.transform.position = checkpoint.transform.position;
		}

		countdown -= Time.deltaTime;				
	
		//Wheel rotation while the car is moving.
		chosenCar.WheelFLTransform.Rotate (chosenCar.WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		chosenCar.WheelFRTransform.Rotate (chosenCar.WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		chosenCar.WheelBLTransform.Rotate (chosenCar.WheelBL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
		chosenCar.WheelBRTransform.Rotate (chosenCar.WheelBR.rpm / 60 * 360 * Time.deltaTime, 0, 0);

		//Front wheels might be already rotated due to the steering.
		int wheelRotation = 0;
		if (PlayerPrefs.GetString("CarModel") == "Audi")
			wheelRotation = 90;
		
		chosenCar.WheelFLTransform.localEulerAngles = new Vector3 (chosenCar.WheelFLTransform.localEulerAngles.x, 
		                                                           chosenCar.WheelFL.steerAngle - chosenCar.WheelFLTransform.localEulerAngles.z + wheelRotation,
		                                                           chosenCar.WheelFLTransform.localEulerAngles.z); 
		chosenCar.WheelFRTransform.localEulerAngles = new Vector3 (chosenCar.WheelFRTransform.localEulerAngles.x,
		                                                 chosenCar.WheelFR.steerAngle - chosenCar.WheelFRTransform.localEulerAngles.z + wheelRotation,
		                                                 chosenCar.WheelFRTransform.localEulerAngles.z);	
		
		EngineSound ();
		BackLights ();
	}

	//Method to deal with the backlights of a car in brake, reverse or idle states.
	void BackLights() 
	{
		if (chosenCar.backLightObject.renderer)
		{
			if (currentSpeed > 0 && (Input.GetAxis ("Vertical") < 0 || currentAction == "COG_PULL") && !braked)
				//brake light
				chosenCar.backLightObject.renderer.material.color = new Color(0.973f, 0.016f, 0, 1);
			else if (currentSpeed < 0 && (Input.GetAxis ("Vertical") > 0 || currentAction == "COG_PUSH") && !braked)
				//brake light
				chosenCar.backLightObject.renderer.material.color = new Color(0.973f, 0.016f, 0, 1);
			else if (currentSpeed < 0 && (Input.GetAxis ("Vertical") < 0 || currentAction == "COG_PULL") && !braked)
				//reverse
				chosenCar.backLightObject.renderer.material.color = new Color(0.671f, 0.667f, 0.686f, 1);
			else if (!braked)
				//idle
				chosenCar.backLightObject.renderer.material.color = new Color(0.424f, 0.055f, 0.047f, 1);
		}
	}

	void SteeringWheel(float currentSteerAngle)
	{
		if (chosenCar.SteeringWheel.renderer)
		{
			//Rotate the steering wheel.
			chosenCar.SteeringWheel.transform.Rotate(0, 0, (-90) / currentSteerAngle / 1.5f * Time.deltaTime);			
			
			//Turn the steering wheel to the initial position if the left/right keys are released.
			Vector3 currentSteeringWheelRotation = chosenCar.SteeringWheel.transform.localEulerAngles;
			if ((!Input.GetButton ("Horizontal") || (currentAction != "COG_LEFT" && currentAction != "COG_RIGHT")) && currentSteeringWheelRotation != Vector3.zero)
				chosenCar.SteeringWheel.transform.localEulerAngles = originalSteeringWheelRotation;
		}
	}

	void StopAfterFinish()
	{
		//If the car has reached the finish sign, stop.
		if (transform.position.x >= 1850 && transform.position.z > 1770)
		{
			showGameOver = true;
		}
	}
	
	void SaveHighscores()
	{
		if (gameOver && !saved)
		{	
			//Check game play mode.
			if (controlBy == "Keyboard")
			{
				//Increase the Top10KeyCount if < 10.
				int topKeyCount = 0;
				if (PlayerPrefs.HasKey("Top10KeyCount"))
					topKeyCount = PlayerPrefs.GetInt("Top10KeyCount"); 
				
				//If  Top10KeyCount > 10, check if current time is better. If it is, record it.
				//A top 10 value is stored as: key = topKeyPrefix; value = String(PlayerName;elapsedTimeInSeconds;coins).
				int[] topKeyTime = new int[topKeyCount];
				int[] topKeyCoins = new int[topKeyCount];
				string[] topKeyValues = new string[topKeyCount];
				int insertPosition = topKeyCount;
				totalTimeSeconds = Mathf.RoundToInt(totalTimeSeconds);
				for (int i = 0; i < topKeyCount; i++)
				{
					topKeyValues[i] = PlayerPrefs.GetString(topKeyPrefix + i);
					topKeyTime[i] = int.Parse(PlayerPrefs.GetString(topKeyPrefix + i).Split(';')[1]);
					topKeyCoins[i] = int.Parse (PlayerPrefs.GetString(topKeyPrefix + i).Split(';')[2]);
				}
				for (int i = 0; i < topKeyCount; i++)
				{
					if ((totalTimeSeconds / coinCounter) < (topKeyTime[i] / topKeyCoins[i]))
					{ 
						insertPosition = i;		
						break;
					}
					if ((totalTimeSeconds / coinCounter) >= (topKeyTime[i] / topKeyCoins[i]) && topKeyCount < 10)
						insertPosition = topKeyCount;
				}
	
				//Update all the current top values and insert the new one.
				int j = 1;
				while (j <= topKeyCount - insertPosition && topKeyCount != 0)
				{
					PlayerPrefs.SetString(topKeyPrefix + (topKeyCount + 1 - j), topKeyValues[topKeyCount-j]);
					j++;
				}
				
				PlayerPrefs.SetString(topKeyPrefix + insertPosition, playerName + ";" + totalTimeSeconds + ";" + coinCounter);
	
				//Save new value for topKeyCount.
				if (topKeyCount < 10)
					topKeyCount++;
				PlayerPrefs.SetInt("Top10KeyCount", topKeyCount);
			}		
			else if (controlBy == "Cognitiv")
			{
				//Increase the Top10CogCount if < 10.
				int topCogCount = 0;
				if (PlayerPrefs.HasKey("Top10CogCount"))
					topCogCount = PlayerPrefs.GetInt("Top10CogCount"); 
				
				//If  Top10CogCount > 10, check if current time is better. If it is, record it.
				//A top 10 value is stored as: key = topCogPrefix; value = String(PlayerName;elapsedTimeInSeconds).
				int[] topCogTime = new int[topCogCount];
				int[] topCogCoins = new int[topCogCount];
				string[] topCogValues = new string[topCogCount];
				int insertPosition = topCogCount;
				totalTimeSeconds = Mathf.RoundToInt(totalTimeSeconds);
				for (int i = 0; i < topCogCount; i++)
				{
					topCogValues[i] = PlayerPrefs.GetString(topCogPrefix + i);
					topCogTime[i] = int.Parse(PlayerPrefs.GetString(topCogPrefix + i).Split(';')[1]);
					topCogCoins[i] = int.Parse(PlayerPrefs.GetString(topCogPrefix + i).Split(';')[2]);
				}
				for (int i = 0; i < topCogCount; i++)
				{
					if ((totalTimeSeconds / coinCounter) < (topCogTime[i] / topCogCoins[i]))
					{ 
						insertPosition = i;		
						break;
					}
					if ((totalTimeSeconds / coinCounter) >= (topCogTime[i] / topCogCoins[i]) && topCogCount < 10)
						insertPosition = topCogCount;
				}
				
				//Update all the current top values and insert the new one.
				int j = 1;
				while (j <= topCogCount - insertPosition && topCogCount != 0)
				{
					PlayerPrefs.SetString(topCogPrefix + (topCogCount + 1 - j), topCogValues[topCogCount-j]);
					j++;
				}
				
				PlayerPrefs.SetString(topCogPrefix + insertPosition, playerName + ";" + totalTimeSeconds + ";" + coinCounter);
				
				//Save new value for topCogCount.
				if (topCogCount < 10)
					topCogCount++;
				PlayerPrefs.SetInt("Top10CogCount", topCogCount);
	
			}
	
			//Save PlayerPrefs. 	
			PlayerPrefs.Save ();

			saved = true;
		}
	}

	void HandBrake() 
	{
		if (Input.GetButton ("Jump") || (EmoExpressiv.clenchExtent > 0)) //spacebar
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

	void SetSlip(float currentForwardFriction, float currentSidewayFriction) 
	{
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

	void EngineSound() 
	{
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
			
			if (countdown <= startCountdown && countdown > 0.5f)
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
		Texture2D myTexture = new Texture2D(1, 1);
		Color grey = new Color(0.5f, 0.5f, 0.5f);
		myTexture.SetPixel(1, 1, grey);
		myTexture.Apply();
		GUI.skin.box.normal.background = myTexture; 	
		GUI.Box (new Rect (Screen.width - 110, Screen.height - 30, 60, 25), currentSpeed.ToString() + " km/h");

		float speedFactor = currentSpeed / chosenCar.topSpeed ;

		//Calculate the rotation angle of the speedometer needle.
		float rotationAngle = Mathf.Lerp (0, 252, speedFactor);

		//Calculate the elapsed time since the start of the game and display it in the right hand side corner.
		totalTimeSeconds = Time.time - startCountdown - initialTime;
		if (totalTimeSeconds > 0)
		{
			float elapsedTimeMinutes = Mathf.Floor (totalTimeSeconds / 60);
			float elapsedTimeSeconds = Mathf.Round(totalTimeSeconds - elapsedTimeMinutes * 60);
			GUI.Label (new Rect(Screen.width - 280, 0, 360, 25), "<color=orange>Elapsed Time: " + string.Format("{0:00}:{1:00}", elapsedTimeMinutes, elapsedTimeSeconds) + "</color>");
			GUI.Label (new Rect(Screen.width - 280, 20, 360, 25), "<color=orange>Coins: " + coinCounter + "</color>");
		}

		//Rotate and draw the speedometer needle.
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 80, Screen.height - 49));
		GUI.DrawTexture (new Rect (Screen.width - 208, Screen.height - 155, 250, 250), speedometerNeedle);

		if (showGameOver)
		{
			Rect exitWindow = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 40, 400, 80);
			exitWindow = GUILayout.Window(8, exitWindow, DoGameOverAction, "Game Over");
		}
	}

	void DoGameOverAction(int windowID)
	{
		GUILayout.Space (2);
		
		//Get label and button style
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");		
		
		//Set alignment to center, fix the button width and set image label.
		labelStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.fixedWidth = 80;
		
		if (countCheckpoints == 17)
		{
			gameOver = true;
			SaveHighscores();
			GUILayout.Label("Congratulations! You have reached the end of the game!");
		}
		else 
		{
			GUILayout.Label ("Oh no! You tried to cheat by not reaching all checkpoints!");	
			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Restart", buttonStyle))
			{
				chosenCar.transform.position = new Vector3(1729.277f, 265.7542f, 11.81225f);
				chosenCar.transform.rotation = originalRotation;		
				initialTime = Time.time;
				showGameOver = false;
				countCheckpoints = 0;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		}	

		GUILayout.Space (5);
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Main Menu", buttonStyle))
		{
			Application.LoadLevel (1);			
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

} //class
