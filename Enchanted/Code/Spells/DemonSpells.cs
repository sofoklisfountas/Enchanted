using UnityEngine;
using System.Collections;

public class DemonSpells : MonoBehaviour {
	
	// References
	private Mana manaScript;
	private SpawnScript spawn;
	private AudioSource source;
	private Transform myTransform;
	private Transform firePointTransform;
	private FireProjectile fireProjectile;	//Instance of the FireProjectile script so we can turn it on/off when needed(when spell button have pressed)

	public Projector projector; // Gameobject type of Projector for targeting
	public GameObject spell2Target; // Gameobject type of ParticleSystem for targeting
	private Vector3 particlePosition; // The position of spell2 Target

	// The spell effects
	public GameObject spell1Effect;
	public GameObject spell2Effect;
	
	// Boolean vars to check if  Button1, Button2 and leftMouseButton have been pressed
	private bool button1Pressed = false;
	private bool button2Pressed = false;
	private bool leftMouseButton1 = false;
	private bool leftMouseButton2 = false;
	
	private Vector3 target; // The point the Raycast hits.
	public LayerMask myLayerMask; // Ignore all layers the Raycast hits except this one.
	private Vector3 launchPosition = new Vector3();	// The position at which the projectile should be instantiated.
	
	private bool iAmOnTheRedTeam;
	private bool iAmOnTheBlueTeam;

	private float manaCost1 = 40;
	private float manaCost2 = 25;
	private string manaMess = "";

	private float cooldown1 = 10;
	private float cooldown2 = 4;

	//Animation clips
	public AnimationClip spellAnimation;

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
			firePointTransform = myTransform.FindChild("FirePoint");
			manaScript = gameObject.GetComponent<Mana>();
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
		if(button1Pressed == false && leftMouseButton1 == false && manaScript.myMana >= manaCost1)
		{
			if(Input.GetButtonDown("Button1"))
			{
				button1Pressed = true;
				fireProjectile.enabled = false; // Disable FireProjectile script so the player cant attack and cast spell simultaneously
				Instantiate(projector, Input.mousePosition ,Quaternion.Euler(90,0,0));

				if(button2Pressed == true)
				{
					Destroy(GameObject.Find("Spell2Target(Clone)"));
					button2Pressed = false;
				}
			}
		}
		
		//if the conditions are met instantiate the spell 
		if(button1Pressed == true && leftMouseButton1 == false)
		{
			if(Input.GetButtonDown("leftMouseButton"))
			{	
				leftMouseButton1 = true;
				manaScript.ManaCost(manaCost1);
				rotatePlayerOnAttack();
				StartCoroutine (delayBeforeNextSpell (1, cooldown1));


				//Apply the effect of the AoE Spell at the position of the mouse cursor
				RaycastHit hit;
				Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if (Physics.Raycast(ray,out hit, 100, myLayerMask))
				{
					target = hit.point;
					Network.Instantiate(spell1Effect, target, Quaternion.identity, 2);
				} 
				

				if(iAmOnTheRedTeam == true)
				{
					networkView.RPC("Instantiate_Spell1", RPCMode.All, target, 5f);
				}
				
				if(iAmOnTheBlueTeam == true)
				{
					networkView.RPC("Instantiate_Spell1", RPCMode.All, target, 5f);
				}
				StartCoroutine(enableFireProjectile());
			}
		}
	}
	
	void Spell2()
	{
		if(button2Pressed == false && leftMouseButton2 == false && manaScript.myMana >= manaCost2)
		{
			if(Input.GetButtonDown("Button2"))
			{
				button2Pressed = true;
				fireProjectile.enabled = false;
				
				// An offset for y axis used for the Particle position not to appear on the ground
				particlePosition = Input.mousePosition;
				particlePosition.y += 2;
				Instantiate(spell2Target, particlePosition, Quaternion.identity);

				if(button1Pressed == true)
				{
					Destroy(GameObject.Find("Projector(Clone)"));
					button1Pressed = false;
				}
			}
		}

		if(button2Pressed == true && leftMouseButton2 == false)
		{
			if(Input.GetButtonDown("leftMouseButton"))
			{
				leftMouseButton2 = true;
				StartCoroutine (delayBeforeNextSpell (2, cooldown2));
				manaScript.ManaCost(manaCost2);
				rotatePlayerOnAttack();
				
				//Apply the effect of Spell2 
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if(Physics.Raycast(ray, out hit, 100, myLayerMask))
				{
					target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				}
			
				Quaternion newRotation = Quaternion.LookRotation(target - transform.position);
				newRotation.x = 0f;
				newRotation.z = 0f;
				transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 50);
				
				launchPosition = firePointTransform.TransformPoint(0, 0, 0.2f); // The launch position of the projectile will be just in front the player
				
				if(iAmOnTheRedTeam == true)
				{
					networkView.RPC("Instantiate_Spell2", RPCMode.All, launchPosition,
					                Quaternion.Euler(firePointTransform.eulerAngles.x + 90,
					                 myTransform.eulerAngles.y, 0), "red");
				}
				
				if(iAmOnTheBlueTeam == true)
				{
					networkView.RPC("Instantiate_Spell2", RPCMode.All, launchPosition,
					                Quaternion.Euler(firePointTransform.eulerAngles.x + 90,
					                 myTransform.eulerAngles.y, 0), "blue");
				}
				StartCoroutine(enableFireProjectile());
			}
		}
	}
	
	[RPC]
	void Instantiate_Spell1(Vector3 center, float radius)
	{
		source.PlayOneShot(spell1Ac);
		playSpellAnimation ();

		// Store all colliders that overlapSphere hits and apply damage to enemmies
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		int i = 0;
		while (i < hitColliders.Length) 
		{
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
			i++;
		}
	}
	
	[RPC]
	void Instantiate_Spell2(Vector3 position, Quaternion rotation, string team)
	{
		source.PlayOneShot(spell2Ac);
		playSpellAnimation ();

		// Access the Projectile script of the new instantiated projectile and supply the player's name and team
		GameObject go = (GameObject) Instantiate(spell2Effect, position, rotation);
		DemonSpell2 spell2Script = go.GetComponent<DemonSpell2>();
		spell2Script.team = team;
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
	
	void playSpellAnimation()
	{
		animation[spellAnimation.name].layer = 1;
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
			leftMouseButton1 = false;
		}
		if (spellNumber == 2)
		{
			Debug.Log(cooldown);
			yield return new WaitForSeconds (cooldown);
			button2Pressed = false;
			leftMouseButton2 = false;
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



