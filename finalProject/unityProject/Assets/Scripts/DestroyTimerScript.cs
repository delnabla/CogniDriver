using UnityEngine;
using System.Collections;

public class DestroyTimerScript : MonoBehaviour {

	public float liveForSeconds = 2f;
	private float timer;

	// Use this for initialization
	void Start () {
		timer += Time.deltaTime;
		if (timer > liveForSeconds)
			Destroy (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
