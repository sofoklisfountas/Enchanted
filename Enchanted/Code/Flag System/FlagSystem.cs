using UnityEngine;
using System.Collections;

public class FlagSystem : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	public Camera myCamera;
	
	public bool redFlagPicked, blueFlagPicked, redFlagLeave, blueFlagLeave, 
				redDrop, blueDrop, redFlagAvailable, blueFlagAvailable, showMessage = false;
		
	private Scoreboard sc;
	private SpawnScript sp;
	
	public GameObject redFlagObj;
	public GameObject blueFlagObj;

	private Rect windowRect;
	private Rect redLabel;
	private Rect blueLabel; 
	
	private string redMess;
	private string blueMess;
	
	private GUIStyle redStyle = new GUIStyle();
	private GUIStyle blueStyle = new GUIStyle();
	
	public GUISkin myGuiSkin;
	
	public SoundEffects soundEffects;
	
	// Use this for initialization
	void Start () {

		myCamera = Camera.main;

		// References to other scripts
		sc = gameObject.GetComponent<Scoreboard>();
		sp = gameObject.GetComponent<SpawnScript>();
		soundEffects = GameObject.Find ("AudioEffectsManager").GetComponent<SoundEffects>();

		redMess = "";
		blueMess = "";
		
		redStyle.normal.textColor = Color.red;
		blueStyle.normal.textColor = Color.cyan;
		
		windowRect = new Rect(0, nativeHeight - 250, 500, 50);
		redLabel = new Rect(50, nativeHeight-240, 500, 50);
		blueLabel = new Rect (50, nativeHeight - 220, 500, 50);
	}
	
	// Update is called once per frame
	void Update () {
		
		PickUpFlag();
		LeaveFlag();
		DropFlag();
		ReturnToBase();
	}

	// When the player clicks on the opponent's flag, it disappears and a message is shown, informing
	// everybody that the flag has been picked up.

	void PickUpFlag()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Input.GetMouseButtonDown(0))
		{
			if(Physics.Raycast(ray, out hit, 15))
			{
				if(hit.collider.tag == "RedFlag" && sp.amIOnTheBlueTeam)
				{
					blueMess = PlayerPrefs.GetString("playerName") + " picked up the Red Flag!";
					
					networkView.RPC("playAudio", RPCMode.All, 0);

					networkView.RPC("BlueFlag", RPCMode.All, blueMess, false);
					
					redFlagPicked = true;
					redFlagLeave = false;
					blueDrop = false;
					
					Network.Destroy(GameObject.FindGameObjectWithTag("RedFlag"));
				}
				
				if(hit.collider.tag == "BlueFlag" && sp.amIOnTheRedTeam)
				{
					redMess = PlayerPrefs.GetString("playerName") + " picked up the Blue Flag!";
					
					networkView.RPC("playAudio", RPCMode.All, 0);
					
					networkView.RPC("RedFlag", RPCMode.All, redMess, false);
					
					blueFlagPicked = true;
					blueFlagLeave = false;
					redDrop = false;
					
					Network.Destroy(GameObject.FindGameObjectWithTag("BlueFlag"));
				}
				
			}
		}
	}

	// Used when the player has successfully returned to the base carring the opponent's flag. The player has to 
	// click on his team's flag so that the team can score 1 point. Then, the flag returns back to its
	// default position (the opponent's base).
	
	void LeaveFlag()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Input.GetMouseButtonDown(0))
		{
			if(Physics.Raycast(ray, out hit, 15))
			{
				if(hit.collider.tag == "RedFlag" && sp.amIOnTheRedTeam && blueFlagPicked == true)
				{
					blueFlagLeave = true;
					blueFlagPicked = false;
					redDrop = false;
					
					Network.Instantiate(blueFlagObj, blueFlagObj.transform.position, blueFlagObj.transform.rotation, 0);
					
					redMess = "Red Team scored!";
					
					networkView.RPC("playAudio", RPCMode.All, 1);
					
					networkView.RPC("RedFlag", RPCMode.All, redMess, false);
					sc.redFlagScore = sc.redFlagScore + 1;
					networkView.RPC("UpdateFlagScoreRed", RPCMode.AllBuffered, sc.redFlagScore);
				}
				
				if(hit.collider.tag == "BlueFlag" && sp.amIOnTheBlueTeam && redFlagPicked == true)
				{
					redFlagLeave = true;
					redFlagPicked = false;
					blueDrop = false;
					
					Network.Instantiate(redFlagObj, redFlagObj.transform.position, redFlagObj.transform.rotation, 0);
					
					blueMess = "Blue Team scored!";
					
					networkView.RPC("playAudio", RPCMode.All, 1);
					
					networkView.RPC("BlueFlag", RPCMode.All, blueMess, false);
					sc.blueFlagScore = sc.blueFlagScore + 1;
					networkView.RPC("UpdateFlagScoreBlue", RPCMode.AllBuffered, sc.blueFlagScore);
				}
			}
		}
	}

	// When the player carries the opponent's flag but in the meantime he is killed, the flag is dropped at the player's last position.
	// Then the flag becomes available for both teams. 

	void DropFlag()
	{
		if(sp.amIOnTheRedTeam && blueFlagPicked && redDrop)
		{
			blueFlagPicked = false;
			blueFlagLeave = false;

			redMess = "Red Team dropped the flag!";
			
			networkView.RPC("playAudio", RPCMode.All, 2);
			
			networkView.RPC("RedFlag", RPCMode.All, redMess, true);
		}
		
		if(sp.amIOnTheBlueTeam && redFlagPicked && blueDrop)
		{
			redFlagPicked = false;
			redFlagLeave = false;
			
			blueMess = "Blue Team dropped the flag!";
			
			networkView.RPC("playAudio", RPCMode.All, 2);
			
			networkView.RPC("BlueFlag", RPCMode.All, blueMess, true);
		}
	}

	// If the flag has been dropped, the team that owns it can click on it and return the flag back to the base.
	void ReturnToBase()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Input.GetMouseButtonDown(0))
		{
			if(Physics.Raycast(ray, out hit, 15))
			{
				if(hit.collider.tag == "RedFlag" && sp.amIOnTheRedTeam && redFlagAvailable)
				{
					redFlagPicked = false;
					redDrop = false;
					redFlagLeave = false;
					
					redMess = "Red Flag returned to base!";
					
					networkView.RPC("playAudio", RPCMode.All, 3);
					
					networkView.RPC("RedFlag", RPCMode.All, redMess, false);
					Network.Destroy(GameObject.FindGameObjectWithTag("RedFlag"));
					Network.Instantiate(redFlagObj, redFlagObj.transform.position, redFlagObj.transform.rotation, 0);
				}
				
				if(hit.collider.tag == "BlueFlag" && sp.amIOnTheBlueTeam && blueFlagAvailable)
				{
					blueFlagPicked = false;
					blueDrop = false;
					blueFlagLeave = false;
					
					blueMess = "Blue Flag returned to base!";
					
					networkView.RPC("playAudio", RPCMode.All, 3);
					
					networkView.RPC("BlueFlag", RPCMode.All, blueMess, false);
					Network.Destroy(GameObject.FindGameObjectWithTag("BlueFlag"));
					Network.Instantiate(blueFlagObj, blueFlagObj.transform.position, blueFlagObj.transform.rotation, 0);
				}
			}
		}
	}
	
	void OnGUI()
	{	
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Set custom GUI skin
		GUI.skin = myGuiSkin;

		// Create a box and display all the messages
		if (sp.spawned == true)
		{
			GUI.Box(windowRect, "");
			
			GUI.Label(redLabel, redMess, redStyle);
			GUI.Label(blueLabel, blueMess, blueStyle);
		}
	}

	// Update each team's score
	
	[RPC]
	void UpdateFlagScoreRed(int flagScore)
	{
		sc.redFlagScore = flagScore;
	}
	
	[RPC]
	void UpdateFlagScoreBlue(int flagScore)
	{
		sc.blueFlagScore = flagScore;
	}

	// Send everyone a message about the status of the flag and indicate if it's available
	
	[RPC]
	void RedFlag(string mess, bool available)
	{
		redMess = mess;
		blueFlagAvailable = available;
	}
	
	[RPC]
	void BlueFlag(string mess, bool available)
	{
		blueMess = mess;
		redFlagAvailable = available;
	}

	// Play audio clips for each different action. Access the AudioEffectsManager where all the audio clips
	// are stored.
	
	[RPC]
	void playAudio(int audioClip)
	{
		soundEffects.PlayAudio (audioClip);
	}
	
}