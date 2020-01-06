using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// Variables that store the position and size of all the GUI elements
	private Rect EnterButton;
	private Rect windowRect;
	private Rect descriptionRect;
	private Rect descriptionLabel;
	private Rect buttonSkill1;
	private Rect buttonSkill2;

	// This variable prevents the player from joining the game without having chosen a character.
	public bool isClicked = false;

	public Texture Ch1Skill1, Ch1Skill2, Ch2Skill1, Ch2Skill2, Ch3Skill1, 
				   Ch3Skill2, Ch4Skill1, Ch4Skill2, Ch5Skill1, Ch5Skill2, skill1, skill2;
		
	public int selectedPlayer;

	// Variables that store all character prefabs so that they spawn in the game if the player picks them from the character selection menu
	public GameObject RedCharacter1;
	public GameObject RedCharacter2;
	public GameObject RedCharacter3;
	public GameObject RedCharacter4;
	public GameObject RedCharacter5;

	public GameObject BlueCharacter1;
	public GameObject BlueCharacter2;
	public GameObject BlueCharacter3;
	public GameObject BlueCharacter4;
	public GameObject BlueCharacter5;

	// Each Spotlight is enabled when the player clicks on a character
	public Light SpotlightTroll;
	public Light SpotlightNecro;
	public Light SpotlightGoblin;
	public Light SpotlightWarlord;
	public Light SpotlightDemon;

	// Variables used in character description window
	private string description;
	private string skill1_Description;
	private string skill2_Description;
	private string window_Title;

	private string buttonName = "ENTER";

	// References to other scripts
	private SpawnScript spawn;

	// Audio
	private SoundEffects soundEffects;

	public GUISkin mySkin;
	
	// Use this for initialization
	void Start () 
	{
		spawn = gameObject.GetComponent<SpawnScript>();

		soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();

		windowRect = new Rect (20, 300, 400, 600);
		descriptionRect = new Rect (20, 400, 350, 150);
		descriptionLabel = new Rect (40, 430, 330, 150);
		buttonSkill1 = new Rect (20, 100, 60, 60);
		buttonSkill2 = new Rect (85, 100, 60, 60);
		EnterButton = new Rect(nativeWidth/2 - 50 , nativeHeight/2 + 400, 100, 60);
	}
	
	// Update is called once per frame
	void Update () 
	{
		CharactersSelection ();
	}

	// If the player clicks on a character with a specific tag, the Spotlight is enabled and the right prefab is stored in a
	// variable from SpawnScript (redTeamPlayers/blueTeamPlayers).
	public void CharactersSelection()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, 100))
			{
				if(hit.collider.name == "Troll(Clone)")
				{
					SelectedChatacter(1, RedCharacter1, BlueCharacter1);
					GameObject light = GameObject.Find("SpotLightTroll");
					SpotlightTroll = light.GetComponent<Light>();
					EnableSpotlight(true, false, false, false, false);
				}
				
				if(hit.collider.name == "Necromancer(Clone)")
				{
					SelectedChatacter(2, RedCharacter2, BlueCharacter2);
					GameObject light = GameObject.Find("SpotLightNecro");
					SpotlightNecro = light.GetComponent<Light>();
					EnableSpotlight(false, true, false, false, false);
				}
				
				if(hit.collider.name == "Goblin(Clone)")
				{
					SelectedChatacter(3, RedCharacter3, BlueCharacter3);
					GameObject light = GameObject.Find("SpotLightGoblin");
					SpotlightGoblin = light.GetComponent<Light>();
					EnableSpotlight(false, false, true, false, false);
				}
				
				if(hit.collider.name == "Warlord(Clone)")
				{
					SelectedChatacter(4, RedCharacter4, BlueCharacter4);
					GameObject light = GameObject.Find("SpotLightWarlord");
					SpotlightWarlord = light.GetComponent<Light>();
					EnableSpotlight(false, false, false, true, false);
				}
				
				if(hit.collider.name == "Demon(Clone)")
				{
					SelectedChatacter(5, RedCharacter5, BlueCharacter5);
					GameObject light = GameObject.Find("SpotLightDemon");
					SpotlightDemon = light.GetComponent<Light>();
					EnableSpotlight(false, false, false, false, true);
				}
				
			}
			else
			{
				return;
			}	
			
		}
	}

	// Check which team the player is on, so that the correct prefab will be chosen to spawn.
	public void SelectedChatacter(int player, GameObject redCharacter, GameObject blueCharacter)
	{
		soundEffects.PlayAudio(4);
		selectedPlayer = player;

		if(spawn.amIOnTheRedTeam == true)
		{ 
			spawn.redTeamPlayers = redCharacter;	
		}
		if(spawn.amIOnTheBlueTeam == true)
		{
			spawn.blueTeamPlayers = blueCharacter;
		}

		isClicked = true;

	}

	// When the player clicks on a character, its Spotlight will be enabled but all the others have to be disabled.
	public void EnableSpotlight(bool trollLight, bool necromancerLight, bool goblinLight, bool warlordLight, bool demonLight)
	{
		Debug.Log("light");
		SpotlightTroll.light.enabled = trollLight;
		SpotlightNecro.light.enabled = necromancerLight;
		SpotlightGoblin.light.enabled = goblinLight;
		SpotlightWarlord.light.enabled = warlordLight;	
		SpotlightDemon.light.enabled = demonLight;
	}
	
	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f));

		if(isClicked)
		{
			// Set custom GUI skin
			GUI.skin = mySkin;

			// If the Enter button is clicked and a character has been chosen, the player spawns in the game and all character
			// prefabs are destroyed from the character selection menu. 
			if(GUI.Button(EnterButton, buttonName))
			{
				soundEffects.PlayAudio(4);

				if(spawn.amIOnTheRedTeam)
				{
					spawn.SpawnRedTeamPlayer();

					Destroy(GameObject.Find("Troll(Clone)"));
					Destroy(GameObject.Find("Goblin(Clone)"));
					Destroy(GameObject.Find("Warlord(Clone)"));
					Destroy(GameObject.Find("Necromancer(Clone)"));
					Destroy(GameObject.Find("Demon(Clone)"));
				}
				
				if(spawn.amIOnTheBlueTeam)
				{
					spawn.SpawnBlueTeamPlayer();

					Destroy(GameObject.Find("Troll(Clone)"));
					Destroy(GameObject.Find("Goblin(Clone)"));
					Destroy(GameObject.Find("Warlord(Clone)"));
					Destroy(GameObject.Find("Necromancer(Clone)"));
					Destroy(GameObject.Find("Demon(Clone)"));
				}
				enabled = false;
			}

			// Character and skills description
			if(selectedPlayer == 1)
			{
				description = "A fearless creature born from mud and ashes of a mythical burnt forest, came to life to defend nature.";
				skill1_Description = "Unleashes energy, gathered from the ground heat to greatly double his Attack Speed for 5sec.";
				skill2_Description = "Smashes the enemy with the Fist of Nature, relic of the Temple of Erth, dealing damage equal to Troll's Spell Damage.";
				window_Title = "TROLL";

				skill1 = Ch1Skill1;
				skill2 = Ch1Skill2;

				windowRect = GUI.Window(2, windowRect, WindowFunction, window_Title);
			}			
			if(selectedPlayer == 2)
			{
				description = "Necromancer casts powerful sorceries, increasing his effectiveness by the spirit of the deceased. Using this extremely rare power, Necromancer can be very useful in team battles.";
				skill1_Description = "Cast an area of effect spell. Deal 20 damage to all enemies and heal all allies by 40.";
				skill2_Description = "Increase Speed by 50 and Mana regeneration by 5 for 5sec.";
				window_Title = "NECROMANCER";

				skill1 = Ch2Skill1;
				skill2 = Ch2Skill2;

				windowRect = GUI.Window(2, windowRect, WindowFunction, window_Title);
			}
			if(selectedPlayer == 3)
			{
				description = "A master of the archery. Picking the right target with precision, the Goblin can strike the opponent dealing extreme amounts of damage while slowing the opponent";
				skill1_Description = "Launch a big fire arrow. A powerful ability that duplicates the hero's spell power and deals that damage to the opponent.";
				skill2_Description = "Fire arrows in a cone, dealing 30 spell damage to multiple enemies, and slows the targets for 5sec";
				window_Title = "GOBLIN";

				skill1 = Ch3Skill1;
				skill2 = Ch3Skill2;

				windowRect = GUI.Window(2, windowRect, WindowFunction, window_Title);
			}
			if(selectedPlayer == 4)
			{
				description = "Commander of Reptilians, Warlord is against to anyone who stands kati conquering the Enchanted Forest.";
				skill1_Description = "Heals hero by 25. The healing received is always equal to Warlord's spell damage.";
				skill2_Description = "Stabs the target, dealing 3,5 bleed damage per second for 5 sec.";
				window_Title = "WARLORD";
				
				skill1 = Ch4Skill1;
				skill2 = Ch4Skill2;

				windowRect = GUI.Window(2, windowRect, WindowFunction, window_Title);
			}
			if(selectedPlayer == 5)
			{
				description = "Through his travels, seeking the mysteries behind demon creation, Demon became wise and strong and managed to craft and manipulate the elements of Nature.";
				skill1_Description = "Spawns ice shards in an area beneath his enemies, dealing 25 damage.";
				skill2_Description = "Releases strong wind from his potion, dealing half Spell Damage and slowing the enemy.";
				window_Title = "DEMON";
				
				skill1 = Ch5Skill1;
				skill2 = Ch5Skill2;

				windowRect = GUI.Window(2, windowRect, WindowFunction, window_Title);
			}
		}
	}

	// A description window appears when the player clicks on a character
	void WindowFunction(int WindowID)
	{

		GUI.Label(new Rect(140, 40, 200, 200), "SPELLS AND STATS");
		GUI.Button(buttonSkill1, skill1);
		GUI.Button(buttonSkill2, skill2);
		GUI.Box(descriptionRect, "Description");
		GUI.Label (descriptionLabel, description);

		// Hovering over the skills buttons, a description of each skill appears on the window
		if(buttonSkill1.Contains(Event.current.mousePosition))
		{
			GUI.Label(new Rect(20, 200, 330, 200), skill1_Description);
		}
		
		if(buttonSkill2.Contains(Event.current.mousePosition))
		{
			GUI.Label(new Rect(20, 200, 330, 200), skill2_Description);
		}
	}

}
