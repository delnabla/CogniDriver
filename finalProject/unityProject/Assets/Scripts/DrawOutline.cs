using UnityEngine;

//This class is writtern by Berenger and it can be found here: http://wiki.unity3d.com/index.php/ShadowAndOutline
public static class DrawOutline
{
	public static void DrawTheOutline(Rect rect, string text, GUIStyle style, Color outColor, Color inColor, float size)
	{
		float halfSize = size * 0.5F;
		GUIStyle backupStyle = new GUIStyle(style);
		Color backupColor = GUI.color;
		
		style.normal.textColor = outColor;
		GUI.color = outColor;
		
		rect.x -= halfSize;
		GUI.Label(rect, text, style);
		
		rect.x += size;
		GUI.Label(rect, text, style);
		
		rect.x -= halfSize;
		rect.y -= halfSize;
		GUI.Label(rect, text, style);
		
		rect.y += size;
		GUI.Label(rect, text, style);
		
		rect.y -= halfSize;
		style.normal.textColor = inColor;
		GUI.color = backupColor;
		GUI.Label(rect, text, style);
		
		style = backupStyle;
	}
}