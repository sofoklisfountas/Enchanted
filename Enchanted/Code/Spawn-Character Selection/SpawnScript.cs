using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This script is attached to the SpawnManager GameObject and it allows the player to spawn

public class SpawnScript : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	public GameObject redspawn;
	public GameObject bluespawn;
	
	// Characters Prefabs
	public Transform TrollPref;
	public Transform NecroPref;
	public Transform GoblinPref;
	public Transform WarlordPref;
	public Transform DemonPref;

	// Determine if the player needs to spawn into the game
	public bool justConnectedToServer = false;
	
	// Determine which team the player is on
	public bool amIOnTheRedTeam = false;
	public bool amIOnTheBlueTeam = false;
	
	// Used to define the JoinTeamWindow
	private Rect joinTeamRect;
	private string joinTeamWindowTitle = "TEAM SELECTION";
	private float joinTeamWindowWidth = 330;
	private float joinTeamWindowHeight = 150;
	private float joinTeamLeftIndent;
	private float joinTeamTopIndent;
	private float buttonHeight = 40;
	
	// The Player prefabs are connected to these in the inspector

	public GameObject redTeamPlayers;
	public GameObject blueTeamPlayers;

	private int redTeamGroup = 0;
	private int blueTeamGroup = 1;
	
	// Used to capture spawn points
	private GameObject[] redSpawnPoints;
	private GameObject[] blueSpawnPoints;

	// Used to determine whether the player is destroyed 
	public bool iAmDestroyed = false;
	
	// Used in determining if the player has spawned for the first time
	public bool firstSpawn = false;

	public bool spawned = false;

	// Reference to other scripts
	private Scoreboard sc;

	// Audio
	private SoundEffects soundEffects;

	public GUISkin mySkin;

	// Use this for initialization
	void Start () 
	{
		sc = gameObject.GetComponent<Scoreboard>();
		soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	// Unity function

	void OnConnectedToServer()
	{
		justConnectedToServer = true;	
	}
	
	void JoinTeamWindow(int windowID)
	{
		if(justConnectedToServer == true)
		{
			// if the player clicks on the Join Red/Blue Team button then assign them to the red/blue team and wait for them to chose a character
			// so that the selected prefab will be stored in the redTeamPlayers/blueTeamPlayers variable. This action is performed in the 
			// TeamSelection script.
			GUILayout.BeginVertical();
			GUILayout.Space(20);
			if(GUILayout.Button("Join Red Team", GUILayout.Height(buttonHeight)))
			{
				soundEffects.PlayAudio(4);

				amIOnTheRedTeam = true;
				justConnectedToServer = false;

				Instantiate(TrollPref, TrollPref.transform.position, TrollPref.transform.rotation);
				Instantiate(NecroPref, NecroPref.transform.position, NecroPref.transform.rotation);
				Instantiate(GoblinPref, GoblinPref.transform.position, GoblinPref.transform.rotation);
				Instantiate(WarlordPref, WarlordPref.transform.position, WarlordPref.transform.rotation);
				Instantiate(DemonPref, DemonPref.transform.position, DemonPref.transform.rotation);

				firstSpawn = true;
			}
			
			if(GUILayout.Button("Join Blue Team", GUILayout.Height(buttonHeight)))
			{
				soundEffects.PlayAudio(4);

				amIOnTheBlueTeam = true;
				justConnectedToServer = false;

				Instantiate(TrollPref, TrollPref.transform.position, TrollPref.transform.rotation);
				Instantiate(NecroPref, NecroPref.transform.position, NecroPref.transform.rotation);
				Instantiate(GoblinPref, GoblinPref.transform.position, GoblinPref.transform.rotation);
				Instantiate(WarlordPref, WarlordPref.transform.position, WarlordPref.transform.rotation);
				Instantiate(DemonPref, DemonPref.transform.position, DemonPref.transform.rotation);

				firstSpawn = true;
			}
			GUILayout.EndVertical();
		}
		
		// Allow the player to respawn if they were destroyed
		if(iAmDestroyed == true)
		{
			if(GUILayout.Button("Respawn", GUILayout.Height(buttonHeight*3)))
			{
				soundEffects.PlayAudio(4);

				if(amIOnTheRedTeam == true)
				{
					SpawnRedTeamPlayer();
					sc.redDeath = true;
					sc.blueKill = true;
				}
				if(amIOnTheBlueTeam == true)
				{
					SpawnBlueTeamPlayer();
					sc.blueDeath = true;
					sc.redKill = true;
				}
				
				iAmDestroyed = false;
			}
		}
	}
	
	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Set custom GUI skin
		GUI.skin = mySkin;

		//If the player has just connected to the server then draw the Join Team window.
		if(justConnectedToServer == true || iAmDestroyed == true)
		{
			joinTeamLeftIndent = nativeWidth / 2 - joinTeamWindowWidth / 2;
			joinTeamTopIndent = nativeHeight / 2 - joinTeamWindowHeight / 2;
			joinTeamRect = new Rect(joinTeamLeftIndent, joinTeamTopIndent, joinTeamWindowWidth, joinTeamWindowHeight);
			joinTeamRect = GUILayout.Window(1, joinTeamRect, JoinTeamWindow, joinTeamWindowTitle);
		}
	}
	
	public void SpawnRedTeamPlayer(){
		
		// Find all red spawn points and place a reference to them in the array redSpawnPoints
		redSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnRedTeam");

		// If it's not the first spawn, the spawn points are different
		if(iAmDestroyed)
		{
			redSpawnPoints = GameObject.FindGameObjectsWithTag("RespawnRedTeam");
		}
		
		// Randomly select one of those spawnpoints.
		GameObject randomRedSpawn = redSpawnPoints[Random.Range(0, redSpawnPoints.Length)];
		
		// Instantiate the player at the randomly selected spawn point
		redspawn = (GameObject)Network.Instantiate(redTeamPlayers, randomRedSpawn.transform.position,
		                                           randomRedSpawn.transform.rotation, redTeamGroup);
		spawned = true;
	}
	
	public void SpawnBlueTeamPlayer(){
		
		// find all blue spawn points and place a reference to them in the array redSpawnPoints
		
		blueSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnBlueTeam");

		// If it's not the first spawn, the spawn points are different
		if(iAmDestroyed)
		{
			blueSpawnPoints = GameObject.FindGameObjectsWithTag("RespawnBlueTeam");
		}
		
		// Randomly select one of those spawnpoints.
		GameObject randomBlueSpawn = blueSpawnPoints[Random.Range(0, blueSpawnPoints.Length)];
		
		// Instantiate the player at the randomly selected spawn point
		bluespawn = (GameObject)Network.Instantiate(blueTeamPlayers , randomBlueSpawn.transform.position,
		                                            randomBlueSpawn.transform.rotation, blueTeamGroup);
		spawned = true;
	}
}
