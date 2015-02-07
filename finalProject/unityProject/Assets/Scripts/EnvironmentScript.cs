using UnityEngine;
using System.Collections;

public class EnvironmentScript : MonoBehaviour {

	public ParticleSystem rain;
	public Material sunnySkybox;
	public Material cloudySkybox;

	// Use this for initialization
	void Start () {
		rain.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Frustration: " + EmoAffectiv.frustrationScore);
		if (EmoAffectiv.frustrationScore >= 0.5f && EmoAffectiv.shortTermExcitementScore <= 0.5f)
			StartRain();
		else 
			StopRain();

		Debug.Log ("Excitement: " + EmoAffectiv.longTermExcitementScore);
	}

	void StartRain()
	{
		if (!rain.isPlaying)
		{
			RenderSettings.skybox = cloudySkybox;
			rain.Play ();
		}
	}

	void StopRain()
	{
		if (rain.isPlaying)
		{
			rain.Stop();
			RenderSettings.skybox = sunnySkybox;
		}
	}
}
