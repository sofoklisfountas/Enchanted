using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	
	public float fadeSpeed = 0.8f;		// The fading speed
	private int drawDepth = -1000;		// The texture's draw order in the hierarchy. Low number means it renders on top
	private float alpha = 1.0f;			// The texture's alpha value between 0 and 1
	private int fadeDirection = -1;		// The direction to fade: in = -1 or out = 1

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		// Fade in/out the alpha value using a direction, a speed and Time.deltaTime to convert the operation to seconds
		alpha += fadeDirection * fadeSpeed * Time.deltaTime;
		
		// Force (clamp) the number between 0 and 1 because GUI.color uses alpha values between 0 and 1
		alpha = Mathf.Clamp01 (alpha);
		
		// Set color of the GUI (the Texture). All color values remain the same and the Alpha is se to the alpha variable
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);  // Set the alpha value
		GUI.depth = drawDepth;												   // Make the black texture render on top (drawn last)
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	public float BeginFade(int direction)
	{
		fadeDirection = direction;
		return(fadeSpeed);
	}
	
	void OnLevelWasLoaded()
	{
		BeginFade (-1);
	}
}
