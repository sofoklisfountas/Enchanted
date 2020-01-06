using UnityEngine;
using System.Collections;

public class NecromancerSpells : MonoBehaviour {

	// References
	private Mana manaScript;
	private SpawnScript spawn;
	private AudioSource source;
	private Transform myTransform;
	private PointAndClickMovement move;
	private FireProjectile fireProjectile; 	//Instance of the FireProjectile script so we can turn it on/off when needed(when spell button have pressed)

	public Projector projector; // Gameobject type of Projector for targeting

	// The spell effects
	public GameObject spell1Effect;
	public GameObject spell2Effect;

	// Boolean vars to check if  Button1, Button2 and leftMouseButton have been pressed
	private bool button1Pressed = false;
	private bool button2Pressed = false;
	private bool leftMouseButton = false;

	private Vector3 target; // The point the Raycast hits.
	public LayerMask myLayerMask; // Ignore all layers the Raycast hits except this one.

	private bool iAmOnTheRedTeam;
	private bool iAmOnTheBlueTeam;

	private float manaCost1 = 30f;
	private float manaCost2 = 5f;
	private string manaMess = "";
	
	private float cooldown1 = 5;
	private float cooldown2 = 7;

	// Animation clips
	public AnimationClip healDamageAnimation;
	public AnimationClip speedManaAnimation;
	
	//Audio clips
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
			manaScript = GetComponent<Mana>();
			move = GetComponent<PointAndClickMovement>();
			fireProjectile = GetComponent<FireProjectile>();
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();

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
				fireProjectile.enabled = false;
				Instantiate(projector, Input.mousePosition ,Quaternion.Euler(90,0,0));
			}
		}

		if(button1Pressed == true && leftMouseButton == false)
		{
			if(Input.GetButtonDown("leftMouseButton"))
			{
				leftMouseButton = true;
				manaScript.ManaCost(manaCost1);
				StartCoroutine (delayBeforeNextSpell (1, cooldown1));
				rotatePlayerOnAttack();
				
				//	Apply the effect of the spell 1 at the position of the mouse cursor
				RaycastHit hit;
				Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if (Physics.Raycast(ray,out hit, 100, myLayerMask))
				{
					target = hit.point;
					Network.Instantiate(spell1Effect, target, Quaternion.identity, 2);	
				} 
				
				if(iAmOnTheRedTeam == true)
				{
					networkView.RPC("Instantiate_Spell1", RPCMode.All, target, 7f);
				}
				
				if(iAmOnTheBlueTeam == true)
				{
					networkView.RPC("Instantiate_Spell1", RPCMode.All, target, 7f);
				}
				StartCoroutine(enableFireProjectile());
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

				float speedBuffRate = 1.4f;
				float manaRegenRate = 2f;
				int buffDuration = 5;
			
				move.SpeedBuff(speedBuffRate, buffDuration);
				manaScript.ManaRegenBuff(manaRegenRate, buffDuration);

				networkView.RPC("Instantiate_Spell2", RPCMode.All, myTransform.position, Quaternion.identity);

				if(button1Pressed)
				{
					Destroy(GameObject.Find("Projector(Clone)"));
					button1Pressed = false;
				}
			}
		}
	}
	
	//	HEAL ALLIES AND APPLY DAMAGE TO OPPONENTS - AOE
	[RPC]
	void Instantiate_Spell1(Vector3 center, float radius)
	{
		source.PlayOneShot(spell1Ac);
		playSpellAnimation(healDamageAnimation);
		
		//	Collect all colliders that collide within the sphere of spell 1
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		int i = 0;
		while (i < hitColliders.Length) 
		{
			//	Determines which are oponents and applies damage to them
			if(iAmOnTheRedTeam &&  hitColliders[i].transform.tag == "BlueTeamTrigger")
			{
				PlayerStats plstats = spawn.redspawn.GetComponent<PlayerStats>();
				HealthAndDamage HDScript = hitColliders[i].transform.GetComponent<HealthAndDamage>();

				HDScript.spellDamage = plstats.spellStat;
				HDScript.iWasJustAttacked = true;
				HDScript.hitBySpell1 = true;
			}
			if(iAmOnTheBlueTeam &&  hitColliders[i].transform.tag == "RedTeamTrigger")
			{
				PlayerStats plstats = spawn.bluespawn.GetComponent<PlayerStats>();
				HealthAndDamage HDScript = hitColliders[i].transform.GetComponent<HealthAndDamage>();

				HDScript.spellDamage = plstats.spellStat;
				HDScript.iWasJustAttacked = true;
				HDScript.hitBySpell1 = true;
			}
			
			
			//	Determines which are allies and applies healing to them
			if(iAmOnTheRedTeam && hitColliders[i].transform.tag == "RedTeamTrigger")
			{
				PlayerStats plstats = spawn.redspawn.GetComponent<PlayerStats>();
				HealthAndDamage HDScript = hitColliders[i].transform.GetComponent<HealthAndDamage>();

				HDScript.healingValue = plstats.spellStat;
				HDScript.applyHeal = true;
			}
			
			if(iAmOnTheBlueTeam && hitColliders[i].transform.tag == "BlueTeamTrigger")
			{
				PlayerStats plstats = spawn.bluespawn.GetComponent<PlayerStats>();
				HealthAndDamage HDScript = hitColliders[i].transform.GetComponent<HealthAndDamage>();

				HDScript.healingValue = plstats.spellStat;
				HDScript.applyHeal = true;
			}
			i++;
		}
	}
	
	[RPC]
	void Instantiate_Spell2(Vector3 position, Quaternion rotation)
	{
		source.PlayOneShot(spell2Ac);
		playSpellAnimation(speedManaAnimation);
		GameObject spell2 = (GameObject)Instantiate (spell2Effect, position, rotation);
		spell2.gameObject.transform.parent = this.gameObject.transform;
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
		animation [spellAnimation.name].speed = 5;
		animation[spellAnimation.name].wrapMode = WrapMode.Once;
		animation.CrossFade(spellAnimation.name);
	}

	// A Coroutine to apply cooldowns on spells
	IEnumerator delayBeforeNextSpell(int spellNumber, float cooldown)
	{
		if (spellNumber == 1) 
		{
			Debug.Log(cooldown);
			yield return new WaitForSeconds (cooldown);
			button1Pressed = false;
			leftMouseButton = false;
		}
		if (spellNumber == 2)
		{
			Debug.Log(cooldown);
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

	// Enables the FireProjectile script after the player casts a spell.
	IEnumerator enableFireProjectile()
	{
		yield return new WaitForSeconds(0.5f);
		fireProjectile.enabled = true;
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

