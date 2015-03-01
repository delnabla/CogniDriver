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
		if (GUI.Button ( new Rect(Screen.width - 100, Screen.height - 50, 50, 25) , "Play"))
			Application.LoadLevel(2);
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

}
