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

		Single pushSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[1]);
		Single pullSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[2]);
		Single leftSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[5]);
		Single rightSkill = EmoEngine.Instance.CognitivGetActionSkillRating((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[6]);

		//Neutral label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight - 38, 90, 25), "Neutral", labelStyle);

		//Train Neutral 
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight - 38, 90, 25), "Train"))
			TrainNeutral();
		
		//Reset Neutral
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight - 38, 90, 25), "Reset"))
			ResetNeutral();			

		//Push label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight - 8, 90, 25), "Push", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 120, halfScreenHeight - 8, 90, 25), Math.Round(pushSkill * 100, 2).ToString() + "%", labelStyle);				

		//Train Push
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight - 8, 90, 25), "Train"))
			TrainPush();

		//Reset Push
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight - 8, 90, 25), "Reset"))
			ResetPush();

		//Pull label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight + 22, 90, 25), "Pull", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 120, halfScreenHeight + 22, 90, 25), Math.Round(pullSkill * 100, 2).ToString() + "%", labelStyle);
		
		//Train Pull
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 22, 90, 25), "Train"))
			TrainPull();
		
		//Reset Pull
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 22, 90, 25), "Reset"))
			ResetPull();
		
		//Left label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight + 52, 90, 25), "Left", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 120, halfScreenHeight + 52, 90, 25), Math.Round(leftSkill * 100, 2).ToString() + "%", labelStyle);

		//Train Left
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 52, 90, 25), "Train"))
			TrainLeft();
		
		//Reset Left
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 52, 90, 25), "Reset"))
			ResetLeft();
		
		//Right label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight + 82, 90, 25), "Right", labelStyle);
		GUI.Label (new Rect(halfScreenWidth + 120, halfScreenHeight + 82, 90, 25), Math.Round(rightSkill * 100, 2).ToString() + "%", labelStyle);

		//Train Right
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 82, 90, 25), "Train"))
			TrainRight();
		
		//Reset Right
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 82, 90, 25), "Reset"))
			ResetRight();
		
		//Display the window which asks whether the user is sure of wishing to reset the training.
		if (showResetTrainingDialog)
		{
			Rect resetTrainingWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 40, 350, 80);
			resetTrainingWindow = GUILayout.Window(5, resetTrainingWindow, DoResetTrainingAction, "Reset Training");	
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

}//class
