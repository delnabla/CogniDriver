using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class EmoCognitiv : MonoBehaviour
{
    //----------------------------------------
    EmoEngine engine = EmoEngine.Instance;
    public static EdkDll.EE_CognitivAction_t[] cognitivActionList = {EdkDll.EE_CognitivAction_t.COG_NEUTRAL,
                                                                    EdkDll.EE_CognitivAction_t.COG_PUSH,
                                                                    EdkDll.EE_CognitivAction_t.COG_PULL,
                                                                    EdkDll.EE_CognitivAction_t.COG_LIFT,
                                                                    EdkDll.EE_CognitivAction_t.COG_DROP,
                                                                    EdkDll.EE_CognitivAction_t.COG_LEFT,
                                                                    EdkDll.EE_CognitivAction_t.COG_RIGHT,
                                                                    EdkDll.EE_CognitivAction_t.COG_ROTATE_LEFT,
                                                                    EdkDll.EE_CognitivAction_t.COG_ROTATE_RIGHT,
                                                                    EdkDll.EE_CognitivAction_t.COG_ROTATE_CLOCKWISE,
                                                                    EdkDll.EE_CognitivAction_t.COG_ROTATE_COUNTER_CLOCKWISE,
                                                                    EdkDll.EE_CognitivAction_t.COG_ROTATE_FORWARDS,
                                                                    EdkDll.EE_CognitivAction_t.COG_ROTATE_REVERSE,
                                                                    EdkDll.EE_CognitivAction_t.COG_DISAPPEAR
                                                                    };
    public static Boolean[] cognitivActionsEnabled = new Boolean[cognitivActionList.Length];
    public static float[] CognitivActionPower = new float[cognitivActionList.Length];
    public static int cognitivActionLever = 0;//Number Of Active Action   
    public static bool IsStarted = false;

	//Variables added by Daniela Florescu
	private static bool showTrainingCompleteDialog = false; 
	private static EdkDll.EE_CognitivAction_t cogAction;
	private static float power;
	public static bool trainingStarted = false;
    
	//----------------------------------------  
    void Start() 
    {
        if (!IsStarted)
        {
            cognitivActionsEnabled[0] = true;
            for (int i = 1; i < cognitivActionList.Length; i++)
            {
                cognitivActionsEnabled[i] = false;
            }
            engine.CognitivEmoStateUpdated +=
                new EmoEngine.CognitivEmoStateUpdatedEventHandler(engine_CognitivEmoStateUpdated);
            engine.CognitivTrainingStarted +=
                new EmoEngine.CognitivTrainingStartedEventEventHandler(engine_CognitivTrainingStarted);
            engine.CognitivTrainingSucceeded +=
                new EmoEngine.CognitivTrainingSucceededEventHandler(engine_CognitivTrainingSucceeded);
            engine.CognitivTrainingCompleted +=
                new EmoEngine.CognitivTrainingCompletedEventHandler(engine_CognitivTrainingCompleted);
            engine.CognitivTrainingRejected +=
                new EmoEngine.CognitivTrainingRejectedEventHandler(engine_CognitivTrainingRejected);
			engine.CognitivTrainingDataErased += 
				new EmoEngine.CognitivTrainingDataErasedEventHandler(engine_CognitivTrainingErase); //added by Daniela Florescu

            IsStarted = true;
        }
    }
    float timeVariable;
    void Update()
    {
        //if in 2s , there's no updata of cognitivStatei
		//set the state down to 0
		timeVariable += Time.deltaTime;
		if (timeVariable <0.3)
		{
			isNotResponding = true;
		}
		
		if (timeVariable >1.0f) 
		{
			timeVariable = 0.0f;
			if (isNotResponding)
			{
					//call smooth state
					ResetCognitivPower(2);
                    ResetCognitivPower(3);
			}
		}
    }

	//Method added by Daniela Florescu
	void OnGUI()
	{
		//Display the window which asks whether to accept the current training.
		if (showTrainingCompleteDialog)
		{
			Rect completeTrainingWindow = new Rect(Screen.width / 2 - 175, Screen.height /2 - 50, 350, 100);
			completeTrainingWindow = GUILayout.Window(4, completeTrainingWindow, DoTrainingCompleteAction, "Training Complete");
		}
	}

    public  static bool isNotResponding = true;
    static void engine_CognitivEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
        //Debug.LogError("CognitivEmoStateUpdated");
        isNotResponding = false;
        EmoState es = e.emoState;
        cogAction = es.CognitivGetCurrentAction();
        //Debug.LogError(cogAction);
        power = (float)es.CognitivGetCurrentActionPower();
        //Debug.LogError(power + ":" +(uint)cogAction);
        //CognitivActionPower[(uint)cogAction] = power;
        for (int i = 1; i < cognitivActionList.Length; i++)
        {
            if (cogAction == cognitivActionList[i])
            {
                CognitivActionPower[i] = power;
                //Debug.LogError(CognitivActionPower[i] + "----------------------");
            }
            else CognitivActionPower[i] = 0;
        }
    }
    static void engine_CognitivTrainingStarted(object sender, EmoEngineEventArgs e)
    {
        Debug.Log("Cognitiv Training Started");
        trainingStarted = true;
    }

    static void engine_CognitivTrainingSucceeded(object sender, EmoEngineEventArgs e)
    {
		showTrainingCompleteDialog = true;
		trainingStarted = false;
        Debug.Log("Cognitiv Training Succeeded");
    }

    static void engine_CognitivTrainingCompleted(object sender, EmoEngineEventArgs e)
    {
        Debug.Log("Cognitiv Training Completed");
		trainingStarted = false;
    }

    static void engine_CognitivTrainingRejected(object sender, EmoEngineEventArgs e)
    {
        Debug.Log("Cognitiv Training Rejected");
		trainingStarted = false;
    }

	//Method added by Daniela Florescu
	static void engine_CognitivTrainingErase(object sender, EmoEngineEventArgs e)
	{
		Debug.Log("Cognitiv Training Erased");
		trainingStarted = false; 
	}

    /// <summary>
    /// Start traning cognitiv action
    /// </summary>
    /// <param name="cognitivAction">Cognitiv Action</param>
    public static void StartTrainingCognitiv(EdkDll.EE_CognitivAction_t cognitivAction)
    {
        if (cognitivAction == EdkDll.EE_CognitivAction_t.COG_NEUTRAL)
        {
            EmoEngine.Instance.CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, cognitivAction);
			EmoEngine.Instance.CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_START);
        }
        else
            for (int i = 1; i < cognitivActionList.Length; i++)
            {
                if (cognitivAction == cognitivActionList[i])
                {
                    Debug.Log("Action compare");
                    if (cognitivActionsEnabled[i])
                    {
                        Debug.Log("Action is enabled");
                        EmoEngine.Instance.CognitivSetTrainingAction((uint)EmoUserManagement.currentUser, cognitivAction);
						EmoEngine.Instance.CognitivSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.EE_CognitivTrainingControl_t.COG_START);
					}
                    else Debug.Log("Action is not enabled");
                }
            }
    }

    /// <summary>
    /// Enable cognitiv action in arraylist
    /// </summary>
    /// <param name="cognitivAction">Cognitiv Action</param>
    /// <param name="iBool">True = Enable/False = Disable</param>
    public static void EnableCognitivAction(EdkDll.EE_CognitivAction_t cognitivAction, Boolean iBool)
    {
        for (int i = 1; i < cognitivActionList.Length; i++)
        {
            if (cognitivAction == cognitivActionList[i])
            {
                cognitivActionsEnabled[i] = iBool;
                Debug.Log("CognitivEnabledList has changed");
            }
        }

    }
    /// <summary>
    /// Enable actions in arraylist (Working in both Unity 2.5 vs 2.6)
    /// </summary>
    public static void EnableCognitivActionsListEx()
    {
        Debug.Log("Set CognitivList Enable");
        cognitivActionLever = 0;
        for (int i = 1; i < cognitivActionList.Length; i++)
        {
            if (cognitivActionsEnabled[i])
            {
                cognitivActionLever++;
            }
        }
        EdkDll.EE_CognitivAction_t[] activeActions = new EdkDll.EE_CognitivAction_t[cognitivActionLever];
        int tmp = 0;
        for (int i = 1; i < cognitivActionList.Length; i++)
        {
            if (cognitivActionsEnabled[i])
            {
                activeActions[tmp] = cognitivActionList[i];
                tmp++;
            }
        }
        EdkDll.SetMultiActiveActions(EmoUserManagement.currentUser, activeActions, cognitivActionLever);
    }
    /// <summary>
    /// Enable actions in arraylist , Work only in 2.6 or upper
    /// </summary>
    public static void EnableCognitivActionsList()
    {
        Debug.Log("Set CognitivList Enable");
        cognitivActionLever = 0;
        uint cognitivActions = 0x0000;
        for (int i = 1; i < cognitivActionList.Length; i++)
        {
            if (cognitivActionsEnabled[i])
            {
                cognitivActions = cognitivActions | ((uint)cognitivActionList[i]);
                cognitivActionLever++;
            }
        }
        EdkDll.EE_CognitivSetActiveActions((uint)EmoUserManagement.currentUser, cognitivActions);
        //EmoEngine.Instance.CognitivSetActiveActions((uint)EmoUserManagement.currentUser,(uint) cognitivActions);
    }
    /// <summary>
    /// Get cognitiv action power in an array of float
    /// </summary>
    /// <returns></returns>
    public static float[] GetCognitivActionPower()
    {
        return CognitivActionPower;
    }
    public static void ResetAllCognitivPower()
    {
        for (int i = 0; i < cognitivActionList.Length;i++ )
        {
            CognitivActionPower[i] = 0;
        }
    }
    public static void ResetCognitivList()
    {
        for (int i = 1; i < cognitivActionsEnabled.Length; i++)
        {
            cognitivActionsEnabled[i] = false;
        }
    }
    public static void ResetCognitivPower(int cognitivAction)
    {
        CognitivActionPower[cognitivAction] = 0;
    }

	//Method added by Daniela Florescu.
	private static void DoTrainingCompleteAction(int windowID)
	{
		if (showTrainingCompleteDialog)
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
				EmoProfileManagement.Instance.SaveCurrentProfile();
				EmoProfileManagement.Instance.SaveProfilesToFile();
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
		}
	}

	//Accessor methods to current action and power. Added by Daniela Florescu.
	public static string getCurrentAction()
	{
		return cogAction.ToString();
	}
	
	public static float getCurrentActionPower()
	{
		return power;
	}	

	public static UInt32 getTrainedActions()
	{
		return EmoEngine.Instance.CognitivGetTrainedSignatureActions((uint)EmoUserManagement.currentUser);
	}

}