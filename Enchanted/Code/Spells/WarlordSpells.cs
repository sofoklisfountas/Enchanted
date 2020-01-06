using UnityEngine;
using System.Collections;

public class WarlordSpells : MonoBehaviour {

	// References
	private Mana manaScript;
	private SpawnScript spawn;
	private AudioSource source;
	private Transform myTransform;

	// The spell effects
	public GameObject spell1Effect;

	//Boolean vars to check if Spell_1 Button and Spell_2 Button have been pressed
	private bool button1Pressed = false;
	private bool button2Pressed = false;
	
	private float radius = 3f;
	private Vector3 target; // The point the Raycast hits.
	public LayerMask myLayerMask; // Ignore all layers the Raycast hits except this one.
	
	private bool iAmOnTheRedTeam;
	private bool iAmOnTheBlueTeam;
	
	private float manaCost1 = 20f;
	private float manaCost2 = 40f;
	private string manaMess = "";
	
	private float cooldown1 = 5f;
	private float cooldown2 = 7f;

	// Animation clips
	public AnimationClip attackSpeedAnimation;
	public AnimationClip jumpAttackAnimation;
	
	//Audio
	public AudioClip spell1Ac;
	public AudioClip spell2Ac;

	// GUI size
	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			myTransform = transform;
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
			manaScript = gameObject.GetComponent<Mana>();

			if(spawn.amIOnTheRedTeam == true)
			{
				iAmOnTheRedTeam = true;
			}
			if(spawn.amIOnTheBlueTeam == true)
			{
				iAmOnTheBlueTeam = true;
			}
		}
		else
		{
			enabled = false;
		}
		source = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckforMana(); // When player's mana runs low, pop a message to inform him/her
		Spell1();
		Spell2();
	}
	
	void CheckforMana()
	{
		if((Input.GetButtonDown("Button1") && manaScript.myMana < manaCost1) || (Input.GetButtonDown("Button2") && manaScript.myMana < manaCost2))
		{
			StartCoroutine("waitForMana");
		}
	}
	
	void Spell1()
	{
		if(button1Pressed == false && manaScript.myMana >= manaCost1)
		{
			if(Input.GetButtonDown("Button1"))
			{
				button1Pressed = true;
				manaScript.ManaCost(manaCost1);
				StartCoroutine (delayBeforeNextSpell (1, cooldown1));

				networkView.RPC("HealSelf", RPCMode.All, myTransform.position, Quaternion.identity);
			}
		}
	}
	
	void Spell2()
	{
		if(button2Pressed == false && manaScript.myMana >= manaCost2)
		{
			if(Input.GetButtonDown("Button2"))
			{
				button2Pressed = true;
				manaScript.ManaCost(manaCost2);
				StartCoroutine (delayBeforeNextSpell (2, cooldown2));

				networkView.RPC("jumpAttack",RPCMode.All, transform.position, radius);
			}
		}
	}

	[RPC]
	void HealSelf(Vector3 position, Quaternion rotation)
	{
		source.PlayOneShot(spell1Ac);

		GameObject healParticle = (GameObject) Instantiate (spell1Effect, position, rotation);
		healParticle.gameObject.transform.parent = this.gameObject.transform;

		PlayerStats plStats = gameObject.GetComponent<PlayerStats>();
		HealthAndDamage HDScript = gameObject.GetComponentInChildren<HealthAndDamage>();

		HDScript.healingValue = plStats.spellStat;
		HDScript.applyHeal = true;
	}

	[RPC]
	void jumpAttack(Vector3 center, float radius)
	{
		source.PlayOneShot(spell2Ac);
		playSpellAnimation (jumpAttackAnimation);
		rotatePlayerOnAttack();
						
		Collider[] collider = Physics.OverlapSphere(center, radius);
		float duration = 5f;
		int i = 0;

		while(i < collider.Length)
		{
			if(iAmOnTheRedTeam && collider[i].transform.tag == "BlueTeamTrigger")
			{
				//Check if enemy is in a 60 degrees angle in front of attacker and then apply the damage and the rest
				if( Vector3.Angle(transform.forward, collider[i].transform.position - transform.position) < 60) 
				{
					PlayerStats plstats = spawn.redspawn.GetComponent<PlayerStats>();
					applyDamageBleed(collider, i, plstats, duration);
				}
			}

			if(iAmOnTheBlueTeam && collider[i].transform.tag == "RedTeamTrigger")
			{
				//Check if enemy is in a 60 degrees angle in front of attacker and then apply the damage and the rest
				if( Vector3.Angle(transform.forward, collider[i].transform.position - transform.position) < 60) 
				{
					PlayerStats plstats = spawn.bluespawn.GetComponent<PlayerStats>();
					applyDamageBleed(collider, i, plstats, duration);
				}
			}
			i++;
		}
	}
	
	
	
	void applyDamageBleed(Collider[] colliderList, int counter,PlayerStats playerStats,float bleedDuration)
	{
		HealthAndDamage HDScript = colliderList[counter].transform.GetComponent<HealthAndDamage>();

		HDScript.spellDamage = playerStats.spellStat*2;
		HDScript.iWasJustAttacked = true;
		HDScript.bleed = true;
		HDScript.bleedInstantiate = true;
		HDScript.bleedTime = bleedDuration;
	}
	
	void rotatePlayerOnAttack()
	{
		// Find the point the Raycast hits
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit, 100, myLayerMask))
		{
			target = hit.point;
		}
		
		// Rotate the player to that point only on y axis
		Quaternion newRotation = Quaternion.LookRotation(target - transform.position);
		newRotation.x = 0f;
		newRotation.z = 0f;
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 70);
	}

	void playSpellAnimation(AnimationClip spellAnimation)
	{
		animation[spellAnimation.name].layer = 1;
		animation[spellAnimation.name].wrapMode = WrapMode.Once;
		animation.CrossFade(spellAnimation.name);
	}
	
	// A Coroutine to apply cooldowns on spells
	IEnumerator delayBeforeNextSpell(int spellNumber, float cooldown)
	{
		if(spellNumber == 1)
		{
			yield return new WaitForSeconds (cooldown);
			button1Pressed = false;
		}
		if(spellNumber == 2)
		{
			yield return new WaitForSeconds (cooldown);
			button2Pressed = false;
		}

	}

	// Pop this message for 3 seconds
	IEnumerator waitForMana()
	{
		manaMess = "Not enough Mana";
		yield return new WaitForSeconds(3);
		manaMess = "";
	}
	
	void OnGUI()
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f));
		
		if(spawn.spawned)
		{
			GUI.Label(new Rect(nativeWidth/2 - 90 ,nativeHeight - 200, 200, 30), manaMess);
		}
	}
	
}