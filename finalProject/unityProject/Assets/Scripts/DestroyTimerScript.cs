using UnityEngine;
using System.Collections;

public class DestroyTimerScript : MonoBehaviour {

	public float liveForSeconds = 2f;
	private float timer;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, liveForSeconds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
