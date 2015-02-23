using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Emotiv;

public class TrainCognitiv : MonoBehaviour {
	
	private static int halfScreenHeight = Screen.height / 2;
	private static int halfScreenWidth = Screen.width / 2;

	private static bool showResetTrainingDialog = false;

	public Transform cubeObject;

	private static UInt32 trainedActions;
	
	void Start()
	{
		//Set the 4 training actions.
		EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[1], true);
		EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[2], true);
		EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[5], true);
		EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[6], true);
		EmoCognitiv.EnableCognitivActionsList();
	}
	
	void Update()
	{
		HandleEvents();	
	}

	void OnGUI()
	{
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.fontSize = 24;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.white;

		bool trainingStarted = EmoCognitiv.trainingStarted;

		Single pushSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[1]);
		Single pullSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[2]);
		Single leftSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[5]);
		Single rightSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[6]);

		//Neutral label
		GUI.Label(new Rect(halfScreenWidth + 60, halfScreenHeight - 10, 90, 25), "Neutral", labelStyle);

		//Train Neutral 
		if (GUI.Button(new Rect(halfScreenWidth + 210, halfScreenHeight - 10, 90, 25), "Train"))
		{
			trainingStarted = true;
			TrainNeutral();
		}
		//Reset Neutral
		if (GUI.Button(new Rect(halfScreenWidth + 320, halfScreenHeight - 10, 90, 25), "Reset"))
			ResetNeutral();			

		//Push label
		GUI.Label(new Rect(halfScreenWidth + 60, halfScreenHeight + 20, 90, 25), "Push", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 130, halfScreenHeight + 20, 90, 25), Math.Round(pushSkill * 100, 0).ToString() + "%", labelStyle);				

		//Train Push
		if (GUI.Button(new Rect(halfScreenWidth + 210, halfScreenHeight + 20, 90, 25), "Train"))
			TrainPush();

		//Reset Push
		if (GUI.Button(new Rect(halfScreenWidth + 320, halfScreenHeight + 20, 90, 25), "Reset"))
			ResetPush();

		//Pull label
		GUI.Label(new Rect(halfScreenWidth + 60, halfScreenHeight + 50, 90, 25), "Pull", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 130, halfScreenHeight + 50, 90, 25), Math.Round(pullSkill * 100, 0).ToString() + "%", labelStyle);
		
		//Train Pull
		if (GUI.Button(new Rect(halfScreenWidth + 210, halfScreenHeight + 50, 90, 25), "Train"))
			TrainPull();
		
		//Reset Pull
		if (GUI.Button(new Rect(halfScreenWidth + 320, halfScreenHeight + 50, 90, 25), "Reset"))
			ResetPull();
		
		//Left label
		GUI.Label(new Rect(halfScreenWidth + 60, halfScreenHeight + 80, 90, 25), "Left", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 130, halfScreenHeight + 80, 90, 25), Math.Round(leftSkill * 100, 0).ToString() + "%", labelStyle);

		//Train Left
		if (GUI.Button(new Rect(halfScreenWidth + 210, halfScreenHeight + 80, 90, 25), "Train"))
			TrainLeft();
		
		//Reset Left
		if (GUI.Button(new Rect(halfScreenWidth + 320, halfScreenHeight + 80, 90, 25), "Reset"))
			ResetLeft();
		
		//Right label
		GUI.Label(new Rect(halfScreenWidth + 60, halfScreenHeight + 110, 90, 25), "Right", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 130, halfScreenHeight + 110, 90, 25), Math.Round(rightSkill * 100, 0).ToString() + "%", labelStyle);

		//Train Right
		if (GUI.Button(new Rect(halfScreenWidth + 210, halfScreenHeight + 110, 90, 25), "Train"))
			TrainRight();
		
		//Reset Right
		if (GUI.Button(new Rect(halfScreenWidth + 320, halfScreenHeight + 110, 90, 25), "Reset"))
			ResetRight();
		
		//Display the window which asks whether the user is sure of wishing to reset the training.
		if (showResetTrainingDialog)
		{
			Rect resetTrainingWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 40, 350, 80);
			resetTrainingWindow = GUILayout.Window(5, resetTrainingWindow, DoResetTrainingAction, "Reset Training");	
		}		
		
		string cogAction = EmoCognitiv.getCurrentAction();
		float power = (float) Math.Round(EmoCognitiv.getCurrentActionPower(), 5); 

		//Animate cube according to current action and its power. 
		if (cogAction == "COG_NEUTRAL")
		{
			//Fix the cube to the center of the room.
			SetDefaultCubePosition(); 
		}
		else if (cogAction == "COG_PUSH")
		{
			//Send cube to back.
			SetDefaultCubePosition();
			cubeObject.localScale -= new Vector3(power * 0.5f, power * 0.5f, power * 0.5f);
			//cubeObject.Translate (new Vector3(-1, -1, 0) * power * 2);
		}
		else if (cogAction == "COG_PULL")
		{
			//Bring cube to front.
			SetDefaultCubePosition();
			cubeObject.localScale += new Vector3(power * 0.3f, power * 0.3f, power * 0.3f);
			//cubeObject.Translate (new Vector3(1, 1, -1) * power * 2);
		}
		else if (cogAction == "COG_LEFT")
		{
			//Move cube to left.
			SetDefaultCubePosition();
			cubeObject.Translate (Vector3.left * power * 1.5f);
		}
		else if (cogAction == "COG_RIGHT")
		{
			//Move cube to right.
			SetDefaultCubePosition();
			cubeObject.Translate (Vector3.right * power * 1.5f);
		}

		if (GUI.Button (new Rect(halfScreenWidth - 20, Screen.height - 45, 50, 30), "Back"))
			Application.LoadLevel(1);

		//Display info label.
		trainedActions = EmoCognitiv.getTrainedActions();
		if (trainedActions != 103)
			GUI.Label (new Rect(halfScreenWidth, halfScreenHeight - 50, 400, 50), "<color=red>You haven't trained all actions. You won't be able to see any progress in animations or skill.</color>");
		else
			GUI.Label (new Rect(halfScreenWidth, halfScreenHeight - 50, 400, 50), "<color=green>All actions have been trained.</color>");

		if (trainingStarted)	
		{
			GUI.Label(new Rect(halfScreenWidth - 250, halfScreenHeight - 95, 150, 25), "<color=orange>Training in progress...</color>");
		}
	}
	
	private void TrainNeutral()
	{
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[0]); //neutral()	
	}

	private void TrainPush()
	{
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[1]); //push()
	}

	private void TrainPull()
	{
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[2]); //pull()
	}

	private void TrainLeft()
	{
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[5]); //left()
	}

	private void TrainRight()
	{
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[6]); //right()
	}

	private void ResetNeutral()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[0]);
		showResetTrainingDialog = true;		
	}

	private void ResetPush()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[1]);
		showResetTrainingDialog = true;
	}

	private void ResetPull()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[2]);
		showResetTrainingDialog = true;
	}

	private void ResetLeft()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[5]);
		showResetTrainingDialog = true;
	}

	private void ResetRight()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[6]);
		showResetTrainingDialog = true;
	}

	private void HandleEvents()
	{
		Debug.Log("Checking events");
		Debug.Log (trainedActions);
	}	

	private static void DoResetTrainingAction(int windowID)
	{
		GUILayout.Space (2);
		
		//Get label and button style
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");		
		
		//Set alignment to center, fix the button width and set image label.
		labelStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.fixedWidth = 60;
		
		GUILayout.Label ("Are you sure you want to reset this action training and delete all training data?", labelStyle);
		GUILayout.Space (5);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Yes", buttonStyle))
		{
			EdkDll.EE_CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ERASE);
			EmoProfileManagement.Instance.SaveCurrentProfile();
			EmoProfileManagement.Instance.SaveProfilesToFile();
			showResetTrainingDialog = false;
		}
		GUILayout.Space(15);
		if (GUILayout.Button ("No", buttonStyle))
		{
			showResetTrainingDialog = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void SetDefaultCubePosition()
	{
		cubeObject.position = new Vector3(947.2634f, 4.680414f, 24);
		cubeObject.localScale = new Vector3(0.6f, 0.6f, 0.6f);
	}
	
}//class
