using UnityEngine;
using System.Collections;


// This script is attached to the player and allows the player to see a box with 
// their health to the lower right of the screen

public class StatDisplay : MonoBehaviour {

	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// The health bar texture is attached to this in the inspector
	public GUISkin mySkin;

	public Texture healthTex;
	public Texture manaTex;

	// These are used in calculating and displaying the health and mana

	private float health;
	private int healthForDisplay;

	private float mana;
	private int manaForDisplay;

	// These are used in defining the StatDisplay box 

	private int boxWidth = 450;
	private int boxHeight = 90;
	private int labelHeight = 450;
	private int labelWidth = 90;
	private int padding = 12;
	private float healthBarLength;
	private int healthBarHeight = 30;
	private float manaBarLength;
	private int manaBarHeight = 30;

	//private GUIStyle healthStyle = new GUIStyle();
	private float commonLeft;
	private float commonTop;

	// A quick reference to the HealthAndDamage script

	private HealthAndDamage HDScript;
	private Mana manaScript;


	// Use this for initialization
	void Start () {
	
		if(networkView.isMine == true)
		{
			// Access HealthAndDamage script and the Mana script

			Transform triggerTransform = transform.FindChild("Trigger");

			HDScript = triggerTransform.GetComponent<HealthAndDamage>();
			manaScript = gameObject.GetComponent<Mana>();

			// Set the GuiStyle

			//healthStyle.normal.textColor = Color.black;
			//healthStyle.fontStyle = FontStyle.Bold;

		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		// Access the HealthAndDamage script continuously and retrieve player's current health

		health = HDScript.myHealth;
		mana = manaScript.myMana;

		// Display health as integer

		healthForDisplay = Mathf.CeilToInt(health);
		manaForDisplay = Mathf.CeilToInt(mana);

		// Calculate how log the health bar should be. The max health is 100
		// representing 100%

		healthBarLength = (health / HDScript.maxHealth) * 430;
		manaBarLength = (mana / manaScript.maxMana) * 430;

	}

	void OnGUI()
	{
		//GuiResize.AutoResize(1920, 1080);
		GUI.skin = mySkin;

		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 


		commonLeft = nativeWidth - (nativeWidth - 700);
		commonTop = nativeHeight - 90;

		// Draw a plain box behind the health bar

		GUI.Box(new Rect(commonLeft, commonTop, boxWidth, boxHeight), "");

		// Draw a gray box behind the health bar

		GUI.Box(new Rect(commonLeft + 10, commonTop + padding, 430, healthBarHeight), "");
		GUI.Box(new Rect(commonLeft + 10, commonTop + padding + manaBarHeight + 5, 430, manaBarHeight), "");

		// Draw the health bar and make it's leght be dependant on the player#s
		// current health

		GUI.DrawTexture(new Rect(commonLeft + 10, commonTop + padding, healthBarLength, healthBarHeight), healthTex);
		GUI.DrawTexture(new Rect(commonLeft + 10, commonTop + padding + manaBarHeight + 5, manaBarLength, manaBarHeight), manaTex);

		GUI.Label(new Rect(commonLeft + 210, commonTop + 20, labelWidth, labelHeight), 
		          healthForDisplay.ToString()+"/" + HDScript.maxHealth);

		GUI.Label(new Rect(commonLeft + 210, commonTop + 55, labelWidth, labelHeight), 
			         manaForDisplay.ToString()+"/100");
	}





}
