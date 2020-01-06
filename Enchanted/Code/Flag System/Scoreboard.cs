using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// Define the Scoreboard
	private Rect boardRect;
	private Rect middle;
	private float space = 33;

	// Scoreboard textures
	public Texture kills;
	public Texture deaths;
	public Texture redFlag;
	public Texture blueFlag;

	public GUISkin mySkin;

	// Score values
	public int redFlagScore;
	public int blueFlagScore;
	public int redKills;
	public int blueKills;
	public int redDeaths;
	public int blueDeaths;

	public bool redKill = false;
	public bool blueKill = false;
	public bool redDeath = false;
	public bool blueDeath = false;

	private SpawnScript spawn;

	// Use this for initialization
	void Start () {
	
		// Set starting score to 0
		redFlagScore = 0;
		blueFlagScore = 0;
		redKills = 0;
		blueKills = 0;
		redDeaths = 0;
		blueDeaths = 0;

		// References to other scripts
		spawn = gameObject.GetComponent<SpawnScript>();

		boardRect = new Rect(nativeWidth/2-200, 0, 400, 30);
		middle = new Rect(nativeWidth/2-5, 0, 10, 30);
	}
	
	// Update is called once per frame
	void Update () {
	
		UpdateKillsAndDeaths();
	}

	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Set custom GUI skin
		GUI.skin = mySkin;

		// Display both teams' score if the player has spawned into the game.
		if(spawn.spawned)
		{
			GUI.Box(boardRect, "");
			GUI.Button(middle, "");

			// Draw all the textures and the updated values on the Scoreboard
			GUI.DrawTexture(new Rect(nativeWidth/2-200, 0, space, 30), kills);
			GUI.Label(new Rect(nativeWidth/2-200+space, 0, space, 30), redKills.ToString()); 
			GUI.DrawTexture(new Rect(nativeWidth/2-200+2*space, 0, space, 30), deaths);
			GUI.Label(new Rect(nativeWidth/2-200+3*space, 0, space, 30), redDeaths.ToString()); 
			GUI.DrawTexture(new Rect(nativeWidth/2-200+4*space, 0, space, 30), redFlag);
			GUI.Label(new Rect(nativeWidth/2-200+5*space, 0, space, 30), redFlagScore.ToString());

			GUI.DrawTexture(new Rect(nativeWidth/2+15, 0, space, 30), blueFlag);
			GUI.Label(new Rect(nativeWidth/2+15+space, 0, space, 30), blueFlagScore.ToString());
			GUI.DrawTexture(new Rect(nativeWidth/2+15+2*space, 0, space, 30), deaths);
			GUI.Label(new Rect(nativeWidth/2+15+3*space, 0, space, 30), blueDeaths.ToString()); 
			GUI.DrawTexture(new Rect(nativeWidth/2+15+4*space, 0, space, 30), kills);
			GUI.Label(new Rect(nativeWidth/2+15+5*space, 0, space, 30), blueKills.ToString());
		}

	}

	// Access HealthAndDamage script and update kills and deaths score
	void UpdateKillsAndDeaths()
	{
		if(spawn.amIOnTheRedTeam && HealthAndDamage.updateScore)
		{
			HealthAndDamage.updateScore = false;
			blueKills = blueKills + 1;
			networkView.RPC("UpdateBlueKills", RPCMode.AllBuffered, blueKills);
			redDeaths = redDeaths + 1;
			networkView.RPC("UpdateRedDeaths", RPCMode.AllBuffered, redDeaths);
		}
		if(spawn.amIOnTheBlueTeam && HealthAndDamage.updateScore)
		{
			HealthAndDamage.updateScore = false;
			redKills = redKills + 1;
			networkView.RPC("UpdateRedKills", RPCMode.AllBuffered, redKills);
			blueDeaths = blueDeaths + 1;
			networkView.RPC("UpdateBlueDeaths", RPCMode.AllBuffered, blueDeaths);
		}

	}

	// Update kills and deaths score everywhere
	[RPC]
	void UpdateRedKills(int kills)
	{
		redKills = kills;
	}

	[RPC]
	void UpdateBlueKills(int kills)
	{
		blueKills = kills;
	}

	[RPC]
	void UpdateRedDeaths(int deaths)
	{
		redDeaths = deaths;
	}

	[RPC]
	void UpdateBlueDeaths(int deaths)
	{
		blueDeaths = deaths;
	}
}
