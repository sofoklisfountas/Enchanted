using UnityEngine;
using System.Collections;

public class Chat : MonoBehaviour 
{
	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// Define the chat window
	private Rect windowRect;

	private string messBox = "", messageToSend = "";
	public string user;

	// References to other scripts
	private SpawnScript spawn;

	// Audio
	private SoundEffects soundEffects;
	public AudioClip enter;

	public GUISkin myskin;

	void Start()
	{
		spawn = gameObject.GetComponent<SpawnScript>();

		windowRect = new Rect(0, nativeHeight-200, 500, 200);

		soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();
	}

	private void OnGUI()
	{
		// Resize Gui
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Custom Gui Skin
		GUI.skin = myskin; 

		// The chat window will be shown if the player has spawned in the game
		if (spawn.spawned == true)
		{
			windowRect = GUI.Window(3, windowRect, windowFunc, "Chat");
		}

	}

	// When the player sends a message, it will be visible to everyone in the game inside the chat box and the variable
	// messageToSend will be available again so the player can type a new message
	private void windowFunc(int id)
	{
		GUILayout.Box(messBox, GUILayout.Height(135));

		GUILayout.BeginHorizontal();
		
		messageToSend = GUILayout.TextField(messageToSend);

		if(GUILayout.Button("Send", GUILayout.Width(100)))
		{
			soundEffects.PlayAudio(10);
			networkView.RPC("Message", RPCMode.AllBuffered, PlayerPrefs.GetString("playerName") + ": " + messageToSend + "\n", user);
			messageToSend = "";
		}

		GUILayout.EndHorizontal();
	}
	
	[RPC]
	void Message(string mess, string pName)
	{
		messBox += mess;
		pName = PlayerPrefs.GetString("playerName");
	}


}
