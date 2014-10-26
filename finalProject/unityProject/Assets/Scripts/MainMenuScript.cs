using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public bool isStart = false;
	public bool isTrain = false;
	public bool isStatistics = false;
	public bool isOptions = false;
	public bool isExit = false;
	
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
			Application.Quit ();
		else if (isStart)
			Application.LoadLevel(1);
	}
	

}//class
