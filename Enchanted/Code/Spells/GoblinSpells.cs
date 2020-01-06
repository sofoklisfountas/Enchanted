using UnityEngine;
using System.Collections;

public class GoblinSpells : MonoBehaviour {
	
	// References
	private Mana manaScript;
	private SpawnScript spawn;
	private AudioSource source;
	private Transform myTransform;
	private Transform firePointTransform;
	private FireProjectile fireProjectile; 	//Instance of the FireProjectile script so we can turn it on/off when needed(when spell button have pressed)

	public Projector projector; // Gameobject type of Projector for targeting
	public GameObject spell2Target; // Gameobject type of ParticleSystem for targeting
	private Vector3 particlePosition; // The position of spell2 Target

	// The spell effects
	public GameObject bigArrow;
	public GameObject smallArrow;

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
	
	private float manaCost1 = 35f;
	private float manaCost2 = 30f;
	private string manaMess = "";

	private float cooldown1 = 8f;
	private float cooldown2 = 6f;

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
				fireProjectile.enabled = false;
				particlePosition = Input.mousePosition;
				particlePosition.y += 2;
				Instantiate(spell2Target, particlePosition, Quaternion.identity);

				if(button2Pressed == true)
				{
					Destroy(GameObject.Find("Projector(Clone)"));
					button2Pressed = false;
				}
			}
		}
		
		if(button1Pressed == true && leftMouseButton1 == false)
		{
			if(Input.GetButtonDown("leftMouseButton"))
			{
				leftMouseButton1 = true;
				StartCoroutine (delayBeforeNextSpell (1, cooldown1));
				manaScript.ManaCost(manaCost1);
				rotatePlayerOnAttack();
				
				//Apply the effect of the AoE Spell at the position of the mouse cursor
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if(Physics.Raycast(ray, out hit, 100, myLayerMask))
				{
					target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				}

				Quaternion newRotation = Quaternion.LookRotation(target - transform.position);
				newRotation.x = 0f;
				newRotation.z = 0f;
				transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 80);
				
				launchPosition = firePointTransform.TransformPoint(0, 0, 0.2f);
				
				if(iAmOnTheRedTeam == true)
				{
					networkView.RPC("Instantiate_Spell1", RPCMode.All, launchPosition,
					                Quaternion.Euler(firePointTransform.eulerAngles.x + 90,
					                 myTransform.eulerAngles.y, 0), "red");		
				}
				
				if(iAmOnTheBlueTeam == true)
				{
					networkView.RPC("Instantiate_Spell1", RPCMode.All, launchPosition,
					                Quaternion.Euler(firePointTransform.eulerAngles.x + 90,
					                 myTransform.eulerAngles.y, 0), "blue");	
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
				Instantiate(projector, myTransform.position ,Quaternion.Euler(90,0,0));

				if(button1Pressed == true)
				{
					Destroy(GameObject.Find("Spell2Target(Clone)"));
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
				
				//Apply the effect of Spell_2
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
				
				launchPosition = firePointTransform.TransformPoint(0, 0, 0.2f);
				
				
				
				
				if(iAmOnTheRedTeam == true)
				{
					networkView.RPC("Instantiate_Spell2", RPCMode.All, launchPosition,
					                Quaternion.Euler(firePointTransform.eulerAngles.x, myTransform.eulerAngles.y, 0), "red");
				}
				
				if(iAmOnTheBlueTeam == true)
				{
					networkView.RPC("Instantiate_Spell2", RPCMode.All, launchPosition,
									Quaternion.Euler(firePointTransform.eulerAngles.x, myTransform.eulerAngles.y, 0), "blue");
				}
				StartCoroutine(enableFireProjectile());
			}
		}
	}
	
	[RPC]
	void Instantiate_Spell1(Vector3 position, Quaternion rotation, string team)
	{
		source.PlayOneShot(spell1Ac);
		playSpellAnimation();
		
		// Access the ArrowScript of the new instantiated projectile and supply the player's team
		GameObject go = (GameObject) Instantiate(bigArrow, position, rotation);
		
		ArrowSpell arrowSpell = go.GetComponent<ArrowSpell>();
		arrowSpell.bigArrow = true;
		arrowSpell.team = team;
		
	} 
	
	[RPC]
	void Instantiate_Spell2(Vector3 position, Quaternion rotation, string team)
	{
		source.PlayOneShot(spell2Ac);
		playSpellAnimation();
		
		// Access the ArrowScript of the new instantiated projectile and supply the player's team
		GameObject go = (GameObject) Instantiate(smallArrow, position, rotation);
		
		ArrowSpell arrowSpell = go.GetComponentInChildren<ArrowSpell>();
		arrowSpell.arrowsSlow = true;
		arrowSpell.team = team;
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
		animation[spellAnimation.name].speed = 4;
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


