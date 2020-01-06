using UnityEngine;
using System.Collections;

public class ItemEffect : MonoBehaviour {

	// References to other scripts
	private ShopKeeper shop;
	private InventoryGrid inv;
	private HealthAndDamage HDScript;
	private Mana manaScript;
	private PlayerStats playerStats;
	private PointAndClickMovement move;
	

	// Use this for initialization
	void Start () {

		if(networkView.isMine)
		{
			shop = gameObject.GetComponent<ShopKeeper>();
			inv = gameObject.GetComponent<InventoryGrid>();
			Transform triggerTransform = transform.FindChild("Trigger");
			HDScript = triggerTransform.GetComponent<HealthAndDamage>();
			manaScript = gameObject.GetComponent<Mana>();
			playerStats = gameObject.GetComponent<PlayerStats>();
			move = gameObject.GetComponent<PointAndClickMovement>();
		}
		else
		{
			enabled = false;
		}



	}
	
	// Update is called once per frame
	void Update () {

	}

	// Access other scripts' methods for the potion effects
	public void Effect()
	{
		if(inv.texInv == shop.itemTex[0])
		{
			// 1. HEALTH POTION
			HDScript.healthPotion();
		}
		if(inv.texInv == shop.itemTex[1])
		{
			// 2. MANA POTION
			manaScript.manaPotion();
		}
		if(inv.texInv == shop.itemTex[2])
		{	
			// 3. SPEED POTION
			move.speedPotion();
		}
		if(inv.texInv == shop.itemTex[3])
		{
			// 4. ATTACK DAMAGE POTION
			playerStats.AttackDamagePotion();
		}
		if(inv.texInv == shop.itemTex[4])
		{
			// 5. MAX HEALTH POTION
			HDScript.maxHealthPotion();
		}
		if(inv.texInv == shop.itemTex[5])
		{
			// 6. SPELL DAMAGE POTION
			playerStats.SpellDamagePotion();
			//potion6 = true;
		}
				
	}



}
