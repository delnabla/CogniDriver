using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public bool isStart = false;
	public bool isTrain = false;
	public bool isStatistics = false;
	public bool isOptions = false;
	public bool isExit = false;

	public float musicVolume = 10f;
	public float sfxVolume = 10f;
	public bool fullscreen = true;

	private bool showExitDialog = false;
	private bool showOptionsDialog = false;
	
	void OnMouseEnter()
	{
		renderer.material.color = Color.yellow;
	}

	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}

	void OnMouseUp()
	{
		if (isExit)
			showExitDialog = true;
		else if (isStart)
			Application.LoadLevel(1);
		else if (isOptions)
			showOptionsDialog = true;
	}
	
	void OnGUI()
	{
		//Note the first unique window ID. Changing it might break stuf!!!	
		//Also, default texture for windows is inherently transparent, 
		//so windows can't be made opaque without using a custom style or skin (cf http://forum.unity3d.com/threads/gui-transparency-question.120376/).

		//Display exit message.
		if (showExitDialog)
		{
			GUI.color = Color.white;
			Rect exitWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 35, 350, 70);
			exitWindow = GUILayout.Window(0, exitWindow, DoExitAction, "Exit");
		}

		//Display options window.
		if (showOptionsDialog)
		{
			Rect optionsWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 50, 350, 100);
			optionsWindow = GUILayout.Window(1, optionsWindow, DoOptionsAction, "Options");	
		}
	}

	//Displays "Are you sure you want to exit?" window. If yes, also saves player profile data.
	void DoExitAction(int windowID)
	{
		GUILayout.Space (2);

		//Get label and button style
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");		

		//Set alignment to center, fix the button width and set image label.
		labelStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.fixedWidth = 60;

		GUILayout.Label ("Are you sure you want to quit?", labelStyle);
		GUILayout.Space (5);
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Yes", buttonStyle))
				Application.Quit();
			GUILayout.Space(15);
			if (GUILayout.Button ("No", buttonStyle))
				showExitDialog = false;
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	//Displays options window
	void DoOptionsAction(int windowID)
	{
		GUILayout.BeginVertical();
			GUILayout.Space (2);
			
			//Get label and slider style.
			GUIStyle labelStyle = GUI.skin.GetStyle("Label");
			GUIStyle sliderStyle = GUI.skin.GetStyle("HorizontalSlider");
			
			//Set alignment to left, fix the slider width 
			labelStyle.alignment = TextAnchor.MiddleLeft;
			sliderStyle.fixedWidth = 100;		

			//Sound volume
			GUILayout.BeginHorizontal();
				GUILayout.Label ("Music volume", labelStyle);
				musicVolume = GUILayout.HorizontalSlider(musicVolume, 0.0f, 10.0f);
			GUILayout.EndHorizontal();
			
			//SFX volume
			GUILayout.BeginHorizontal();
				GUILayout.Label ("Sound effects volume", labelStyle);
				sfxVolume = GUILayout.HorizontalSlider(sfxVolume, 0.0f, 10.0f);
			GUILayout.EndHorizontal();
			
			//Fullscreen
			GUILayout.BeginHorizontal();
				GUILayout.Label ("Fullscreen", labelStyle);
				GUILayout.Space(80);
				fullscreen = GUILayout.Toggle (fullscreen, "");
			GUILayout.EndHorizontal();		

			//Close button
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Back"))
					showOptionsDialog = false;
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

}//class
