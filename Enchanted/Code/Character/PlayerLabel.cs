using UnityEngine;
using System.Collections;

// This script is attached to each player and it draws the health bar above the character.

public class PlayerLabel : MonoBehaviour {

	// The health bar texture is attached to this in the inspector.
	public Texture healthTex;
	public Texture barTex;

	// References to other scripts
	public Camera myCamera;
	private Transform myTransform;
	private Transform triggerTransform;
	private HealthAndDamage HDScript;
	
	// Determine where the health bar and player name will be drawn.
	private Vector3 worldPosition = new Vector3();
	private Vector3 screenPosition = new Vector3();

	// Define the health bar
	private int labelTop = 18;
	private int labelWidth = 110;
	private int labelHeight = 15;
	private int barTop = 2;
	private int healthBarHeight = 10;
	private int healthBarLeft = 110;
	private float healthBarLength;
	private float adjustment = 2.5f;
	
	// Used in displaying the player's name
	public string playerName;
	private GUIStyle myStyle = new GUIStyle();
	
	
	// Use this for initialization
	void Start () {
		
		// This script will run for all players.
		if(networkView.isMine == true || networkView.isMine == false)
		{
			myTransform = transform;
			myCamera = Camera.main;

			Transform triggerTransform = transform.FindChild("Trigger");
			HDScript = triggerTransform.GetComponent<HealthAndDamage>();
			
			// The font color of the GUIStyle depends on which team the player is on
			if(myTransform.tag == "Blue Team")
			{
				myStyle.normal.textColor = Color.cyan;
			}
			if(myTransform.tag == "Red Team")
			{
				myStyle.normal.textColor = Color.red;
			}

			myStyle.fontSize = 12;
			myStyle.fontStyle = FontStyle.Bold;
			myStyle.clipping = TextClipping.Overflow; // Allow the text to extend beyond the width of the label
		}
		else
		{
			enabled = false;
		}		
	}
	
	// Update is called once per frame
	void Update () {

		// The length of the health bar will depend on the the player's current health.
		// If the player's health falls below 1, the length of the health bar will remain 1.
		if(HDScript.myHealth < 1)
		{
			healthBarLength = 1;
		}
		if(HDScript.myHealth >= 1)
		{
			healthBarLength = (HDScript.myHealth / HDScript.maxHealth) * 100;
		}
	}
	
	
	void OnGUI()
	{
		// Display the player's name above the character. 
		worldPosition = new Vector3(myTransform.position.x, myTransform.position.y + adjustment,myTransform.position.z);
		
		// Convert the worldPosition to a point on the screen.
		screenPosition = myCamera.WorldToScreenPoint(worldPosition);

		// Draw the health bar and the gray bar behind it
		GUI.Box(new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop, 100, healthBarHeight), "");
		
		GUI.DrawTexture(new Rect(screenPosition.x - healthBarLeft /2, Screen.height - screenPosition.y - barTop, healthBarLength, healthBarHeight), healthTex);

		GUI.DrawTexture(new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop, 100, healthBarHeight), barTex);
		
		// Draw the player's name above them
		GUI.Label(new Rect(screenPosition.x - labelWidth /2,  Screen.height - screenPosition.y - labelTop, labelWidth, labelHeight), playerName, myStyle);
		
		
	}

	
}