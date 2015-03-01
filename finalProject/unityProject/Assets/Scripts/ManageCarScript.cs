using UnityEngine;
using System.Collections;

public class ManageCarScript : MonoBehaviour {

	public Car audi;
	public Car ferrari;

	// Use this for initialization
	void Start () {
		string chosenCar = PlayerPrefs.GetString("CarModel");

		//Render other car model invisible.
		//Assign car to main camera.
		if (chosenCar == "Audi")
		{		
			audi.gameObject.SetActive(true);
			ferrari.gameObject.SetActive(false);
		}
		else if (chosenCar == "Ferrari")
		{
			audi.gameObject.SetActive(false);
			ferrari.gameObject.SetActive(true);
		}
	}
	
}
