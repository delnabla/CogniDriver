using UnityEngine;
using System.Collections;

public class EnvironmentScript : MonoBehaviour {

	public ParticleSystem rain;
	public Material sunnySkybox;
	public Material cloudySkybox;
	public AudioClip rainSound;

	private static float rainSoundVolume = 1 * MainMenuScript.sfxVolume / 10;

	// Use this for initialization
	void Start () {
		rain.Stop();
		audio.clip = rainSound;
		audio.volume = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Frustration: " + EmoAffectiv.frustrationScore + " Excitement: " + EmoAffectiv.shortTermExcitementScore);
		if (EmoAffectiv.frustrationScore >= 0.5f && EmoAffectiv.shortTermExcitementScore <= 0.5f)
			StartRain();
		else 
			StopRain();
	}

	void StartRain()
	{
		if (!rain.isPlaying)
		{
			RenderSettings.skybox = cloudySkybox;
			audio.clip = rainSound;
			audio.Play();
			RainFadeIn();
			rain.Play (); //play the particle system animation.
		}
	}

	void StopRain()
	{
		if (rain.isPlaying)
		{
			RainFadeOut ();
			audio.Stop();
			rain.Stop();
			RenderSettings.skybox = sunnySkybox;
		}
	}

	void RainFadeIn()
	{
		if (rainSoundVolume < 1 * MainMenuScript.sfxVolume / 10)
		{
			rainSoundVolume += 0.1f * Time.deltaTime;
			audio.volume = rainSoundVolume;
		}
	}

	void RainFadeOut()
	{
		if (rainSoundVolume > 0)
		{
			rainSoundVolume -= 0.1f * Time.deltaTime;
			audio.volume = rainSoundVolume;
		}
	}
}
