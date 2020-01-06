using UnityEngine;
using System.Collections;

// This script is used in displaying the characters' icons and skills in the game

public class GuiDisplay : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// Description of skills
	private string spell1Descr;
	private string spell2Descr;

	// Textures for characters' icons
	public Texture character1;
	public Texture character2;
	public Texture character3;
	public Texture character4;
	public Texture character5;

	// Define skills and character icon GUI
	private Rect spell1Rect;
	private Rect spell2Rect;
	private Rect boxRect;
	private Rect labelRect;
	
	private Rect characterImage;

	public GUISkin mySkin;

	// Use this for initialization
	void Start () {
	
		spell1Rect = new Rect(nativeWidth/2 - 90, nativeHeight - 150, 60, 60);
		spell2Rect = new Rect(nativeWidth/2 - 30 , nativeHeight - 150, 60, 60);
		boxRect = new Rect(nativeWidth/2-90, nativeHeight-370, 250, 200);
		labelRect = new Rect(nativeWidth/2-70, nativeHeight-320, 230, 200);

		characterImage = new Rect (nativeWidth - (nativeWidth - 500), nativeHeight - 200, 200, 200);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Set custom GUI skin
		GUI.skin = mySkin;

		// References to other scripts
		SpawnScript sp = gameObject.GetComponent<SpawnScript>();
		CharacterSelection ts = gameObject.GetComponent<CharacterSelection>();

		// Check which player has been selected and draw the skills buttons and a box with a description for each of them 
		if(sp.spawned == true)
		{
			if(ts.selectedPlayer == 1)
			{
				GUI.Button(spell1Rect, ts.Ch1Skill1);
				GUI.Button(spell2Rect, ts.Ch1Skill2);

				spell1Descr = "Unleashes energy, gathered from the ground heat to greatly double his Attack Speed for 5sec.";
				spell2Descr = "Smashes the enemy with the Fist of Nature, relic of the Temple of Erth, dealing damage equal to Troll's Spell Damage.";

				GUI.Box(characterImage, character1);
			}
			if(ts.selectedPlayer == 2)
			{
				GUI.Button(spell1Rect, ts.Ch2Skill1);
				GUI.Button(spell2Rect, ts.Ch2Skill2);

				spell1Descr = "Cast an area of effect spell. Deal 20 damage to all enemies and heal all allies by 40.";
				spell2Descr = "Increase Speed by 50 and Mana regeneration by 5 for 5sec.";

				GUI.Box(characterImage, character2);
			}
			if(ts.selectedPlayer == 3)
			{
				GUI.Button(spell1Rect, ts.Ch3Skill1);
				GUI.Button(spell2Rect, ts.Ch3Skill2);

				spell1Descr = "Launches a big fire arrow. A powerful ability that duplicates the hero's spell power and deals that damage to the opponent.";
				spell2Descr = "Fires arrows in a cone, dealing 30 spell damage to multiple enemies, and slows the targets for 5sec";

				GUI.Box(characterImage, character3);
			}
			if(ts.selectedPlayer == 4)
			{
				GUI.Button(spell1Rect, ts.Ch4Skill1);
				GUI.Button(spell2Rect, ts.Ch4Skill2);

				spell1Descr = "Heals hero by 25. The healing received is always equal to Warlord's spell damage.";
				spell2Descr = "Stabs the target, dealing 3,5 bleed damage per second for 5 sec.";

				GUI.Box(characterImage, character4);
			}
			if(ts.selectedPlayer == 5)
			{
				GUI.Button(spell1Rect, ts.Ch5Skill1);
				GUI.Button(spell2Rect, ts.Ch5Skill2);

				spell1Descr = "Spawns ice shards in an area beneath his enemies, dealing 25 damage.";
				spell2Descr = "Releases strong wind from his potion, dealing half Spell Damage and slowing the enemy.";

				GUI.Box(characterImage, character5);
			}

			// Show the number of skill in the button
			GUI.Label(new Rect(nativeWidth/2 - 80, nativeHeight - 150, 60, 60), "1");
			GUI.Label(new Rect(nativeWidth/2 - 20, nativeHeight - 150, 60, 60), "2");

			// Hover over buttons
			if(spell1Rect.Contains(Event.current.mousePosition))
			{
				GUI.Box(boxRect, "Spell 1");
				GUI.Label(labelRect, spell1Descr);
			}
			if(spell2Rect.Contains(Event.current.mousePosition))
			{
				GUI.Box(boxRect, "Spell 2");
				GUI.Label(labelRect, spell2Descr);
			}

		}
	}
}
