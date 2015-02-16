using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	private bool paused;
	private bool showPauseDialog;
	private bool showExitDialog;
	private float lastTimeScale; 

	// Use this for initialization
	void Start () {
		paused = false;
		showPauseDialog = false;
		showExitDialog = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Attach this to a different object. e.g. camera.
		//Pause/resume when P key is pressed or eyebrows are raised.
		if ((Input.GetKeyDown (KeyCode.P)) && (paused == false))
		{
			paused = true;
			lastTimeScale = Time.timeScale;
			Time.timeScale = 0.0001f;
			AudioListener.pause = true;
			showPauseDialog = true;
		}
		else if ((Input.GetKeyDown (KeyCode.P) || showPauseDialog == false) && (paused == true))
		{
			paused = false;
			Time.timeScale = lastTimeScale;
			AudioListener.pause = false;
			showPauseDialog = false;
		}
	}

	void OnGUI() {
		if (showPauseDialog)
		{
			Rect pauseWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 30, 350, 60);
			pauseWindow = GUILayout.Window(6, pauseWindow, DoPauseAction, "Game Paused");
		}
		if (showExitDialog)
		{
			Rect exitWindow = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 40, 400, 80);
			exitWindow = GUILayout.Window(7, exitWindow, DoExitAction, "Go to Main Menu?");
		}
	}

	//Displays game paused window. 
	void DoPauseAction(int windowID)
	{
		GUILayout.Space (2);
		
		//Get label and button style
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");		
		
		//Set alignment to center, fix the button width and set image label.
		labelStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.fixedWidth = 80;
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Resume", buttonStyle))
		{
			showPauseDialog = false;			
		}
		if (GUILayout.Button("Main Menu", buttonStyle))
		{
			//Display are you sure you want to exit? Your game progress will not be saved.
			showPauseDialog = false;
			showExitDialog = true;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	void DoExitAction(int windowID)
	{
		GUILayout.Space (2);
		
		//Get label and button style
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");		
		
		//Set alignment to center, fix the button width and set image label.
		labelStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.fixedWidth = 80;
		
		GUILayout.Label("Are you sure you want to go back to main menu? Your game progress will not be saved.");
		GUILayout.Space (5);

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Yes", buttonStyle))
		{
			Application.LoadLevel (1);			
		}
		if (GUILayout.Button("No", buttonStyle))
		{
			//Display are you sure you want to exit? Your game progress will not be saved.
			showExitDialog = false;
			showPauseDialog = false;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
}
