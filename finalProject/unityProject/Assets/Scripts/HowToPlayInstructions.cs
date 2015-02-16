using UnityEngine;
using System.Collections;

public class HowToPlayInstructions : MonoBehaviour {

	public Vector2 scrollPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUIStyle localStyle = new GUIStyle(GUI.skin.label);
		localStyle.normal.textColor = Color.black;
		localStyle.fontStyle = FontStyle.Bold;
		localStyle.fontSize = 20;	
		localStyle.wordWrap = false;	
		GUILayout.Space(15);
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label ("How to Play", localStyle);
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(15);

			localStyle.fontSize = 12;
			localStyle.fontStyle = FontStyle.Normal;

			GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace();
				GUILayout.Box ("<b><size=15>Keyboard</size></b> \n\nArrow Keys - Control car movement; \nSpacebar - Handbrake; \nC - Change camera view.",localStyle);
				GUILayout.FlexibleSpace();

				localStyle.wordWrap = true;
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width (400), GUILayout.Height (400));

				GUILayout.Box ("<b><size=15>Cognitiv</size></b> \n\n" +
				                 "For using the cognitiv game play mode, a player profile will first need to be trained in the \"Train Headset\" tab. \n\n " + 
				                 "The recommendation is to train the Neutral state a few times while in a relaxed mode, trying not to think about anything. \n\n" +
				                 "A \"Training in progress...\" label will appear on the top of the virtual room while training. \n\n" + 
				                 "Next, one should proceed to training once all of the other states Push, Pull, Left and Right. On first training, try to be relaxed. \n\n " +
				                 "After all states have been trained once, proceed to training the Push action. First reset your training so far, and concentrate on visualising " + 
				                 "how the cube is being pushed to the back of the virtual room. The percentage next to the name of the action, shows how well you can do that state. " + 
				                 "It represents the skill level. After training it once, the next time you're going to do it, you will see the cube being animated to give it visual clues as " + 
				                 "to how well you're doing the same thing. \n\n " +
				                 "If the cube is not moving at all, after multiple trainings, you should probably start over by resetting the action. " + 
				                 "If you're doing it good, and the skill level is not 0 but you can't move the cube this one try during training, just reject the training at the end as it will " + 
				                 "otherwise have a negative effect on your skill level. Proceed to training the other states, Pull, Left and Right in a similar way. \n\n " +
				                 "Repeat the training process until a high enough skill level is achieved for each state and you are confident you can perform each action at will." +  
				                 "The Push, Pull, Left, Right actions will be used to control the movement of the car. \n \n " + 
				                 "Additional controls: \n\n " +
				                 "Clench teeth - Handbrake; \n " +
				                 "Left wink - Change the camera view.", localStyle, GUILayout.MaxWidth (400));
				GUILayout.EndScrollView();
				GUILayout.FlexibleSpace();
				GUILayout.Box ("<b><size=15>Gyro</size></b>\n\nNot functioning. Work in progress.", localStyle);
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(30);
			GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button ("Back"))
					Application.LoadLevel (1);
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace();
	}

}
