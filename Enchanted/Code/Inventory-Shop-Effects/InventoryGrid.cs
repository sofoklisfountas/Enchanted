using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryGrid : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	public int temp;
	public int itemCounter = 0;
	
	public string bagName = "INVENTORY";

	// Position and size of inventory, buttons (inventory slots) and gold
	private Rect boxRect;
	private Rect[,] invButton = new Rect[3,3]; // A 3x3 array of rectangles 
	private Rect goldLabel;
	private Rect coinRect;

	public float slotSize = 50;
	
	public bool showBag = false;
	public bool itemSelecredRight = false;

	public bool fullBag = false;

	// Textures
	public Texture[] defaultTextureSlot = new Texture[9]; // An array of textures for each slot
	public Texture texInv;
	public Texture bagIcon;
	public Texture coinTex;

	// References to other scripts
	public ItemEffect itemEf;

	// Audio
	private SoundEffects soundEffects;

	public GUISkin myStyle;

	// Use this for initialization
	void Start ()
	{
		if(networkView.isMine)
		{
			boxRect = new Rect (nativeWidth - 565, nativeHeight - 300, 210, 240);

			goldLabel = new Rect(nativeWidth - 550, nativeHeight - 90, 100, 40);
			coinRect = new Rect(nativeWidth-475, nativeHeight-80, 15, 15);

			itemEf = gameObject.GetComponent<ItemEffect>();

			soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();

			texInv = null;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Open and close inventory with the "B" button of the keyboard
		if(Input.GetKeyUp(KeyCode.B))
		{
			soundEffects.PlayAudio(6);
			ToggleBag();
		}
	}

	void OnGUI()
	{
		// Resize GUI
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Custom GUI skin
		GUI.skin = myStyle;

		if(networkView.isMine)
		{
			// If the player clicks on the button, the inventory window will toggle
			if(GUI.Button(new Rect(nativeWidth - 450, nativeHeight - 50, 40, 40), bagIcon))
			{
				soundEffects.PlayAudio(6);
				ToggleBag();
			}
			
			if(showBag == true)
			{
				GUI.Box(boxRect, bagName);

				int width = 0;
				int height = 0;
				int k = 0;

				// Create inventory as a 3x3 array of buttons
				for(int i=0; i<3; i++)
				{
					for(int j=0; j<3; j++)
					{
						if(j==0 && i>0)
						{
							height += 65;
							width = 0;
						}
						width += 65;

						invButton[i,j] = new Rect(nativeWidth-615+width, nativeHeight-275+height, 50, 50);

						if(GUI.Button(new Rect(invButton[i,j]), defaultTextureSlot[k]))
						{
							// If the player clicks on a slot with the left mouse button the effect of the potion
							// will take place.
							if(Event.current.button == 0)
							{
								soundEffects.PlayAudio(7);

								itemCounter--;
								texInv = defaultTextureSlot[k];
								itemEf.Effect();
								defaultTextureSlot[k] = null;
								fullBag = false;
							}
							// If the player clicks on a slot with the right mouse button the item will be "ready" to sell
							else if(Event.current.button == 1)
							{
								soundEffects.PlayAudio(4);
								
								itemSelecredRight = true;
								texInv = defaultTextureSlot[k]; // The texture clicked will be stored in a temp variable
								temp = k; // Store the index of the array to see which button was pressed

							}
						}
						k++;
					}
				}
				GUI.DrawTexture(coinRect, coinTex);
				GUI.Label(goldLabel, "GOLD: " + CountdownTimer.myGold.ToString("f0"));
			}
		}
	}

	// Open and close inventory
	void ToggleBag()
	{
		showBag = !showBag;
	}

}
