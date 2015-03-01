using UnityEngine;
using System.Collections;

public class ChooseCar : MonoBehaviour {

	public Texture arrowLeft;
	public Texture arrowRight;

	public Transform audi;
	public Transform ferrari;
	public Transform target;
	public Transform targetOut;

	private static int carIndex;
	private static int prevCarIndex; 

	private const int NO_OF_CARS = 2;

	private float smoothTime = 0.3f;
	private float xVelocity = 0;
	private float xVelocityOut = 0;

	public Texture2D blueColour;
	public Texture2D greenColour;
	public Texture2D yellowLightColour;
	public Texture2D yellowColour;
	public Texture2D purpleColour;
	public Texture2D blueDarkColour;
	public Texture2D blackColour;
	public Texture2D whiteColour;

	// Use this for initialization
	void Start () 
	{
		carIndex = 0;
		prevCarIndex = -1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		moveOutOfSight();
		displayCar ();
	}
	
	void OnGUI()
	{
		if (GUI.Button( new Rect(Screen.width / 8 - 50, Screen.height / 2, 46, 46) , arrowLeft))
		{
			prevCarIndex = carIndex;
			if (carIndex > 0)
				carIndex--;
			else 
				carIndex = NO_OF_CARS - 1;
			moveOutOfSight();
			displayCar();
		}
		if (GUI.Button( new Rect(Screen.width * 7 / 8, Screen.height / 2, 46, 46) , arrowRight))
		{
			prevCarIndex = carIndex;
			if (carIndex < NO_OF_CARS - 1)
				carIndex++;
			else
				carIndex = 0;
			moveOutOfSight();
			displayCar();
		}

		//Display "Back" and "Play" buttons.
		if (GUI.Button (new Rect(20, Screen.height - 50, 50, 25) , "Back"))
			Application.LoadLevel(1);
		if (GUI.Button ( new Rect(Screen.width - 100, Screen.height - 50, 50, 25) , "Play"))
			Application.LoadLevel(2);

		//Draw colour textures.
		Rect bluePos = new Rect(Screen.width / 2 + 100, Screen.height - 150, 25, 25);
		Rect greenPos = new Rect(Screen.width / 2 + 135, Screen.height - 150, 25, 25);
		Rect yellowLightPos = new Rect(Screen.width / 2 + 170, Screen.height - 150, 25, 25);
		Rect yellowPos = new Rect(Screen.width / 2 + 205, Screen.height - 150, 25, 25);
		Rect purplePos = new Rect(Screen.width / 2 + 240, Screen.height - 150, 25, 25);
		Rect blueDarkPos = new Rect(Screen.width / 2 + 275, Screen.height - 150, 25, 25);
		Rect blackPos = new Rect(Screen.width / 2 + 310, Screen.height - 150, 25, 25);
		Rect whitePos = new Rect(Screen.width / 2 + 345, Screen.height - 150, 25, 25);

		GUI.DrawTexture(bluePos, blueColour);
		GUI.DrawTexture(greenPos, greenColour);
		GUI.DrawTexture(yellowLightPos, yellowLightColour);
		GUI.DrawTexture(yellowPos, yellowColour);
		GUI.DrawTexture(purplePos, purpleColour);
		GUI.DrawTexture(blueDarkPos, blueDarkColour);
		GUI.DrawTexture(blackPos, blackColour);
		GUI.DrawTexture(whitePos, whiteColour);
		
		//Assign selected colour.
		Event e = Event.current;
		if (e.type == EventType.MouseUp) {
			if(bluePos.Contains(e.mousePosition)) 
				assignColour(new Color(0.392f, 0.541f, 0.69f, 1));				
			else if (greenPos.Contains(e.mousePosition)) 
				assignColour (new Color(0.392f, 0.69f, 0.643f, 1));
			else if (yellowLightPos.Contains(e.mousePosition)) 
				assignColour (new Color(0.855f, 0.745f, 0.145f, 1));
			else if (yellowPos.Contains(e.mousePosition)) 
				assignColour (new Color(0.82f, 0.6f, 0, 1));
			else if (purplePos.Contains(e.mousePosition)) 
				assignColour (new Color(0.169f, 0.031f, 0.2f, 1));
			else if (blueDarkPos.Contains(e.mousePosition)) 
				assignColour (new Color(0.012f, 0.071f, 0.125f, 1));
			else if (blackPos.Contains(e.mousePosition)) 
				assignColour (new Color(0, 0, 0, 1));
			else if (whitePos.Contains(e.mousePosition)) 
				assignColour (new Color(1, 1, 1, 1));
		}
	}

	void displayCar()
	{
		switch(carIndex)
		{
			case 0: float newPositionAudi = Mathf.SmoothDamp (audi.transform.position.x, target.position.x, ref xVelocity, smoothTime);
					audi.transform.position = new Vector3(newPositionAudi, audi.transform.position.y, audi.transform.position.z);
					PlayerPrefs.SetString("CarModel","Audi");
					break; 
			case 1:	float newPositionFerrari = Mathf.SmoothDamp (ferrari.transform.position.x, target.position.x, ref xVelocity, smoothTime);
					ferrari.transform.position = new Vector3(newPositionFerrari, ferrari.transform.position.y, ferrari.transform.position.z); 
					PlayerPrefs.SetString("CarModel","Ferrari");
					break; 
		}
	}
	
	void moveOutOfSight()
	{
		switch(prevCarIndex)
		{
			case 0: float newPositionAudi = Mathf.SmoothDamp (audi.transform.position.x, targetOut.position.x, ref xVelocityOut, smoothTime);
					audi.transform.position = new Vector3(newPositionAudi, audi.transform.position.y, audi.transform.position.z); break; 
			case 1:	float newPositionFerrari = Mathf.SmoothDamp (ferrari.transform.position.x, targetOut.position.x, ref xVelocityOut, smoothTime);
					ferrari.transform.position = new Vector3(newPositionFerrari, ferrari.transform.position.y, ferrari.transform.position.z); break; 
		}
	}

	void assignColour(Color colour)
	{
		Transform carBody;
		if (carIndex == 0)
		{
			carBody = audi.Find ("Plane01/sub01");
			
			Material[] allMaterials = carBody.renderer.sharedMaterials; 
			for (int i = 0; i < allMaterials.Length; i++)
				if (allMaterials[i].name == "wire_026177148")
					allMaterials[i].color = colour;
		}
		else if (carIndex == 1)
		{
			carBody = ferrari.Find ("body");
			
			Material[] allMaterials = carBody.renderer.sharedMaterials; 
			for (int i = 0; i < allMaterials.Length; i++)
				if (allMaterials[i].name == "c0")
					allMaterials[i].color = colour;
		}
	}

}
