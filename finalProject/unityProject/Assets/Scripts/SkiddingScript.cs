using UnityEngine;
using System.Collections;

//The credit for this class is mostly to FlatTutorials: https://www.youtube.com/watch?v=sIM2_tDGRRY&list=PL67XFC3MYQ6K0PXSad15xFrhxNL76r4Te&index=18
public class SkiddingScript : MonoBehaviour {

	public float currentFriction;
	private float skidAt = 7.5f;
	public GameObject skidSound;
	public float soundEmition = 15;
	private float soundWait;
	private float soundWaitReverse;
	public float markWidth = 1;
	private int skidding;
	private Vector3[] lastPos = new Vector3[2];
	public Material skidMaterial;
	public GameObject skidSmoke;
	public float smokeDepth = 0.4f;
	public bool backWheel = false;
	public GameObject reverseSound;

	// Use this for initialization
	void Start () 
	{ 
		skidSmoke.transform.position = new Vector3(transform.position.x, transform.position.y - smokeDepth, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
		WheelHit hit;
		string currentAction = EmoCognitiv.getCurrentAction();
		WheelCollider wheel = (WheelCollider)transform.GetComponent ("WheelCollider");
		wheel.GetGroundHit (out hit);
		currentFriction = Mathf.Abs (hit.sidewaysSlip);
		float rpm = transform.GetComponent<WheelCollider> ().rpm;
		if ((skidAt <= currentFriction) || (rpm < 300 && (Input.GetAxis("Vertical") > 0 || currentAction == "COG_PUSH") && backWheel && hit.collider )) 
		{
			if (soundWait <= 0)
			{
				skidSound.audio.volume = MainMenuScript.sfxVolume / 10;
				Instantiate (skidSound, hit.point, Quaternion.identity);
				soundWait = 1;
			}
		}

		//Reverse sound.
		if (Input.GetAxis ("Vertical") < 0 || currentAction == "COG_PULL")
		{
			if (soundWaitReverse <= 0)
			{
				reverseSound.audio.volume = MainMenuScript.sfxVolume / 10 * 0; //delete 0 to enable.
				Instantiate (reverseSound, hit.point, Quaternion.identity);
				soundWaitReverse = 10;
			}
		}

		soundWait -= Time.deltaTime * soundEmition;
		soundWaitReverse -= Time.deltaTime * soundEmition;

		if (skidAt <= currentFriction || (rpm < 300 && (Input.GetAxis("Vertical") > 0 || currentAction == "COG_PUSH") && backWheel && hit.collider)) 
		{
			skidSmoke.particleEmitter.emit = true;
			SkidMesh ();
		} else {
			skidSmoke.particleEmitter.emit = false;
			skidding = 0;
		}
	}

	void SkidMesh()
	{
		WheelHit hit;
		WheelCollider wheel = (WheelCollider)transform.GetComponent ("WheelCollider");
		wheel.GetGroundHit (out hit);
		GameObject mark = new GameObject ("Mark");
		MeshFilter filter = mark.AddComponent<MeshFilter>();
		mark.AddComponent<MeshRenderer>();
		Mesh markMesh = new Mesh ();
		Vector3[] vertices = new Vector3[4];
		if (skidding == 0) {
			skidding = 1;
			vertices[0] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * (new Vector3(markWidth, 0.01f, 0));
			vertices[1] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * (new Vector3(-markWidth, 0.01f, 0));
		} else {
			vertices[1] = lastPos[0];
			vertices[0] = lastPos[1];
		}
		vertices[2] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * (new Vector3(-markWidth, 0.01f, 0));
		vertices[3] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * (new Vector3(markWidth, 0.01f, 0));
		lastPos [0] = vertices[2];
		lastPos [1] = vertices[3];
		int[] triangles = new int[6]{0, 1, 2, 2, 3, 0};
		markMesh.vertices = vertices;
		markMesh.triangles = triangles;
		markMesh.RecalculateNormals();
		Vector2[] uvmap = new Vector2[4];
		uvmap [0] = new Vector2 (1, 0);
		uvmap [1] = new Vector2 (0, 0);
		uvmap [2] = new Vector2 (0, 1);
		uvmap [3] = new Vector2 (1, 1);
		markMesh.uv = uvmap;
		filter.mesh = markMesh;
		mark.renderer.material = skidMaterial;
		mark.AddComponent<DestroyTimerScript>();
	}

} //class
