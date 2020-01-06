using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player and allows
/// them to do melee damage.
/// </summary>

public class MeleeSystem : MonoBehaviour {

	// References
	private Menu menu;
	private ShopKeeper sk;
	private InventoryGrid inv;
	private SpawnScript spawn;
	private AudioSource source;
	
	private RaycastHit hit; // A ray projected in front of the projectile to see if it will hit a recognisable collider.
	private Vector3 target; // The point the Raycast hits.
	public LayerMask myLayerMask; // Ignore all layers the Raycast hits except this one.
	private float autoAttackDamage; 
	private float radius = 3f; // The radius tha the melee player can do damage with a single attack

	// Used to control the rate of fire.
	private float nextAttack = 2f; 
	public float attackRate = 1.3f;

	// Determine which team the player is on
	public bool iAmOnTheRedTeam = false;
	public bool iAmOnTheBlueTeam = false;

	// Animation clips
	public AnimationClip attackAnim;

	// Audio clips
	public AudioClip meleeAc;

	private bool eventGui = false; // Used to prevent player from shooting when Shop, Bag or OptionsMenu are open.
	
	// Use this for initialization
	void Start () {
		
		if(networkView.isMine == true)
		{
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
			inv = gameObject.GetComponent<InventoryGrid>();
			sk = gameObject.GetComponent<ShopKeeper>();
			menu = GameObject.Find("MultiplayerManager").GetComponent<Menu>();
			source = gameObject.GetComponent<AudioSource>();

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
	void Update () {
		
		if(Input.GetMouseButton(0) && Time.time > nextAttack && eventGui)
		{
			nextAttack = Time.time + attackRate;
			networkView.RPC("MeleeAttack", RPCMode.All, transform.position, radius, transform.name, attackRate);

			//Rotate the player where mouse cursor points when attacking
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out hit, 100, myLayerMask))
			{
				target = hit.point;
			}

			Quaternion newRotation = Quaternion.LookRotation(target - transform.position);
			newRotation.x = 0f;
			newRotation.z = 0f;
			transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 80);
		}

		if(inv.showBag || sk.shopOpen || menu.OptionsPressed)
		{
			eventGui = false;
		}
		else
		{
			eventGui = true;
		}
	}
	
	
	[RPC]
	void MeleeAttack(Vector3 center, float radius, string originatorName, float attackSpeed)
	{
		source.PlayOneShot (meleeAc);

		animation[attackAnim.name].layer = 1;
		animation[attackAnim.name].speed = 2 * (1/attackSpeed);
		animation[attackAnim.name].wrapMode = WrapMode.Once;
		animation.CrossFade(attackAnim.name);

		Collider[] collider = Physics.OverlapSphere(center, radius);
		int i = 0;

		while(i < collider.Length)
		{
			if(iAmOnTheRedTeam && collider[i].transform.tag == "BlueTeamTrigger")
			{
				//Check if enemy is in an angle in front of attacker and then apply the damage and the rest
				if( Vector3.Angle(transform.forward, collider[i].transform.position - transform.position) < 60) 
				{
					PlayerStats plstats = spawn.redspawn.GetComponent<PlayerStats>();
					HealthAndDamage HDScript = collider[i].transform.GetComponent<HealthAndDamage>();

					HDScript.attackDamage = plstats.attackStat;
					HDScript.iWasJustAttacked = true;
					HDScript.hitByMelee = true;
				}
			}
			if(iAmOnTheBlueTeam && collider[i].transform.tag == "RedTeamTrigger")
			{
				//Check if enemy is in an angle in front of attacker and then apply the damage and the rest
				if( Vector3.Angle(transform.forward, collider[i].transform.position - transform.position) < 60) 
				{
					PlayerStats plstats = spawn.bluespawn.GetComponent<PlayerStats>();
					HealthAndDamage HDScript = collider[i].transform.GetComponent<HealthAndDamage>();

					HDScript.attackDamage = plstats.attackStat;
					HDScript.iWasJustAttacked = true;
					HDScript.hitByMelee = true;
				}
			}
			i++;
		}
	}

	public void IncreaseAttackSpeed(float rate, int duration)
	{
		attackRate = rate;
		StartCoroutine (DefaultAttackSpeed (duration));
	}

	IEnumerator DefaultAttackSpeed(int time)
	{
		yield return new WaitForSeconds(time);
		attackRate = 1.5f;
	}
}
