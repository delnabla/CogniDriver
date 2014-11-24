using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Emotiv;

public class TrainCognitiv : MonoBehaviour {
	
	/*private bool isNeutralTrain = false;
	private bool isPushTrain = false;
	private bool isPullTrain = false;
	private bool isLeftTrain = false;
	private bool isRightTrain = false;
	
	private bool isNeutralReset = false;
	private bool isPushReset = false;
	private bool isPullReset = false;
	private bool isLeftReset = false;
	private bool isRightReset = false;*/
	
	private static int halfScreenHeight = Screen.height / 2;
	private static int halfScreenWidth = Screen.width / 2;

	//private static bool showTrainingCompleteDialog = false;
	
	void Start()
	{
		//showTrainingCompleteDialog = false;
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
		
		//Train Push
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight - 8, 90, 25), "Train"))
			TrainPush();

		//Reset Push
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight - 8, 90, 25), "Reset"))
			ResetPush();

		//Pull label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight + 22, 90, 25), "Pull", labelStyle);
		
		//Train Pull
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 22, 90, 25), "Train"))
			TrainPull();
		
		//Reset Pull
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 22, 90, 25), "Reset"))
			ResetPull();
		
		//Left label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight + 52, 90, 25), "Left", labelStyle);

		//Train Left
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 52, 90, 25), "Train"))
			TrainLeft();
		
		//Reset Left
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 52, 90, 25), "Reset"))
			ResetLeft();
		
		//Right label
		GUI.Label(new Rect(halfScreenWidth + 50, halfScreenHeight + 82, 90, 25), "Right", labelStyle);

		//Train Right
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 82, 90, 25), "Train"))
			TrainRight();
		
		//Reset Right
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 82, 90, 25), "Reset"))
			ResetRight();
		//Display the window which asks whether to accept the current training.
		/*if (showTrainingCompleteDialog)
		{
			Rect completeTrainingWindow = new Rect(Screen.width / 2 - 100, Screen.height /2 - 78, 210, 170);
			completeTrainingWindow = GUILayout.Window(4, completeTrainingWindow, DoTrainingCompleteAction, "Training Complete");
		}*/
	}
	
	private void TrainNeutral()
	{
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[0]); //neutral()
	}

	private void TrainPush()
	{
		//EmoCognitiv.cognitivActionsEnabled[1] = true;
		//EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[1], true);
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[1]); //push()
	}

	private void TrainPull()
	{
		//EmoCognitiv.cognitivActionsEnabled[2] = true;
		//EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[2], true);
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[2]); //pull()
	}

	private void TrainLeft()
	{
		//EmoCognitiv.cognitivActionsEnabled[5] = true;
		//EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[5], true);
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[5]); //left()
	}

	private void TrainRight()
	{
		//EmoCognitiv.cognitivActionsEnabled[6] = true;
		//EmoCognitiv.EnableCognitivAction(EmoCognitiv.cognitivActionList[6], true);
		EmoCognitiv.StartTrainingCognitiv(EmoCognitiv.cognitivActionList[6]); //right()
	}

	private void ResetNeutral()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[0]);
		EdkDll.EE_CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ERASE);
	}

	private void ResetPush()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[1]);
		EdkDll.EE_CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ERASE);
	}

	private void ResetPull()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[2]);
		EdkDll.EE_CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ERASE);
	}

	private void ResetLeft()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[5]);
		EdkDll.EE_CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ERASE);
	}

	private void ResetRight()
	{
		EdkDll.EE_CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, EmoCognitiv.cognitivActionList[6]);
		EdkDll.EE_CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ERASE);
	}

	private void HandleEvents()
	{
		Debug.Log("Checking events");
	
		/*IntPtr eEvent = EdkDll.EE_EmoEngineEventCreate();
		IntPtr eState = EdkDll.EE_EmoStateCreate();
		
		int state = EdkDll.EE_EngineGetNextEvent(eEvent);
		int eventType = (int) EdkDll.EE_EmoEngineEventGetType(eEvent);
		Debug.Log ("state: " + state);
		Debug.Log("event type: " + eventType);
		//if (eventType == (int) EdkDll.EE_Event_t.EE_CognitivEvent || eventType == (int) EdkDll.EE_Event_t.EE_EmoStateUpdated)
		//{
			//Debug.Log("Discovered cognitiv");
		//	EdkDll.EE_EmoEngineEventGetEmoState(eEvent, eState);
			EdkDll.EE_CognitivEvent_t cognitivEvent = EdkDll.EE_CognitivEventGetType(eEvent);
			
			switch (cognitivEvent) {
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingStarted:
			{
				Debug.Log ("Training Started");
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingSucceeded:
			{
				Debug.Log ("Cognitiv training succeeded");
				showTrainingCompleteDialog = true;
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingFailed:
			{
				Debug.Log ("Cognitiv training failed");
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingCompleted:
			{
				Debug.Log ("Cognitiv training complete");	
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingDataErased:
			{
				Debug.Log ("Cognitiv training erased");
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingRejected:
			{
				Debug.Log ("Cognitiv training rejected");
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivTrainingReset:
			{
				Debug.Log ("Cognitiv training reset"); 
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivAutoSamplingNeutralCompleted:
			{
				Debug.Log ("Cognitiv auto sampling neutral completed");
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivSignatureUpdated:
			{
				Debug.Log("Cognitiv signature updated");
				break;
			}
				
			case EdkDll.EE_CognitivEvent_t.EE_CognitivNoEvent:
				break;
				
			default:
				Debug.Log ("Unknown cognitiv event type");
				break;
			}
		//}
		Debug.Log ("showTrainingCompleteDialog: " + showTrainingCompleteDialog);*/
	}	

	/*private static void DoTrainingCompleteAction(int windowID)
	{
		GUILayout.Space (2);
		
		//Get label and button style
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");		
		
		//Set alignment to center, fix the button width and set image label.
		labelStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.fixedWidth = 60;
		
		GUILayout.Label ("Training is now complete. What would you like to do with this training?", labelStyle);
		GUILayout.Space (5);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Accept", buttonStyle))
		{
			EmoEngine.Instance.CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_ACCEPT);
			showTrainingCompleteDialog = false;
		}
		GUILayout.Space(15);
		if (GUILayout.Button ("Reject", buttonStyle))
		{
			EmoEngine.Instance.CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_REJECT);
			showTrainingCompleteDialog = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}*/

//training reset

}//class
