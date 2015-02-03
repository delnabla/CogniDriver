using UnityEngine;
using System.Collections;
using Emotiv;

public class MainMenuScript : MonoBehaviour {

	public bool isStart = false;
	public bool isTrain = false;
	public bool isStatistics = false;
	public bool isOptions = false;
	public bool isExit = false;
	public bool isHelp = false;
	public bool isPlayerProfile = false;

	public static float musicVolume = 10f;
	public static float sfxVolume = 10f;
	public static bool fullscreen = true;
	public GameObject welcomeMessage;

	private static bool showExitDialog = false;
	private static bool showOptionsDialog = false;
	private static bool showCreateProfile = false;
	private static bool showChooseProfile = false;

	private const string playerNameKeyPrefix = "PlayerName";
	private const string noOfProfilesKeyPrefix = "NumberOfPlayerProfiles";

	private static string playerName = "PlayerName";
	private string selectedPlayer;
	private static int selectedPlayerIndex = 0;
	private static string[] playerList;
	private static int noOfPlayerProfiles; //Up to 10.

	private static int selectionControlIndex = 1;
	public static string selectedControlTool = "Cognitiv";

	void Awake()
	{
		DontDestroyOnLoad(this);
	}

	void Start()
	{
		showChooseProfile = false;
	
		//Load emotiv profiles.
		if (isPlayerProfile)
			EmoProfileManagement.Instance.LoadProfilesFromFile();
		//Get the number of player profiles.
		if (PlayerPrefs.HasKey(noOfProfilesKeyPrefix))
			noOfPlayerProfiles = PlayerPrefs.GetInt(noOfProfilesKeyPrefix);	
		else 
			noOfPlayerProfiles = 0;

		//If there is one, ask to choose or create a new one.
		if (noOfPlayerProfiles > 0)
		{
			showChooseProfile = true;
			playerList = new string[noOfPlayerProfiles];
			for (int i = 0; i < noOfPlayerProfiles; i++) 
				playerList[i] = PlayerPrefs.GetString(playerNameKeyPrefix + i); 
		}	
		
		//If there aren't any, ask to create a new one.
		else
		{
			showCreateProfile = true;
		}
	}
	
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
		else if (isTrain)
			Application.LoadLevel(2);
		else if (isOptions)
			showOptionsDialog = true;
		else if (isPlayerProfile)
			showChooseProfile = true;
	}
	
	void OnGUI()
	{
		//Note the first unique window ID. Changing it might break stuf!!!	
		//Also, default texture for windows is inherently transparent, 
		//so windows can't be made opaque without using a custom style or skin (cf http://forum.unity3d.com/threads/gui-transparency-question.120376/).
		
		GUI.color = Color.white;

		//Display exit message.
		if (showExitDialog)
		{
			Rect exitWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 40, 350, 80);
			exitWindow = GUILayout.Window(0, exitWindow, DoExitAction, "Exit");
		}

		//Display options window.
		if (showOptionsDialog)
		{
			Rect optionsWindow = new Rect(Screen.width / 2 - 175, Screen.height / 2 - 75, 350, 150);
			optionsWindow = GUILayout.Window(1, optionsWindow, DoOptionsAction, "Options");	
		}

		//Display window to enter player name.
		if (showCreateProfile)
		{			
			Rect createProfileWindow = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 35, 200, 70);
			createProfileWindow = GUILayout.Window(2, createProfileWindow, CreateUserProfile, "New player");
		}

		//Display the window which allows the player to select an existing profile
		//or offers the option to create a new player profile.
		if (showChooseProfile)
		{
			Rect chooseProfileWindow = new Rect(Screen.width / 2 - 100, Screen.height /2 - 78, 250, 170);
			chooseProfileWindow = GUILayout.Window(3, chooseProfileWindow, ChooseUserProfile, "Choose player");
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

			//Control game by using keyboard, gyro or cognitiv.
			GUILayout.BeginHorizontal();
				GUILayout.Label ("Control using ", labelStyle);
				string[] selection = new string[]{"Keyboard", "Cognitiv", "Gyro"};
				selectionControlIndex = GUILayout.SelectionGrid(selectionControlIndex, selection, 3, "toggle");
				if (selectionControlIndex == 0)
					selectedControlTool = "Keyboard";
				else if (selectionControlIndex == 1)
					selectedControlTool = "Cognitiv";
				else 
					selectedControlTool = "Gyro";
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

	void CreateUserProfile(int windowID)
	{
		//Pop up window to introduce player name. Add 'create' button.
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();		
				playerName = GUILayout.TextField(playerName, 50);	
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Create"))
				{
					showCreateProfile = false;
					PlayerPrefs.SetString(playerNameKeyPrefix + noOfPlayerProfiles.ToString(), playerName); //save player name
					noOfPlayerProfiles++; 	
					PlayerPrefs.SetInt(noOfProfilesKeyPrefix, noOfPlayerProfiles); //update number of player profiles
					
					//Add new user to Emotiv and set it to current user.
					//EPOCManager.SendMessage("AddNewProfile", playerName);
					if (isPlayerProfile)
					{
						EmoProfileManagement.Instance.AddNewProfile(playerName);
				
						//Save all changes to PlayerPrefs and Emotiv.
						EmoProfileManagement.Instance.SaveCurrentProfile();
						EmoProfileManagement.Instance.SaveProfilesToFile();
					}
					PlayerPrefs.Save();	
					
					//Rebuild the playerList array.
					playerList = new string[noOfPlayerProfiles];
					for (int i = 0; i < noOfPlayerProfiles; i++) 
						playerList[i] = PlayerPrefs.GetString(playerNameKeyPrefix + i); 
			
					//Set welcome message.
					welcomeMessage.GetComponent<TextMesh>().text = "Welcome, " + playerName + "!";
				}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	void ChooseUserProfile(int windowID)
	{
		GUIStyle buttonStyle = GUI.skin.GetStyle("Button");	
		buttonStyle.fixedWidth = 105;

		//Pop up list to choose player name or enter a new one.
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();	

				//Get the chosen player.
				selectedPlayerIndex = GUILayout.SelectionGrid(selectedPlayerIndex, playerList, 2, buttonStyle);
				selectedPlayer = playerList[selectedPlayerIndex]; 

				//Set welcome message.
				welcomeMessage.GetComponent<TextMesh>().text = "Welcome, " + selectedPlayer + "!";
				
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				buttonStyle.fixedWidth = 75;
				if (GUILayout.Button("Select", buttonStyle))
				{
					showChooseProfile = false;	
					if (isPlayerProfile)
						EmoProfileManagement.Instance.SetUserProfile(selectedPlayer);
				}
				if (GUILayout.Button ("Remove", buttonStyle))
				{			
					//Remove last player name key.
					PlayerPrefs.DeleteKey(playerNameKeyPrefix + (noOfPlayerProfiles - 1));					
			
					//Decrease noOfPlayerProfiles.
					noOfPlayerProfiles--;
					PlayerPrefs.SetInt(noOfProfilesKeyPrefix, noOfPlayerProfiles); 

					//All player names after the removed one, receive the predecessor name key.
					for (int i = selectedPlayerIndex; i < noOfPlayerProfiles; i++)
						PlayerPrefs.SetString(playerNameKeyPrefix + i, playerList[i+1]);					

					//Rebuild the playerList array.
					playerList = new string[noOfPlayerProfiles];
					for (int i = 0; i < noOfPlayerProfiles; i++) 
						playerList[i] = PlayerPrefs.GetString(playerNameKeyPrefix + i); 
					
					if (isPlayerProfile)
					{
						EmoProfileManagement.Instance.DeleteProfile(selectedPlayer);
						EmoProfileManagement.Instance.SaveProfilesToFile();
					}
					PlayerPrefs.Save ();
	
					//Delete profile file manually because Emotiv doesn't clean it.
					string fileToDelete = System.IO.Directory.GetCurrentDirectory() + @"/EmotivUserProfile/" + selectedPlayer + ".up";
					if (System.IO.File.Exists(fileToDelete))
						System.IO.File.Delete(fileToDelete);
			
					//Set the current player to the currently last one.
					if (noOfPlayerProfiles == 0)
					{
						showCreateProfile = true;
						showChooseProfile = false;
					}
					else
					{
						selectedPlayerIndex = noOfPlayerProfiles - 1;
						selectedPlayer = playerList[selectedPlayerIndex]; 
					}
				}
				if (noOfPlayerProfiles < 10)
					if (GUILayout.Button("Create", buttonStyle))
					{
						showCreateProfile = true;
						showChooseProfile = false;	
					}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
		
}//class
