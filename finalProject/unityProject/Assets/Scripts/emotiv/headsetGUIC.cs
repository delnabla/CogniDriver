// This code belongs to Emotiv with small modifications on how we display the data and it's been rewritten in C# from Javascript.

using UnityEngine;
using System.Collections;

public class headsetGUIC : MonoBehaviour {

	EmoEngine engine = EmoEngine.Instance;
	public float int_scale = 0.7f;
	private Rect headArea;
	private Rect head;
	public static int[] node;

	public Texture2D headset;
	public Texture2D redButton;
	public Texture2D blackButton;
	public Texture2D orangeButton;
	public Texture2D yellowButton;
	public Texture2D greenButton;
	public bool isEnabled = true;

	// Use this for initialization
	void Start () {

		node = new int[18];
		for (int i = 0; i < 18; i++)
		{
			node[i] = 0;
		}  
		
		headArea = new Rect(10, 15, 225 * int_scale, 200 * int_scale);
		if(head == new Rect(0,0,0,0))
		{
			head = new Rect(0, 0, 200 * int_scale, 200 * int_scale);
		}
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Updating...");
		Debug.Log("nChan: " + EmoEngineInst.nChan);
		for (int i = 0 ; i< EmoEngineInst.nChan ;i++)
		{
			node[i] = EmoEngineInst.Cq[i];
			Debug.Log("node[" + i + "]: " + node[i]);
		}  
	}

	void OnGUI()
	{
		isEnabled = GUI.Toggle(new Rect(5,5,100,50), isEnabled, "Show info");
		if (isEnabled) DrawGUI();
	}

	void DrawGUI(){
		
		GUI.BeginGroup (headArea);
		GUI.DrawTexture ( head , headset);
		
		GUI.DrawTexture ( new Rect(47*int_scale, 26*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[3]));
		GUI.DrawTexture ( new Rect(130*int_scale, 26*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[16]));
		
		GUI.DrawTexture ( new Rect(67*int_scale, 51*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[5]));
		GUI.DrawTexture ( new Rect(110*int_scale, 51*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[14]));
		
		GUI.DrawTexture ( new Rect(18*int_scale, 53*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[4]));
		GUI.DrawTexture ( new Rect(159*int_scale, 53*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[15]));
		
		GUI.DrawTexture ( new Rect(37*int_scale, 71*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[5]));
		GUI.DrawTexture ( new Rect(141*int_scale, 71*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[13]));
		
		// T7 T8
		GUI.DrawTexture ( new Rect(8*int_scale, 93*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[7]));
		GUI.DrawTexture ( new Rect(169*int_scale, 93*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[12]));
		
		//CMS
		GUI.DrawTexture ( new Rect(18*int_scale, 118*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[0]));
		GUI.DrawTexture ( new Rect(159*int_scale, 118*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[1]));
		
		GUI.DrawTexture ( new Rect(37*int_scale, 148*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[8]));
		GUI.DrawTexture ( new Rect(140*int_scale, 148*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[11]));
		
		GUI.DrawTexture ( new Rect(64*int_scale, 172*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[9]));
		GUI.DrawTexture ( new Rect(113*int_scale, 172*int_scale, 23*int_scale, 23*int_scale) , nodeStatus(node[10]));
		
		GUI.EndGroup(); 
	}

	Texture2D nodeStatus(int node)
	{
		Texture2D returnButton;
		switch (node)
		{ 
		case 0:
			returnButton = blackButton;
			break;
		case 1:
			returnButton = redButton; 
			break; 
		case 2:
			returnButton = orangeButton;
			break; 
		case 3:
			returnButton = yellowButton; 
			break; 
		case 4:
			returnButton = greenButton; 
			break; 
		default:
			returnButton = blackButton;
			break;			
		}
		return returnButton; 
	}
	
	void DisableInfo()
	{
		isEnabled = false;
	}
	
	void EnableInfo()
	{
		isEnabled = true;
	}
} //class
