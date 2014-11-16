using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Emotiv;

public class TrainCognitiv : MonoBehaviour {
	
	private bool isNeutralTrain = false;
	private bool isPushTrain = false;
	private bool isPullTrain = false;
	private bool isLeftTrain = false;
	private bool isRightTrain = false;
	
	private bool isNeutralReset = false;
	private bool isPushReset = false;
	private bool isPullReset = false;
	private bool isLeftReset = false;
	private bool isRightReset = false;
	
	private static int halfScreenHeight = Screen.height / 2;
	private static int halfScreenWidth = Screen.width / 2;
	
	void Start()
	{
		EmoEngine engine = EmoEngine.Instance;
		
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
	}
	
	void OnGUI()
	{
		//Train Neutral 
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight - 38, 90, 25), "Train"));
		 // trainNeutral();
		 // EmoEngine.Instance.CognitivSetTrainingAction(0, EdkDll.EE_CognitivAction_t.COG_PUSH);                    
         // EmoEngine.Instance.CognitivSetTrainingControl(0, EdkDll.EE_CognitivTrainingControl_t.COG_START);
		
		//Reset Neutral
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight - 38, 90, 25), "Reset"));
		 
		//Train Push
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight - 8, 90, 25), "Train"));
		
		//Reset Push
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight - 8, 90, 25), "Reset"));
		
		//Train Pull
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 22, 90, 25), "Train"));
		
		//Reset Pull
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 22, 90, 25), "Reset"));
		
		//Train Left
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 52, 90, 25), "Train"));
		
		//Reset Left
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 52, 90, 25), "Reset"));
		
		//Train Right
		if (GUI.Button(new Rect(halfScreenWidth + 200, halfScreenHeight + 82, 90, 25), "Train"));
		
		//Reset Right
		if (GUI.Button(new Rect(halfScreenWidth + 310, halfScreenHeight + 82, 90, 25), "Reset"));
	}
	
	private static void engine_CognitivEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
            EmoState es = e.emoState;
            EdkDll.EE_CognitivAction_t cogAction = es.CognitivGetCurrentAction();
            Single power = es.CognitivGetCurrentActionPower();
            Boolean isActive = es.CognitivIsActive();            
    }
	
	private static void engine_CognitivTrainingStarted(object sender, EmoEngineEventArgs e)
    {
        Console.WriteLine("Start Cognitiv Training");
    }

	//TODO: Change this to use dialog window buttons.
	private static void engine_CognitivTrainingSucceeded(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("Cognitiv Training Success. Accept/Reject?");
		/*ConsoleKeyInfo cki = Console.ReadKey(true);
		if (cki.Key == ConsoleKey.A)
		{
			EmoEngine.Instance.CognitivSetTrainingControl(0, EdkDll.EE_CognitivTrainingControl_t.COG_ACCEPT);
		}
		else
		{
			EmoEngine.Instance.CognitivSetTrainingControl(0, EdkDll.EE_CognitivTrainingControl_t.COG_REJECT);
		}*/
	}
	
	private static void engine_CognitivTrainingCompleted(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("Cognitiv Training Completed.");
	}

	private static void engine_CognitivTrainingRejected(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("Cognitiv Training Rejected.");
	}
}
