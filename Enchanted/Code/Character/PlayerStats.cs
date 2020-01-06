using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	// Used in resizing all the GUI.
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// References to other scripts.
	private PointAndClickMovement moveScript;
	private HealthAndDamage HDScript;
	private CharacterSelection teamSelectionScript;
	private SpawnScript spawn;

	public float attackStat;
	public float spellStat;
	public float speedStat;

	// An array that contains one texture for each stat
	public Texture[] statsIcons = new Texture[4];

	private Rect statWindow;

	public GUISkin myGuiSkin;

	// Use this for initialization
	void Start () {
	
		// Define the player's statistics window.
		statWindow = new Rect (nativeWidth - (nativeWidth - 1150), nativeHeight - 200, 200, 230);

		if(networkView.isMine)
		{
			teamSelectionScript = GameObject.Find("SpawnManager").GetComponent<CharacterSelection>();
			Transform triggerTransform = transform.FindChild("Trigger");
			HDScript = triggerTransform.GetComponent<HealthAndDamage>();
			moveScript = transform.GetComponent<PointAndClickMovement>();
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();

			// Assign the player's stats when a character is selected.
			AssignPlayerStats();
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f));

		// Set custom skin
		GUI.skin = myGuiSkin;

		// The statistics window will be drawn when the player is spawned.
		if(spawn.spawned)
		{
			statWindow = GUI.Window(4, statWindow, windowFunc, "");
		}
	}

	public void AssignPlayerStats()
	{
		if(teamSelectionScript.selectedPlayer == 1)
		{
			// Stats for Troll
			attackStat = 8f;
			spellStat = 20f;
			speedStat = 4f;
		}			
		if(teamSelectionScript.selectedPlayer == 2)
		{
			// Stats for Necromancer
			attackStat = 10f;
			spellStat = 20f;
			speedStat = 5f;
		}
		if(teamSelectionScript.selectedPlayer == 3)
		{
			// Stats for Goblin
			attackStat = 20f;
			spellStat = 15f;
			speedStat = 6f;
		}
		if(teamSelectionScript.selectedPlayer == 4)
		{
			// Stats for Warlord
			attackStat = 12f;
			spellStat = 15;
			speedStat = 5f;
		}
		if(teamSelectionScript.selectedPlayer == 5)
		{
			// Stats for Demon
			attackStat = 15f;
			spellStat = 25f;
			speedStat = 5f;	
		}
	}

	void windowFunc(int WindowID)
	{
		GUI.DrawTexture (new Rect (20, 30, 30, 30), statsIcons [0]);
		GUI.DrawTexture (new Rect (20, 70, 30, 30), statsIcons [1]);
		GUI.DrawTexture (new Rect (20, 110, 30, 30), statsIcons [2]);
		GUI.DrawTexture (new Rect (20, 150, 30, 30), statsIcons [3]);

		GUI.Label(new Rect(60, 30, 200, 100), "Attack Damage: " + attackStat);
		GUI.Label(new Rect(60, 70, 200, 100), "Health: " + HDScript.maxHealth);
		GUI.Label(new Rect(60, 110, 200, 100), "Spell Damage: " + spellStat);
		GUI.Label(new Rect(60, 150, 200, 100), "Speed: " + moveScript.currentSpeed * 10);
	}

	// Potion functions, accessed by ItemEffect script.

	public void AttackDamagePotion()
	{
		attackStat = attackStat + 20f;
	}

	public void SpellDamagePotion()
	{
		spellStat = spellStat + 20f;
	}

}
