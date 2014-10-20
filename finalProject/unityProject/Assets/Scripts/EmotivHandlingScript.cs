using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using Emotiv;



public class EmotivHandlingScript 
{	
	public EmotivHandlingScript()
	{
		Main ();
	}

	static void engine_EmoEngineConnected(object sender, EmoEngineEventArgs e)
	{
		Debug.Log("Connected");
	}

	static void engine_UserAdded(object sender, EmoEngineEventArgs e)
	{
		Debug.Log("user added " + e.userId);
		Profile profile = EmoEngine.Instance.GetUserProfile(0);
		profile.GetBytes();
	}

	static void engine_ExpressivEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
	{
		EmoState es = e.emoState;
		
		EdkDll.EE_ExpressivAlgo_t[] expAlgoList = { 
			EdkDll.EE_ExpressivAlgo_t.EXP_BLINK, 
			EdkDll.EE_ExpressivAlgo_t.EXP_CLENCH, 
			EdkDll.EE_ExpressivAlgo_t.EXP_EYEBROW, 
			EdkDll.EE_ExpressivAlgo_t.EXP_FURROW, 
			EdkDll.EE_ExpressivAlgo_t.EXP_HORIEYE, 
			EdkDll.EE_ExpressivAlgo_t.EXP_LAUGH, 
			EdkDll.EE_ExpressivAlgo_t.EXP_NEUTRAL, 
			EdkDll.EE_ExpressivAlgo_t.EXP_SMILE, 
			EdkDll.EE_ExpressivAlgo_t.EXP_SMIRK_LEFT, 
			EdkDll.EE_ExpressivAlgo_t.EXP_SMIRK_RIGHT, 
			EdkDll.EE_ExpressivAlgo_t.EXP_WINK_LEFT, 
			EdkDll.EE_ExpressivAlgo_t.EXP_WINK_RIGHT
		};
		Boolean[] isExpActiveList = new Boolean[expAlgoList.Length];
		
		Boolean isBlink = es.ExpressivIsBlink();
		Boolean isLeftWink = es.ExpressivIsLeftWink();
		Boolean isRightWink = es.ExpressivIsRightWink();
		Boolean isEyesOpen = es.ExpressivIsEyesOpen();
		Boolean isLookingUp = es.ExpressivIsLookingUp();
		Boolean isLookingDown = es.ExpressivIsLookingDown();
		Boolean isLookingLeft = es.ExpressivIsLookingLeft();
		Boolean isLookingRight = es.ExpressivIsLookingRight();
		Single leftEye = 0.0F;
		Single rightEye = 0.0F;
		Single x = 0.0F;
		Single y = 0.0F;
		es.ExpressivGetEyelidState(out leftEye, out rightEye);
		es.ExpressivGetEyeLocation(out x, out y);
		Single eyebrowExtent = es.ExpressivGetEyebrowExtent();
		Single smileExtent = es.ExpressivGetSmileExtent();
		Single clenchExtent = es.ExpressivGetClenchExtent();
		EdkDll.EE_ExpressivAlgo_t upperFaceAction = es.ExpressivGetUpperFaceAction();
		Single upperFacePower = es.ExpressivGetUpperFaceActionPower();
		EdkDll.EE_ExpressivAlgo_t lowerFaceAction = es.ExpressivGetLowerFaceAction();
		Single lowerFacePower = es.ExpressivGetLowerFaceActionPower();
		for (int i = 0; i < expAlgoList.Length; ++i)
		{
			isExpActiveList[i] = es.ExpressivIsActive(expAlgoList[i]);
		}
		
		Debug.Log( 	"" + isBlink + "" + isLeftWink + "" + isRightWink + "" + isEyesOpen + "" + isLookingUp +
		          "" + isLookingDown + "" + isLookingLeft + "" + isLookingRight + "" + leftEye + "" + rightEye +
		          "" + x + "" + y + "" + eyebrowExtent + "" + smileExtent + "" + upperFaceAction +
		          "" + upperFacePower + "" + lowerFaceAction + "" + lowerFacePower);
		for (int i = 0; i < expAlgoList.Length; ++i)
		{
			Debug.Log(isExpActiveList[i]);
		}  
	}

	static void engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
	{
		EmoState es = e.emoState;
		
		Int32 headsetOn = es.GetHeadsetOn();
		Int32 numCqChan = es.GetNumContactQualityChannels();            
		EdkDll.EE_EEG_ContactQuality_t[] cq = es.GetContactQualityFromAllChannels();
		for (Int32 i = 0; i < numCqChan; ++i)
		{
			if (cq[i] != es.GetContactQuality(i))
			{
				throw new Exception();
			}
		}
		EdkDll.EE_SignalStrength_t signalStrength = es.GetWirelessSignalStatus();
		Int32 chargeLevel = 0;
		Int32 maxChargeLevel = 0;
		es.GetBatteryChargeLevel(out chargeLevel, out maxChargeLevel);
		
		
		Debug.Log( headsetOn + signalStrength + chargeLevel + maxChargeLevel);
		
		for (int i = 0; i < cq.Length; ++i)
		{
			Debug.Log(cq[i]);
		}
	} 
	
	static void Main()
	{
		EmoEngine engine = EmoEngine.Instance;
		engine.EmoEngineConnected += new EmoEngine.EmoEngineConnectedEventHandler(engine_EmoEngineConnected);
		engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded);
		engine.ExpressivEmoStateUpdated += new EmoEngine.ExpressivEmoStateUpdatedEventHandler(engine_ExpressivEmoStateUpdated);
		engine.EmoEngineEmoStateUpdated += new EmoEngine.EmoEngineEmoStateUpdatedEventHandler(engine_EmoEngineEmoStateUpdated);
		engine.Connect();
		while (true)
		{
			try
			{
				engine.ProcessEvents(1000); //Processes EmoEngine events until there are no more events or 1000ms have elapsed. 
			}
			catch (EmoEngineException e)
			{
				Debug.Log(e.ToString());
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		} //while
		engine.Disconnect();
	}
	
	
	
} // class
