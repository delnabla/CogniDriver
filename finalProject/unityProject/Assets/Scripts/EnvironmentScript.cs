using UnityEngine;
using System.Collections;

public class EnvironmentScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Frustration: " + EmoAffectiv.frustrationScore);
		Debug.Log ("Excitement: " + EmoAffectiv.longTermExcitementScore);
	}
}
