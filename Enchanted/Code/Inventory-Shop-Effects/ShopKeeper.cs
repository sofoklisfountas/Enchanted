using UnityEngine;
using System.Collections;

public class ShopKeeper : MonoBehaviour {

	// Used in resizing all the GUI
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	private RaycastHit hit;
	
	private Rect shopButton; 
	private Rect[,] itemRect = new Rect[2,3]; // A 2x3 array of rectangles
	private Rect hoverBox;
	private Rect hoverLabel;

	private string[] typeOfPotion;
	private int[] itemEffectValue;

	// Textures
	public Texture[] itemTex = new Texture[6]; 
	public Texture tex; // A temp variable that stores the texture of the selected item
	public Texture exit;

	public bool shopOpen = false;
	public bool itemSelected = false;
	public bool buyPressed = false;
	public bool sellPressed = false;
	
	public float[] itemBuyValue;
	public float[] itemSellValue;

	// Temp variables that store buy and sell values of the selected item
	public float buyValue;
	public float sellValue;

	private string goldMess = "";

	// References to other scripts
	public InventoryGrid inv;
	private SpawnScript spawn;

	// Audio
	private SoundEffects soundEffects;

	// Custom GUI skin
	public GUISkin mySkin;

	// Use this for initialization
	void Start () {
	
		if(networkView.isMine)
		{
			shopButton = new Rect(nativeWidth - 560, nativeHeight - 50, 100, 40);			
			hoverBox = new Rect(nativeWidth/2 - 280, nativeHeight/2 - 250, 200, 200);
			hoverLabel = new Rect (nativeWidth / 2 - 275, nativeHeight / 2 - 230, 190, 190);

			tex = null;

			inv = gameObject.GetComponent<InventoryGrid>();
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();

			soundEffects = GameObject.Find("AudioEffectsManager").GetComponent<SoundEffects>();

			itemBuyValue = new float[6]{40f, 50f, 30f, 25f, 60f, 20f};
			itemSellValue = new float[6]{20f, 25f, 15f, 10f, 30f, 10f};

			typeOfPotion = new string[6]{"Health", "Mana", "Speed", "Attack Damage", "Max Health", "Spell Damage"};
			itemEffectValue = new int[6]{30, 40, 100, 20, 50, 20};
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {

		// Open and close shop window using the "S" button of the keyboard
		if(Input.GetKeyUp(KeyCode.S))
		{
			soundEffects.PlayAudio(8);
			shopOpen = !shopOpen;
		}
		BuyItem();
	}

	// BUY item
	public void BuyItem()
	{
		int i = 0;
		while(i < inv.defaultTextureSlot.Length)
		{
			if(buyPressed && !inv.fullBag)
			{
				if(inv.defaultTextureSlot[i] == null)
				{
					inv.defaultTextureSlot[i] = tex;
					inv.itemCounter++;
					itemSelected = false;
					buyPressed = false;
					inv.fullBag = false;

					// Check if inventory is full. The the player will not be able to buy
					if(inv.itemCounter == inv.defaultTextureSlot.Length)
					{
						inv.fullBag = true;
					}
					break;
				}
			}
			i++;
		}
	}
	
	// SELL item
	public void SellItem(float value, int position)
	{
		int i = 0;
		while(i < inv.defaultTextureSlot.Length)
		{
			if(inv.itemSelecredRight && sellPressed)
			{
				inv.defaultTextureSlot[position] = null;
				CountdownTimer.myGold = CountdownTimer.myGold + value;
				sellValue = 0;
				inv.itemSelecredRight = false;
				sellPressed = false;
				break;
			}
			i++;
		}		
	}

	public float getItemSellValue(Texture tempTex)
	{
		for(int i=0; i<itemTex.Length; i++)
		{
			if(itemTex[i] == tempTex)
			{
				sellValue = itemSellValue[i];
			}
		}
		return sellValue;
	}

	
	void OnGUI()
	{
		// GUI resize
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 

		// Custom GUI skin
		GUI.skin = mySkin;

		// The shop button will appear when the player spawns in the game
		if(spawn.spawned)
		{
			if(GUI.Button(shopButton, "SHOP"))
			{
				shopOpen = true;
				soundEffects.PlayAudio(8);
			}
		}
		// Drawn the shop window and buttons
		if(shopOpen)
		{
			GUI.Box(new Rect(nativeWidth/2 - 600, nativeHeight/2 - 250, 300, 300), "SHOP");

			int counter = 0;
			int width = 0;
			int height = 0;

			for(int i=0; i<2; i++)
			{
				for(int j=0; j<3; j++ )
				{
					if(j==0 && i>0)
					{
						height += 90;
						width = 0;
					}
					width += 90;

					itemRect[i,j] = new Rect(nativeWidth/2-665+width, nativeHeight/2-220+height, 70, 70);

					// If the player clicks on a button, the item's texture will be stored in the tex variable, so that
					// it will be possible to appear in the inventory if the player buys it.
					if(GUI.Button(new Rect(itemRect[i,j]), itemTex[counter]))
					{
						tex = itemTex[counter];
						itemSelected = true;
						
						buyValue = itemBuyValue[counter]; // The buy value of the selected item
						
						soundEffects.PlayAudio(4);
					}
					// Hovering over shop buttons will display a window with the description of the item.
					if(itemRect[i,j].Contains(Event.current.mousePosition))
					{
						GUI.Box(hoverBox, typeOfPotion[counter]+" Potion");
						GUI.Label(hoverLabel, "\nIncrease "+typeOfPotion[counter]+" by "+itemEffectValue[counter]+"\n\n BUY: "+itemBuyValue[counter]+" GOLD \n\n SELL: "+itemSellValue[counter]+" GOLD");
					}
					counter++;

					// The player can buy the item only if they have selected one and the inventory is not full. The player's gold must be
					// enough, too.
					if(GUI.Button(new Rect(nativeWidth/2 - 588, nativeHeight/2 - 50, 132, 60), "BUY") && itemSelected && !inv.fullBag)
					{
						if(CountdownTimer.myGold >= buyValue)
						{
							buyPressed = true;
							
							soundEffects.PlayAudio(9);
							
							CountdownTimer.myGold = CountdownTimer.myGold - buyValue;
						}	
						else
						{
							StartCoroutine("waitForGold");
						}
					}

					// The player can sell if they right click on the item in the inventory
					if(GUI.Button(new Rect(nativeWidth/2 - 444, nativeHeight/2 - 50, 132, 60), "SELL") && inv.itemSelecredRight)
					{
						sellPressed = true;
						inv.fullBag = false;
						inv.itemCounter--;
						soundEffects.PlayAudio(9);

						// Find the sell value of the item, using the texture the player clicked
						SellItem (getItemSellValue(inv.texInv), inv.temp);
					}
				}
			}

			// Exit Button
			if(GUI.Button(new Rect(nativeWidth/2 - 600, nativeHeight/2 - 250, 30, 30), exit))
			{
				shopOpen = false;
				soundEffects.PlayAudio(4);
			}
			GUI.Label(new Rect(nativeWidth/2-90, nativeHeight-200, 200, 30), goldMess);
		}
	}
	
	IEnumerator waitForGold()
	{
		goldMess = "Not enough gold";
		yield return new WaitForSeconds(3);
		goldMess = "";
	}
}
