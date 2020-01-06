using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is attached to the MultiplayerManager and it 
/// is the foundation for our multiplayer system.
/// 
/// This script accesses the ScoreTable script to inform it of
/// the winning score criteria.
/// 
/// This script accesses the GameSettings script.
/// 
/// This script is accessed by the CursorControl script.
/// </summary>

public class MultiplayerScript : MonoBehaviour {
	
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;
	
	private string titleMessage = "";
	private string connectToIP = "127.0.0.1";
	private int connectionPort = 26500;	
	private bool useNAT = false;	
	private string ipAddress;	
	private string port;	
	private int numberOfPlayers = 10;	
	public string playerName;	
	public string serverName;	
	public string serverNameForClient;	
	public bool iWantToSetupAServer;	
	public bool iWantToConnectToAServer;	
	public bool iWantToSetupAPublicServer;	
	public bool iWantToSetupAPrivateServer;	
	public bool iWantToConnectToAPublicServer;	
	public bool iWantToConnectToAPrivateServer;
	
	public bool server;
	
	//These variables are used to define the main window.
	private Rect connectionWindowRect;
	private float connectionWindowWidth = 400;	
	private float connectionWindowHeight = 280;	
	private float btnHeightCntWindow = 60;	
	private float leftIndent;	
	private float topIndent;
	
	//These variables are used to define the server shutdown window.
	private Rect serverDisWindowRect;	
	private float serverDisWindowWidth = 300;	
	private float serverDisWindowHeight = 150;	
	private float serverDisWindowLeftIndent = 10;	
	private float serverDisWindowTopIndent = 10;
	
	//These variables are used to define the client disconnect window.
	private Rect clientDisWindowRect;	
	private float clientDisWindowWidth = 300;	
	private float clientDisWIndowHeight = 170;	
	public bool showDisconnectWindow;	
	private float btnHeightSmaller = 30;
	
	private GUIStyle plainStyle = new GUIStyle();	
	
	//Used in MasterServer implementation
	private string gameNameType = "Multiplayer 2014 Cath-Sof";	
	private Ping masterServerPing;	
	private Vector2 scrollPosition = Vector2.zero;	
	private GUIStyle boldStyleCentered = new GUIStyle();	
	private HostData[] hostData;	
	private string ipString;	
	private List<Ping> serverPingList = new List<Ping>();	
	private bool noPublicServers;	
	private float pbWidth = 70;	
	private float sbWidth = 250;	
	private float defCntWindowWidth;	
	private float defCntWindowHeight;	
	private float adjCntWindowWidth = 550;	
	private float adjCntWindowHeight = 400;
	
	private SpawnScript spawn;
	
	public GUISkin mySkin;
	
	private SoundEffects soundEffects;
	
	private Menu menu;
	private Camera myCamera;
	
	// Use this for initialization
	void Start () 
	{
		myCamera = Camera.main;
		menu = gameObject.GetComponent<Menu>();
		spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
		soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();
		
		//Load the last used serverName from registry. 
		//If the serverName is blank then use "Server" as a default name.
		serverName = PlayerPrefs.GetString("serverName");
		
		if(serverName == "")
		{
			serverName = "Server";	
		}
		
		//Load the last used playerName from registry. 
		//If the playerName is blank then use "Player" as a default name.
		playerName = PlayerPrefs.GetString("playerName");
		
		if(playerName == "")
		{
			playerName = "Player";	
		}
		
		
		//Set GUIStyles.
		plainStyle.alignment = TextAnchor.MiddleLeft;		
		plainStyle.normal.textColor = Color.white;		
		plainStyle.wordWrap = true;		
		plainStyle.fontStyle = FontStyle.Bold;		
		boldStyleCentered.alignment = TextAnchor.MiddleCenter;		
		boldStyleCentered.normal.textColor = Color.white;		
		boldStyleCentered.wordWrap = true;		
		boldStyleCentered.fontStyle = FontStyle.Bold;
		
		//Ping the master server to find out how long it takes to communicate to it. 
		//I have to RequestHostList otherwise the IP address of the default Unity Master Server won't be available.
		MasterServer.RequestHostList(gameNameType);		
		masterServerPing = new Ping(MasterServer.ipAddress);
		
		//Capture the default window size. The window size will be changed 
		//when looking at what public servers are available for connecting to.
		defCntWindowHeight = connectionWindowHeight;		
		defCntWindowWidth = connectionWindowWidth;		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			showDisconnectWindow = !showDisconnectWindow;	
		}
	}
	
	
	void ConnectWindow(int windowID)
	{
		//Leave a gap from the header.
		GUILayout.Space(15);
		
		//When the player launches the game they have the option to create a server or join a server. The variables
		//iWantToSetupAServer and iWantToConnectToAServer start as false so the player is presented with two buttons
		//"Setup my server" and "Connect to a server". 
		if(iWantToSetupAServer == false && 
		   iWantToConnectToAServer == false && 
		   iWantToSetupAPrivateServer == false && 
		   iWantToSetupAPublicServer == false &&
		   iWantToConnectToAPrivateServer == false &&
		   iWantToConnectToAPublicServer == false &&
		   menu.PlayPressed)
		{
			if(GUILayout.Button("Setup a server", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToSetupAServer = true;	
			}
			
			GUILayout.Space(10);
			
			if(GUILayout.Button("Connect to a server", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToConnectToAServer = true;
			}
			
			GUILayout.Space(10);
			
			if(GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				menu.PlayPressed = false;
			}
		}
		
		//If the player clicks on the Setup A Server button then they are given two server options. 
		//He can setup a server that's public and registered with the master server or 
		//he can setup a private game where port forwarding or LAN must be used for establishing a connection.
		if(iWantToSetupAServer)
		{
			if(GUILayout.Button("Setup a public server", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToSetupAPublicServer = true;
				iWantToSetupAServer = false;
			}
			
			if(GUILayout.Button("Setup a private server", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToSetupAPrivateServer = true;				
				iWantToSetupAServer = false;
			}
			
			if(GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToSetupAServer = false;					
				iWantToSetupAPrivateServer = false;				
				iWantToSetupAPublicServer = false;
			}
		}
		
		//If the player has chosen to setup a public server then initialize the server
		//and register it with the Master Server.
		if(iWantToSetupAPublicServer)
		{	
			//The user can type a name for their server into the textfield.
			GUILayout.Label("Enter a name for your server");			
			serverName = GUILayout.TextField(serverName);
			
			GUILayout.Space(5);
			
			
			if(GUILayout.Button("Launch and register public server", GUILayout.Height(btnHeightCntWindow)))
			{	
				soundEffects.PlayAudio(4);
				
				//Save the serverName using PlayerPrefs.
				PlayerPrefs.SetString("serverName", serverName);
				
				//If this computer doesn't have a public address then use NAT.
				Network.InitializeServer(numberOfPlayers,connectionPort,!Network.HavePublicAddress());					
				MasterServer.RegisterHost(gameNameType, serverName, "");
				
				//Sets the camera to render specific layer
				myCamera.cullingMask = (1 << LayerMask.NameToLayer("Multiplayer"));
				
				iWantToSetupAPublicServer = false;
				server = true;
			}
			
			GUILayout.Space(10);
			
			if(GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToSetupAPublicServer = false;				
				iWantToSetupAServer = true;
			}
		}
		
		if(iWantToSetupAPrivateServer)
		{
			//The user can type a name for their server into the textfield.
			GUILayout.Label("Enter a name for your server");			
			serverName = GUILayout.TextField(serverName);
			
			GUILayout.Space(5);
			
			//The user can type in the Port number for their server into textfield.
			//We defined a default value above in the variables as 26500.
			GUILayout.Label("Server Port");			
			connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));
			
			GUILayout.Space(10);
			
			if(GUILayout.Button("Start my own server", GUILayout.Height(btnHeightCntWindow/2)))
			{
				soundEffects.PlayAudio(4);
				
				//Create the server
				Network.InitializeServer(numberOfPlayers, connectionPort, useNAT);
				
				//Save the serverName using PlayerPrefs.
				PlayerPrefs.SetString("serverName", serverName);
				
				//Sets the camera to render specific layer
				myCamera.cullingMask = (1 << LayerMask.NameToLayer("Multiplayer"));
				
				iWantToSetupAPrivateServer = false;
				server = true;
			}
			
			if(GUILayout.Button("Go Back", GUILayout.Height(30)))
			{
				soundEffects.PlayAudio(4);
				iWantToSetupAPrivateServer = false;					
				iWantToSetupAServer = true;
			}
		}
		
		//If the player has chosen to connect to a server then give the him the option to connect to private server that will
		//port forwarding, or LAN to connect to, or the option to connect to a server from a list of servers.
		if(iWantToConnectToAServer)
		{
			if(GUILayout.Button("Connect to a public server", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToConnectToAPublicServer = true;				
				iWantToConnectToAServer = false;				
				MakeConnectionWindowBigger();				
				StartCoroutine(TalkToMasterServer());
			}			
			
			if(GUILayout.Button("Connect to a private server", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToConnectToAPrivateServer = true;				
				iWantToConnectToAServer = false;
			}
			
			if(GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
			{
				soundEffects.PlayAudio(4);
				iWantToConnectToAServer = false;					
				iWantToConnectToAPrivateServer = false;				
				iWantToConnectToAPublicServer = false;
			}
		}
		
		if(iWantToConnectToAPublicServer)
		{	
			//The user can type their player name into the textfield.
			GUILayout.Label("Enter your player name", plainStyle);			
			playerName = GUILayout.TextField(playerName);			
			GUILayout.Box("",GUILayout.Height(5));
			
			GUILayout.Space(15);
			
			//If hostData is empty and and no public servers were found then display that to the user.
			if(hostData.Length == 0 && noPublicServers == false)
			{
				GUILayout.Space(50);			
				GUILayout.Label("Searching for public servers...", boldStyleCentered);	
				
				GUILayout.Space(50);
			}
			
			//If hostData isn't empty then display the list of public servers it has.
			else if(hostData.Length != 0)
			{				
				//Header row
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Public servers", plainStyle, GUILayout.Height(btnHeightCntWindow/2), GUILayout.Width(sbWidth));			
				GUILayout.Label("Players", boldStyleCentered, GUILayout.Height(btnHeightCntWindow/2), GUILayout.Width(pbWidth));
				GUILayout.Label("Latency", boldStyleCentered, GUILayout.Height(btnHeightCntWindow/2), GUILayout.Width(pbWidth));
				
				GUILayout.EndHorizontal();
				
				scrollPosition = GUILayout.BeginScrollView(scrollPosition,false,false);
				
				for(int i = 0; i < hostData.Length; i++)
				{					
					GUILayout.BeginHorizontal();
					
					//Each of the available public servers are listed as buttons and the player
					//clicks on the relevant button to connect to a public server.
					if(GUILayout.Button(hostData[i].gameName, GUILayout.Height(btnHeightCntWindow/2), GUILayout.Width(sbWidth)))
					{
						soundEffects.PlayAudio(4);
						
						//Ensure that the player can't join a game with an empty name
						if(playerName == "")
						{
							playerName = "Player";	
						}
						
						//If the player has a name that isn't empty then attempt to join the server.
						if(playerName != "")
						{
							//Connect to the selected public server and save the player's name to player prefs.
							Network.Connect(hostData[i]);							
							PlayerPrefs.SetString("playerName", playerName);
						}		
					}
					
					//Dispaly the number of players currently in the server and the max number of players.
					GUILayout.Label((hostData[i].connectedPlayers -1) + "/" + (hostData[i].playerLimit -1), boldStyleCentered, 
					                GUILayout.Height(btnHeightCntWindow/2), GUILayout.Width(pbWidth));					
					
					//List the latency of each of the public servers. If the ping isn't complete or a latency couldn't be retreived
					//then output N/A meaning Not Available. I think we can't ping computers within our own network that don't have 
					//a public IP address. The ping should work on servers that are not part of our network.
					if(serverPingList[i].isDone)
					{				
						if(serverPingList[i].time <= 0)
						{
							GUILayout.Label("N/A", boldStyleCentered, GUILayout.Height(btnHeightCntWindow/2),GUILayout.Width(pbWidth));		
						}
						
						else
						{
							GUILayout.Label(serverPingList[i].time.ToString(), boldStyleCentered, GUILayout.Width(pbWidth));
						}
					}
					
					else
					{
						GUILayout.Label("N/A", boldStyleCentered, GUILayout.Height(btnHeightCntWindow/2),GUILayout.Width(pbWidth));	
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(10);
				}
				GUILayout.EndScrollView();
			}			
			else
			{
				GUILayout.Space(50);			
				GUILayout.Label("No public servers found.", boldStyleCentered);					
				GUILayout.Space(50);
			}
			
			GUILayout.Space(15);
			GUILayout.Box("",GUILayout.Height(5));
			
			//A refresh button that allows the user to refresh the list of public servers.
			if(GUILayout.Button("Refresh", GUILayout.Height(btnHeightCntWindow/2)))
			{
				soundEffects.PlayAudio(4);
				noPublicServers = false;			
				StartCoroutine(TalkToMasterServer());
			}
			
			GUILayout.Space(10);
			
			if(GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow/2)))
			{
				soundEffects.PlayAudio(4);
				MakeConnectionWindowDefaultSize();				
				iWantToConnectToAPublicServer = false;			
				iWantToConnectToAServer = true;			
				noPublicServers = false;
			}
		}
		
		if(iWantToConnectToAPrivateServer)
		{
			//The user can type their player name into the textfield.
			GUILayout.Label("Enter your player name");		
			playerName = GUILayout.TextField(playerName);
			GUILayout.Space(5);
			
			//The player can type the IP address for the server that they want to connect to into the textfield.
			GUILayout.Label("Type in Server IP");		
			connectToIP = GUILayout.TextField(connectToIP);
			
			GUILayout.Space(5);
			
			//The player can type in the Port number for the server they want to connect to into the textfield.
			GUILayout.Label("Server Port");	
			connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));
			
			GUILayout.Space(5);
			
			//The player clicks on this button to establish a connection.
			if(GUILayout.Button("Connect", GUILayout.Height(btnHeightSmaller)))
			{
				soundEffects.PlayAudio(4);
				
				//Ensure that the player can't join a game with an empty name
				if(playerName == "")
				{
					playerName = "Player";	
				}
				
				//If the player has a name that isn't empty then attempt to join the server.
				if(playerName != "")
				{
					//Connect to a server with the IP address contained in connectToIP 
					//and with the port number contained in connectionPort.
					Network.Connect(connectToIP, connectionPort);			
					PlayerPrefs.SetString("playerName", playerName);
				}		
			}
			
			GUILayout.Space(5);
			
			if(GUILayout.Button("Go Back", GUILayout.Height(btnHeightSmaller)))
			{
				soundEffects.PlayAudio(4);
				iWantToConnectToAPrivateServer = false;			
				iWantToConnectToAServer = true;
			}		
		}		
	}
	
	
	IEnumerator TalkToMasterServer()
	{	
		hostData = new HostData[0];
		
		//Clear the list of servers available so that only the most uptodate list will be put together.
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(gameNameType);
		
		//Wait a bit as it takes time for the host list to be retrieved from the Master Server.
		yield return new WaitForSeconds(masterServerPing.time/100 + 0.1f);
		
		//The list of public servers has been retrieved so put this into the hostData array.
		hostData = MasterServer.PollHostList();	
		
		//If no public servers are found then change the bool below to true.
		//This will affect what message is displayed when searching for servers.
		if (hostData.Length == 0)
		{
			noPublicServers = true;	
		}	
		
		//Clear the serverPingList and Trim of all the indexes
		//This allows the list to be resused and prevents GUI draw errors.
		serverPingList.Clear();	
		serverPingList.TrimExcess();
		
		//For each public server create an entry in the serverPingList
		//so that the ping of that server can be recorded and the latency then displayed.
		if (hostData.Length != 0)
		{			
			for(int i = 0; i < hostData.Length; i++)
			{					
				serverPingList.Add(new Ping(hostData[i].ip[0]));
			}
		}
	}
	
	void MakeConnectionWindowBigger ()
	{
		connectionWindowHeight = adjCntWindowHeight;
		connectionWindowWidth = adjCntWindowWidth;
	}
	
	void MakeConnectionWindowDefaultSize ()
	{
		connectionWindowHeight = defCntWindowHeight;
		connectionWindowWidth = defCntWindowWidth;
	}
	
	void ServerDisconnectWindow(int windowID)
	{
		GUILayout.Label("Server name: " + serverName);
		
		//Show the number of players connected.
		GUILayout.Label("Number of players connected: " + Network.connections.Length);
		
		//If there is at least one connection then show the average ping.
		if(Network.connections.Length >= 1)
		{
			GUILayout.Label("Ping: " + Network.GetAveragePing(Network.connections[0]));	
		}
		
		//Shutdown the server if the user clicks on the Shutdown server button.
		if(GUILayout.Button("Shutdown server"))
		{
			soundEffects.PlayAudio(4);
			Network.Disconnect();	
		}
	}
	
	void ClientDisconnectWindow(int windowID)
	{
		//Show the player the server they are connected to and the average ping of their connection.
		GUILayout.Label("Connected to server: " + serverName);
		GUILayout.Label("Ping; " + Network.GetAveragePing(Network.connections[0]));
		GUILayout.Space(7);
		
		//The player disconnects from the server when they press the Disconnect button.
		if(GUILayout.Button("Disconnect", GUILayout.Height(btnHeightSmaller)))
		{
			soundEffects.PlayAudio(4);
			Network.RemoveRPCs(Network.player);
			spawn.spawned = false;
			Network.Disconnect();	
		}
		
		GUILayout.Space(10);			
		
		//This button allows the player using a webplayer who has can gone 
		//fullscreen to be able to return to the game. Pressing escape in
		//fullscreen doesn't help as that just exits fullscreen.
		if(GUILayout.Button("Return To Game", GUILayout.Height(btnHeightSmaller)))
		{
			soundEffects.PlayAudio(4);
			showDisconnectWindow = false;	
		}
	}
	
	//Unity method (for client and server)
	void OnDisconnectedFromServer()
	{
		//If a player loses the connection or leaves the scene then the level is restarted on their computer.
		Application.LoadLevel(Application.loadedLevel);
	}
	
	//Unity method (for server)
	void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
		//When the player leaves the server delete them across the network
		//along with their RPCs so that other players no longer see them.
		Network.RemoveRPCs(networkPlayer);
		Network.DestroyPlayerObjects(networkPlayer);	
	}
	
	//Unity method (for server)
	void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		networkView.RPC("TellPlayerServerName", RPCMode.All, networkPlayer, serverName);	
	}
	
	//Unity method (for client)
	void OnConnectedToServer()
	{
		iWantToConnectToAPrivateServer = false;
		iWantToConnectToAPublicServer = false;
		
		MakeConnectionWindowDefaultSize();
	}	
	
	void OnGUI()
	{
		GUI.skin = mySkin;
		
		Vector2 resizeRatio = new Vector2(Screen.width / nativeWidth, Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 
		
		//If the player is disconnected then run the ConnectWindow function.
		Menu menu = gameObject.GetComponent<Menu>();
		
		if(Network.peerType == NetworkPeerType.Disconnected && menu.PlayPressed)
		{
			//Determine the position of the window based on the width and height of the screen.
			//The window will be placed in the middle of the screen.
			
			leftIndent = nativeWidth / 2 - connectionWindowWidth / 2;
			topIndent = nativeHeight / 2 - connectionWindowHeight / 2;
			
			connectionWindowRect = new Rect(leftIndent, topIndent, connectionWindowWidth, connectionWindowHeight);
			
			connectionWindowRect = GUILayout.Window(1, connectionWindowRect, ConnectWindow, titleMessage);
		}		
		
		//If the game is running as a server then run the ServerDisconnectWindow function.
		if(Network.peerType == NetworkPeerType.Server)
		{
			
			//Defining the Rect for the server's disconnect window.
			serverDisWindowRect = new Rect(serverDisWindowLeftIndent, serverDisWindowTopIndent,
			                               serverDisWindowWidth, serverDisWindowHeight);
			
			serverDisWindowRect = GUILayout.Window(2, serverDisWindowRect, ServerDisconnectWindow, "");
			
			GUILayout.EndHorizontal();			
			GUILayout.EndArea();
		}
		
		
		//If the connection type is a client (a player) then show a window 
		//that allows them to disconnect from the server.
		if(Network.peerType == NetworkPeerType.Client && showDisconnectWindow)
		{
			clientDisWindowRect = new Rect(nativeWidth / 2 - clientDisWindowWidth / 2,
			                               nativeHeight / 2 - clientDisWIndowHeight / 2,
			                               clientDisWindowWidth, clientDisWIndowHeight);
			
			clientDisWindowRect = GUILayout.Window(1, clientDisWindowRect, ClientDisconnectWindow, "");
		}
	}
	
	
	//Used to tell the MultiplayerScript in connected players the serverName. Otherwise
	//players connecting wouldn't be able to see the name of the server.
	[RPC]
	void TellPlayerServerName (string servername)
	{
		serverName = servername;	
	}
}
