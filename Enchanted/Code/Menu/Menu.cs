using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	// Custom GUI skins
	public GUISkin myGuiSkin;
	public GUISkin myGuiSkin2;

	// Texture of the game logo
	public Texture2D LOGO;

	public bool PlayPressed = false;
	public bool OptionsPressed = false;
	public bool display = false;
	public bool sound = false;
	public bool drop1 = false;
	public bool drop2 = false;
	private bool showButton = true;

	private Rect mainWindowRect; // Define the main window
	
	private Rect optionsWindowRect; // Define the options window
	private Rect optionRectButton; // Define the options button
	private Rect optionsHover;

	// Display Settings variables

	private Rect buttonGraphicsRect; // Graphics Options button

	// Define the labels used in graphics quality and resolution settings
	private Rect graphicsRect;
	private Rect screenRect;
	
	private Rect qualityButton; // Define the graphics quality button
	private Rect qualityRect; // A button that stores all available qualities
	private Rect resolutionButton; // Define the resolution button
	private Rect[,] resolutionRect = new Rect[8,3]; // An array of rectangles that store all the available resolutions

	// Volume Settings
	
	private Rect buttonVolumeRect; // Volume Options button
	private Rect effectsRect; // Define the label used in SFX volume
	private Rect backgroundRect; // Define the label used in background music volume
	private Rect masterRect; // Define the label used in master volume 
	private Rect spellsRect; // Define the label used in character spells volume

	// Define volume slidebars
	private Rect slidebarRect1;
	private Rect slidebarRect2;
	private Rect slidebarRect3;
	private Rect slidebarRect4;

	// Options button texture
	public Texture optionsButton;

	private string selectedOption = "";
	private string selectedOption2 = "";

	// References to other scripts
	private SpawnScript spawn;
	private SoundEffects soundEffects;

	// Default slider values for volume options
	private float sliderValue = 0.5f;
	private float sliderValue2 = 0.5f;
	private float sliderValue3 = 0.5f;
	private float sliderValue4 = 0.5f;

	// These GameObjects are assigned in the inspector
	public GameObject audioManager;
	public GameObject audioEffectsManager;


	// Use this for initialization
	void Start () {
	
		mainWindowRect = new Rect(nativeWidth/2 - 200, nativeHeight/2 - 140 , 400, 280); 
		optionsWindowRect = new Rect(nativeWidth/2 + 400, nativeHeight/2-200, 400, 520);
		buttonGraphicsRect = new Rect(nativeWidth/2+500, nativeHeight/2-150, 200, 60);
		buttonVolumeRect = new Rect(nativeWidth/2+500, nativeHeight/2-70, 200, 60);
		graphicsRect = new Rect(nativeWidth/2+450, nativeHeight/2, 400, 30);
		screenRect = new Rect(nativeWidth/2+450, nativeHeight/2+50, 400, 30);

		masterRect = new Rect (nativeWidth / 2 + 430, nativeHeight / 2 + 30, 400, 30);
		slidebarRect1 = new Rect (nativeWidth / 2 + 430, nativeHeight / 2 + 60, 340, 30);
		effectsRect = new Rect (nativeWidth / 2 + 430, nativeHeight / 2 + 90, 400, 30);
		slidebarRect2 = new Rect (nativeWidth / 2 + 430, nativeHeight / 2 + 120, 340, 30);
		backgroundRect = new Rect (nativeWidth / 2 + 430, nativeHeight / 2 + 150, 400, 30);
		slidebarRect3 = new Rect (nativeWidth / 2 + 430, nativeHeight / 2 + 180, 340, 30);

		optionRectButton = new Rect(nativeWidth - 100, nativeHeight - 50, 40, 40);
		optionsHover = new Rect (nativeWidth - 155, nativeHeight - 80, 100, 40);

		spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
		soundEffects = audioEffectsManager.GetComponent<SoundEffects>();
	}
	
	// Update is called once per frame
	void Update () {

		ToggleOptions ();
	}

	// The first window that appears when the game starts
	void PlayWindow(int WindowID)
	{
		GUILayout.BeginVertical ();
		GUILayout.Space(40);

		if(GUILayout.Button("PLAY", GUILayout.Height(60)))
		{
			PlayPressed = true;
			soundEffects.PlayAudio(4);
		}

		GUILayout.Space(20);

		if(GUILayout.Button("QUIT", GUILayout.Height(60)))
		{
			soundEffects.PlayAudio(4);
			Application.Quit();
		}
		GUILayout.EndVertical ();
	}
	
	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f));

		// Custom GUI skin
		if(!spawn.spawned)
		{
			GUI.skin = myGuiSkin;
		}
		else
		{
			GUI.skin = myGuiSkin2;
		}
		// Drawn the logo of the game
		GUI.DrawTexture(new Rect(nativeWidth/2-709, -250, 1418, 851), LOGO);

		// Display the main window
		if(!PlayPressed)
		{
			mainWindowRect = GUILayout.Window(0, mainWindowRect, PlayWindow, "");
		}

		// Open and close Options Window
		if(GUI.Button(optionRectButton, optionsButton))
		{
			OptionsPressed = !OptionsPressed;
			soundEffects.PlayAudio(5);
		}

		// Hover over Options button
		if(optionRectButton.Contains(Event.current.mousePosition))
		{
			GUI.Label(optionsHover, "Options");
		}

		// Draw Options Window and display all settings
		if(OptionsPressed)
		{
			GUI.Box (optionsWindowRect, "SETTINGS");

			DisplayOptions();
			AudioOptions();
		}

		// When the player spawns in the game, the options window, buttons and labels will have different position
		InGameOptions ();
	}

	void DisplayOptions()
	{
		if(GUI.Button(buttonGraphicsRect, "Display Options"))
		{
			display = true;
			sound = false;
			soundEffects.PlayAudio(4);
		}
		
		if(display)
		{
			GUI.Label(graphicsRect, "Graphics Quality");
			qualityButton = new Rect(nativeWidth/2+600, nativeHeight/2, 100 ,30);

			// The qualityButton will have different position when the player spawns in the game
			if(spawn.spawned)
			{
				qualityButton = new Rect(nativeWidth-200, nativeHeight/2-200, 100, 30);
			}
			
			if(GUI.Button(qualityButton, selectedOption))
			{
				
				// If the qualityButton is pressed, the resolutionButton will disappear
				showButton = false;

				drop1 = !drop1;
				soundEffects.PlayAudio(4);


				if(!drop1)
				{
					showButton = true;
				}
			}
			
			// A list of Unity's default quality settings will appear as a drop down list
			if(drop1)
			{
				for(int i=0; i<QualitySettings.names.Length; i++)
				{
					qualityRect = new Rect(nativeWidth/2+600, (nativeHeight/2+30)+i*30, 100, 30);
					
					if(spawn.spawned)
					{
						qualityRect = new Rect(nativeWidth-200, (nativeHeight/2-170)+i*30, 100, 30);
					}

					// When the player selects a quality option, the drop down list will close and the selected
					// option will appear on the button. The quality changes will take place.
					if(GUI.Button(qualityRect, QualitySettings.names[i]))
					{
						selectedOption = QualitySettings.names[i];
						QualitySettings.SetQualityLevel(i, true);
						drop1 = false;

						soundEffects.PlayAudio(4);
					}
				}
			}

			GUI.Label(screenRect, "Screen Resolution");

			// If the player has not pressed the qualityButton, the resolutionButton will be shown
			if(showButton)
			{
				resolutionButton = new Rect(nativeWidth/2+600, nativeHeight/2+50, 100, 30);

				// The resolutionButton will have different position when the player spawns in the game
				if(spawn.spawned)
				{
					resolutionButton = new Rect(nativeWidth-200, nativeHeight/2-150, 100, 30);
				}
				// When the player selects a resolution, the selected option will appear on the button.
				if(GUI.Button (resolutionButton, selectedOption2))
				{
					soundEffects.PlayAudio(4);
					drop2 = !drop2;
				}
				// A drop down list will appear when the player clicks on the resolutionButton, displaying all the available resolutions
				if(drop2)
				{
					int k =0;
					int width = 0;
					int height = 0;
					
					for(int i=0; i<Screen.resolutions.Length; i++)
					{
						for(int j=0; j<3; j++)
						{
							if(j==0 && i>0)
							{
								height +=30;
								width =0;
							}
							
							resolutionRect[i,j] = new Rect((nativeWidth/2+450)+width, (nativeHeight/2+80)+height, 100, 30);
							
							if(spawn.spawned)
							{
								resolutionRect[i,j] = new Rect((nativeWidth-350)+width, (nativeHeight/2-120)+height, 100, 30);
							}
							
							width += 100;

							// If the player clicks on a specific resolution, the changes will take place and the drop down list will close. 
							// The selected option will be stored in the selectedOption2 variable, so that it will be displayed on the resolutionButton.
							if(GUI.Button(resolutionRect[i,j], Screen.resolutions[k].width.ToString()+"x"+Screen.resolutions[k].height.ToString()))
							{
								Screen.SetResolution(Screen.resolutions[k].width, Screen.resolutions[k].height, true);
								selectedOption2 = Screen.resolutions[k].width.ToString()+"x"+Screen.resolutions[k].height.ToString();
								drop2 = false;

								soundEffects.PlayAudio(4);
							}
							k++;
							if(k==Screen.resolutions.Length)
							{
								break;
							}
						}
					}
				}
			}
		}
	}

	void AudioOptions()
	{
		if(GUI.Button(buttonVolumeRect, "Volume Options"))
		{
			sound = true;
			display = false;

			soundEffects.PlayAudio(4);
		}
		
		if(sound)
		{
			// The master volume is referred to all audio clips in the game. The AudioListener is responsible for 
			// what the player can hear.
			GUI.Label(masterRect, "Master Volume");
			sliderValue = GUI.HorizontalSlider(slidebarRect1, sliderValue, 0.0f, 1.0f);
			
			AudioListener.volume = sliderValue;

			// The SFX volume is controlled accessing the AudioEffectsManager Gameobject. All SFX audio clips
			// are stored in this Gameobject. 
			GUI.Label(effectsRect, "UI SFX Volume");
			sliderValue2 = GUI.HorizontalSlider(slidebarRect2, sliderValue2, 0.0f, 1.0f);
			
			audioEffectsManager.audio.volume = sliderValue2;

			// The background audio clips (both menu and game background music) are stored in the AudioManager Gameobject.
			GUI.Label(backgroundRect, "Background Music Volume");
			sliderValue3 = GUI.HorizontalSlider(slidebarRect3, sliderValue3, 0.0f, 1.0f);
			
			audioManager.audio.volume = sliderValue3;

			// The Spells audio clips are stored in each player seperately. The players access themselves, based on
			// the team they chose and they access their AudioSource volume.

			if(spawn.spawned)
			{
				GUI.Label(spellsRect, "Spells Volume");
				sliderValue4 = GUI.HorizontalSlider(slidebarRect4, sliderValue4, 0.0f, 1.0f);
				
				if(spawn.amIOnTheRedTeam)
				{
					spawn.redspawn.GetComponent<AudioSource>().volume = sliderValue4;
				}
				if(spawn.amIOnTheBlueTeam)
				{
					spawn.bluespawn.GetComponent<AudioSource>().volume = sliderValue4;
				}
			}

		}
	}

	void InGameOptions()
	{
		if(spawn.spawned)
		{

			optionRectButton = new Rect(nativeWidth - 400, nativeHeight - 50, 40, 40);
			optionsHover = new Rect(nativeWidth - 400, nativeHeight - 80, 100, 40);
			optionsWindowRect = new Rect(nativeWidth-400, nativeHeight/2-400, 400, 520);
			buttonGraphicsRect = new Rect(nativeWidth-300, nativeHeight/2-350, 200, 60);
			buttonVolumeRect = new Rect(nativeWidth-300, nativeHeight/2-270, 200, 60);
			graphicsRect = new Rect(nativeWidth-350, nativeHeight/2-200, 400, 30);
			screenRect = new Rect(nativeWidth-350, nativeHeight/2-150, 400, 30);
			
			masterRect = new Rect (nativeWidth-370, nativeHeight / 2-170, 400, 30);
			slidebarRect1 = new Rect (nativeWidth-370, nativeHeight / 2-140, 340, 30);
			effectsRect = new Rect (nativeWidth-370, nativeHeight / 2-110, 400, 30);
			slidebarRect2 = new Rect (nativeWidth-370, nativeHeight / 2 -80, 340, 30);
			backgroundRect = new Rect (nativeWidth-370, nativeHeight / 2 -50, 400, 30);
			slidebarRect3 = new Rect (nativeWidth-370, nativeHeight / 2 -20, 340, 30);
			spellsRect = new Rect (nativeWidth-370, nativeHeight / 2 +10, 340, 30);
			slidebarRect4 = new Rect (nativeWidth-370, nativeHeight / 2+40, 340, 30);
			
			LOGO = null;
			

		}

	}

	void ToggleOptions()
	{
		if(Input.GetKeyUp(KeyCode.O))
		{
			OptionsPressed = !OptionsPressed;
			soundEffects.PlayAudio(5);
		}
	}

}
