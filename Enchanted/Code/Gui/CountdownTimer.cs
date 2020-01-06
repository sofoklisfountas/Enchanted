using UnityEngine;
using System.Collections;

public class CountdownTimer : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// Set the duration of the gameplay
	private float seconds = 59f;
	private float minutes = 14f;

	// All players will have a specific amount of gold when they spawn for the first time
	public static float myGold = 500.0f;

	private bool endGame = false;
	private bool endGameAudio = false;

	// Define the window that appears when the game ends
	private Rect boxRect;
	private string winningMessage;

	// References to other scripts
	private Scoreboard sc;
	private SpawnScript spawn;

	// Custom Gui Skins
	public GUISkin mySkin;
	public GUISkin mySkinEndGame;

	// Audio - references to other scripts
	private SoundEffects soundEffects;
	private BackgroundMusic backgroundAudio;

	// Use this for initialization
	void Start () 
	{
		sc = gameObject.GetComponent<Scoreboard>();
		spawn = gameObject.GetComponent<SpawnScript>();
		soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();
		backgroundAudio = GameObject.Find("AudioManager").GetComponent<BackgroundMusic>();

		winningMessage = "";

		boxRect = new Rect(nativeWidth/2-400, nativeHeight/2-250, 800, 500);
	}
	
	// Update is called once per frame
	void Update () 
	{
		countDown();
		GoldCount();
		EndGame();
	}

	void OnGUI()
	{
		// Custom GUI skin
		GUI.skin = mySkin;

		if(spawn.spawned)
		{
			// Resize Gui
			Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
			GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

			// Draw a label that shows the countdown timer
			GUI.Label(new Rect(nativeWidth/2-40, nativeHeight - (nativeHeight - 20), 100, 100), minutes.ToString("f0") + ":" + seconds.ToString("f0"));

			// When the game ends, a box will appear showing the score of each team (flags captured, kills and deaths) and 
			// which team won.
			if(endGame)
			{
				GUI.skin = mySkinEndGame;

				GUI.Box(boxRect, "END GAME");
				GUI.Label(new Rect(nativeWidth/2-50, nativeHeight/2-200, 200, 30), winningMessage);
				GUI.Button(new Rect(nativeWidth/2-5, nativeHeight/2-150, 10, 300), "");

				GUI.Label(new Rect(nativeWidth/2-250, nativeHeight/2-100, 150, 30), "RED TEAM");
				GUI.Label(new Rect(nativeWidth/2+150, nativeHeight/2-100, 150, 30), "BLUE TEAM");

				GUI.DrawTexture(new Rect(nativeWidth/2-300, nativeHeight/2-20, 33, 30), sc.redFlag);
				GUI.DrawTexture(new Rect(nativeWidth/2-300, nativeHeight/2+30, 33, 30), sc.kills);
				GUI.DrawTexture(new Rect(nativeWidth/2-300, nativeHeight/2+80, 33, 30), sc.deaths);

				GUI.Label(new Rect(nativeWidth/2-250, nativeHeight/2-20, 150, 30), "Flags captured: " + sc.redFlagScore.ToString());
				GUI.Label(new Rect(nativeWidth/2-250, nativeHeight/2+30, 150, 30), "Kills : " + sc.redKills.ToString());
				GUI.Label(new Rect(nativeWidth/2-250, nativeHeight/2+80, 150, 30), "Deaths : " + sc.redDeaths.ToString());

				GUI.DrawTexture(new Rect(nativeWidth/2+100, nativeHeight/2-20, 33, 30), sc.blueFlag);
				GUI.DrawTexture(new Rect(nativeWidth/2+100, nativeHeight/2+30, 33, 30), sc.kills);
				GUI.DrawTexture(new Rect(nativeWidth/2+100, nativeHeight/2+80, 33, 30), sc.deaths);
				
				GUI.Label(new Rect(nativeWidth/2+150, nativeHeight/2-20, 150, 30), "Flags captured: " + sc.blueFlagScore.ToString());
				GUI.Label(new Rect(nativeWidth/2+150, nativeHeight/2+30, 150, 30), "Kills : " + sc.blueKills.ToString());
				GUI.Label(new Rect(nativeWidth/2+150, nativeHeight/2+80, 150, 30), "Deaths : " + sc.blueDeaths.ToString());

				// If the players hit the "Go back" button, they will be disconnected from the server and the game will restart
				if(GUI.Button(new Rect(nativeWidth/2-50, nativeHeight/2+180, 100, 60), "GO BACK"))
				{
					spawn.spawned = false;
					Network.Disconnect();
					Network.DestroyPlayerObjects(Network.player);
				}
			}
		}
	}

	// If the player has spawned, the countdown timer will start
	void countDown()
	{
		if(spawn.spawned)
		{
			if(seconds <= 0)
			{
				seconds = 59;
				if(minutes >= 1)
				{
					minutes--;
				}
				else
				{
					minutes = 0;
					seconds = 0;
				}
			}
			else
			{
				seconds = seconds - Time.deltaTime;
			}

			// Send an RPC to everyone so that they will all be synchronized

			networkView.RPC("UpdateTime", RPCMode.AllBuffered, minutes, seconds);

		}

	}

	// The gold of the player will be increased by 1 every second
	void GoldCount()
	{
		myGold = myGold + 1.0f * Time.deltaTime;
	}

	// Determines when the game ends and which team won. If both teams have captured the same amount of flags
	// then the winning team will be decided based on the kills they made. Play different audio clips, depending
	// on who is on the winning side
	void EndGame()
	{
		if(minutes == 0 && seconds == 0)
		{
			endGame = true;
			EndGameAudio(11);

			if(sc.redFlagScore > sc.blueFlagScore)
			{
				winningMessage = "Red Team wins!";
				networkView.RPC("WinningMessage", RPCMode.AllBuffered, winningMessage);
			}

			if(sc.redFlagScore < sc.blueFlagScore)
			{
				winningMessage = "Blue Team wins!";
				networkView.RPC("WinningMessage", RPCMode.AllBuffered, winningMessage);
			}

			if(sc.redFlagScore == sc.blueFlagScore)
			{
				if(sc.redKills > sc.blueKills)
				{
					winningMessage = "Red Team wins!";
					networkView.RPC("WinningMessage", RPCMode.AllBuffered, winningMessage);
				}
				if(sc.redKills < sc.blueKills)
				{
					winningMessage = "Blue Team wins!";
					networkView.RPC("WinningMessage", RPCMode.AllBuffered, winningMessage);
				}
			}
		}
	}

	// Play end game audio clip only once and mute background music
	void EndGameAudio(int audioID)
	{
		if(!endGameAudio)
		{
			soundEffects.PlayAudio(audioID);
			endGameAudio = true;
			backgroundAudio.source.mute = true;
		}
	}
	
	[RPC]
	void UpdateTime(float min, float sec)
	{
		minutes = min;
		seconds = sec;
	}

	[RPC]
	void WinningMessage(string mess)
	{
		winningMessage = mess;
	}

}
