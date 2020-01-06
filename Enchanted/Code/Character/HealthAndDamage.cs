using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the Trigger GameObject on the player and
/// it manages the health of the player across the network and applies
/// damage to the player across the network.
/// </summary>

public class HealthAndDamage : MonoBehaviour {
	
	// References
	private FlagSystem fs;
	private SpawnScript spawn;
	private GameObject parentObject;
	
	public bool iWasJustAttacked;
	
	//These variables are used in figuring out what the player has been hit by and how much damage to apply.
	public bool hitByProjectile = false;
	public bool hitBySpell1 = false;
	public bool hitByMelee = false;
	public bool applyHeal = false;
	public bool bleed = false;
	public bool bleedInstantiate = false;
	
	public float bleedTime;
	public float attackDamage;
	public float spellDamage;
	public float healingValue;
	public float bleedRate;
	
	//This is used to prevent the player from getting hit while they are undergoing destruction.
	private bool destroyed = false;
	
	public static bool updateScore = false; // Seting this true when a playuer dies to update the score
	
	//These variables are used in managing the player's health.
	public float myHealth = 100;
	public float maxHealth = 100;
	public float healthRegenRate;
	public float previousHealth = 100;
	
	// Effects
	public GameObject hitEffect;
	public GameObject bleedParticle;
	public GameObject bleedEffect;	
	
	// Use this for initialization
	void Start () 
	{
		parentObject = transform.parent.gameObject;
		spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
		fs = GameObject.Find("SpawnManager").GetComponent<FlagSystem>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		healthRegenRate = maxHealth * 0.015f;
		
		//If the player is hit by an opposing team projectil, then that projectile will have set iWasJustAttacked to true.
		if(iWasJustAttacked == true)
		{
			//Check what the player was hit by and apply damage.
			if(hitByProjectile == true && destroyed == false)
			{
				myHealth = myHealth - attackDamage;	
				networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.Others, myHealth);
				
				//Instantiate hit effect
				networkView.RPC("InstantiateHitEffect", RPCMode.All, transform.position, Quaternion.identity);
				
				hitByProjectile = false;
			}
			if(hitBySpell1 == true && destroyed == false)
			{
				myHealth = myHealth - spellDamage;
				networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.Others, myHealth);
				
				hitBySpell1 = false;
				
			}
			
			if(hitByMelee == true && destroyed == false)
			{
				myHealth = myHealth - attackDamage;	
				networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.Others, myHealth);
				
				//Instantiate hit effect
				networkView.RPC("InstantiateHitEffect", RPCMode.All, transform.position, Quaternion.identity);
				
				hitByMelee = false;
			}
		}
		iWasJustAttacked = false;
		
		
		// Heal (Necromancer, Warlord)
		if(applyHeal)
		{
			myHealth = myHealth + healingValue;
			networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.Others, myHealth);
			applyHeal = false;
		}
		
		// Warlord bleed
		if(bleed)
		{
			bleedTime -= Time.deltaTime; 
			myHealth = myHealth - (spellDamage/5)* Time.deltaTime;
			
			if(bleedInstantiate)
			{
				networkView.RPC("BleedEffect", RPCMode.All, transform.position, Quaternion.identity);
			}
			StartCoroutine(bleedFalse(bleedTime));
			
			networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.All, myHealth);
		}
		
		
		//Each player is responsible for destroying themselves.
		if(myHealth <= 0 && networkView.isMine == true)
		{	
			//Access the SpawnScript and set the iAmDestroyed bool to true so that this player can respawn.
			GameObject spawnManager = GameObject.Find("SpawnManager");
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			spawnScript.iAmDestroyed = true;

			//Remove this player's RPCs. If we didn't do this a ghost of this player would remain in the game
		
			updateScore = true;
		
			networkView.RPC("DestroyPlayer", RPCMode.AllBuffered);
			
			// When a player dies, if he has picked up the flag, drop it at the position that player died so that someone else can pick it up
			if(spawn.amIOnTheRedTeam && fs.blueFlagPicked)
			{
				fs.redDrop = true;
				Network.Instantiate(fs.blueFlagObj, parentObject.transform.position, parentObject.transform.rotation, 0);
				
			}
			if(spawn.amIOnTheBlueTeam && fs.redFlagPicked)
			{
				fs.blueDrop = true;
				Network.Instantiate(fs.redFlagObj, parentObject.transform.position, parentObject.transform.rotation, 0);
			}
		}
		
		if(myHealth > 0 && networkView.isMine)
		{
			if(myHealth != previousHealth)
			{
				networkView.RPC("UpdateMyHealthRecordEverywhere", RPCMode.AllBuffered, myHealth);
			}
		}
		
		//Regen the player's health if it is below the max health. If bleed eefect is applied on the player, stop health regenaration
		if((myHealth < maxHealth) != bleed)
		{
			myHealth = myHealth + healthRegenRate * Time.deltaTime;
		}
		
		//If the player's health exceeds the max health while regenerating then set it back to the max health.
		if(myHealth > maxHealth)
		{
			myHealth = maxHealth;	
		}
	}
	
	public void healthPotion()
	{
		myHealth = myHealth + maxHealth * 0.30f;
		bleed = false;
		networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.All, myHealth);	
	}
	
	public void maxHealthPotion()
	{
		maxHealth = maxHealth + 50.0f;
		myHealth = maxHealth;
		networkView.RPC("UpdateMaxHealthEverywhere", RPCMode.All, maxHealth);
		networkView.RPC("UpdateMyCurrentHealthEverywhere", RPCMode.All, myHealth);	
	}
	
	[RPC]
	void UpdateMyCurrentHealthEverywhere (float health)
	{
		myHealth = health;	
	}
	
	[RPC]
	void UpdateMaxHealthEverywhere (float maxhealth)
	{
		maxHealth = maxhealth;	
	}
	
	[RPC]
	void UpdateMyHealthRecordEverywhere(float health)
	{
		previousHealth = health;
	}
		
	[RPC]
	public void ApplyDamage(int damage)
	{
		myHealth = myHealth - damage;
	}
	
	[RPC]
	void InstantiateHitEffect(Vector3 position, Quaternion rotation)
	{
		Instantiate(hitEffect, position, rotation);
	}
	
	[RPC]
	void BleedEffect(Vector3 position, Quaternion rotation)
	{
		bleedEffect = (GameObject)Instantiate (bleedParticle, position, rotation);
		bleedEffect.gameObject.transform.parent = this.gameObject.transform;
		bleedInstantiate = false;
	}

	[RPC]
	void DestroyPlayer()
	{
		Destroy(parentObject);
	}
	
	IEnumerator bleedFalse(float time)
	{
		yield return new WaitForSeconds(time);
		bleed = false;
		Network.Destroy (bleedEffect);
	}

}
