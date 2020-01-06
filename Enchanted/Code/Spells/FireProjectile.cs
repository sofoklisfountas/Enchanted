using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player and allows
/// them to fire projectiles.
/// </summary>

public class FireProjectile : MonoBehaviour {
	
	// References
	private Transform myTransform;	
	private Transform firePointTransform;
	private InventoryGrid inv;
	private ShopKeeper sk;
	private Menu menu;
	private AudioSource source;
	private SpawnScript spawn; // Used to find out wich team the player is on
	
	public GameObject projectile;
	private Vector3 target; // The point the Raycast hits.
	public LayerMask myLayerMask; // Ignore all layers the Raycast hits except this one.
	private Vector3 launchPosition;	// The position at which the projectile should be instantiated.
	
	//Used to control the rate of fire.
	private float fireRate = 1.5f;
	private float nextFire;
	
	// Determine which team the player is on
	public bool iAmOnTheRedTeam;
	public bool iAmOnTheBlueTeam;
	
	private bool eventGui; // Used to prevent player from shooting when Shop, Bag or OptionsMenu are open.
	
	//Animation clip
	public AnimationClip attackAnim;
	
	//Audio clip
	public AudioClip projectileAc;
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine)
		{
			myTransform = transform;
			firePointTransform = myTransform.FindChild("FirePoint");
			menu = GameObject.Find("MultiplayerManager").GetComponent<Menu>();
			inv = gameObject.GetComponent<InventoryGrid>();
			sk = gameObject.GetComponent<ShopKeeper>();
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
			
			if(spawn.amIOnTheRedTeam)
			{
				iAmOnTheRedTeam = true;
			}
			if(spawn.amIOnTheBlueTeam)
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
		if(Input.GetMouseButton(0) && Time.time > nextFire && eventGui)
		{	
			rotatePlayerOnAttack();
			Shoot();
		}
		
		StopShooting();
	}
	
	void StopShooting()
	{
		// Prevent player from shooting when Shop, Bag or 		  OptionsMenu are open.
		if(inv.showBag || sk.shopOpen || menu.OptionsPressed)
		{
			eventGui = false;
		}
		else
		{
			eventGui = true;
		}
	}
	
	void Shoot()
	{
		launchPosition = firePointTransform.TransformPoint(0, 0, 0.2f);	// The launch position of the projectile will be just in front the player
		nextFire = Time.time + fireRate;
		
		networkView.RPC("attackAnimation", RPCMode.All);
		
		// Create the projectile at the launchPosition and tilt its angle so that its horizontal using the angle eulerAngles.x + 90.
		// Also make it team specific
		if(iAmOnTheRedTeam)
		{
			networkView.RPC("SpawnProjectile", RPCMode.All, launchPosition,
			                Quaternion.Euler(firePointTransform.eulerAngles.x + 90,
			                 myTransform.eulerAngles.y, 0), "red");
		}
		
		if(iAmOnTheBlueTeam)
		{
			networkView.RPC("SpawnProjectile", RPCMode.All, launchPosition,
			                Quaternion.Euler(firePointTransform.eulerAngles.x + 90,
			                 myTransform.eulerAngles.y, 0), "blue");
		}
	}
	
	void rotatePlayerOnAttack()
	{
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
	}
	
	[RPC]
	void SpawnProjectile(Vector3 position, Quaternion rotation, string team)
	{
		source.PlayOneShot(projectileAc);
		
		// Access the Projectile script of the new instantiated projectile and supply the player's name and team.
		GameObject go = (GameObject) Instantiate(projectile, position, rotation);
		
		Projectile bScript = go.GetComponent<Projectile>();
		bScript.team = team;
	}
	
	[RPC]
	void attackAnimation()
	{
		animation[attackAnim.name].layer = 1; // The animation clip's priority
		animation[attackAnim.name].speed = 3; 
		animation[attackAnim.name].wrapMode = WrapMode.Once; // Playback type of the animation clip
		animation.CrossFade(attackAnim.name); 
	}
}
