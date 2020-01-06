using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the  projectile and it
/// governs the behaviour of the projectile.
/// </summary>

public class Projectile : MonoBehaviour {
		
	// References
	private Transform myTransform;
	private SpawnScript spawn;

	public GameObject projectileExplosion;	//The explosion effect is attached to this in the inspector
	public float projectileSpeed = 10; // The projectiles flight speed.
	private bool expended = false; 	//Prevent the projectile from causing urther harm once it has hit something.
	private RaycastHit hit;	// A ray projected in front of the projectile to see if it will hit a recognisable collider.
	public float range = 1.0f;	// The range of that ray.	
	public float expireTime = 2; // The life span of the projectile.
	
	public string team; // Used in hit detection 
	public float damage;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
	
		StartCoroutine(DestroyMyselfAfterSomeTime()); // As soon as the projectile is instantiated start a countdown to destroy it.
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Translate the projectile in the up direction (the pointed end of the projectile).
		myTransform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);

		//If the ray hits something then execute this code.
		if(Physics.Raycast(myTransform.position,myTransform.up, out hit, range) && expended == false)
		{
			//If the collider has the tag of map then..
			if(hit.transform.tag == "map")
			{
				expended = true;

				//Instantiate an explosion effect.
				Instantiate(projectileExplosion, hit.point, Quaternion.identity);
							
				//Destroy the projectile from the scene.
				Destroy(myTransform.gameObject);
			}

			if(hit.transform.tag == "RedTeamTrigger" || hit.transform.tag == "BlueTeamTrigger")
			{
				expended = true;

				//Instantiate an explosion effect.
				Instantiate(projectileExplosion, hit.point, Quaternion.identity);

				//Destroy the projectile from the scene.
				Destroy(myTransform.gameObject);

				// Access the HealthAndDamage Script of the enemy player and inform them that they have been attacked.
				// Also set the projectile damage equal to the attackStat of the player
				if(hit.transform.tag == "BlueTeamTrigger" && team == "red")
				{
					PlayerStats plstats = spawn.redspawn.GetComponent<PlayerStats>();
					HealthAndDamage HDScript = hit.transform.GetComponent<HealthAndDamage>();

					HDScript.attackDamage = plstats.attackStat;
					HDScript.iWasJustAttacked = true;
					HDScript.hitByProjectile = true;
				}

				// Access the HealthAndDamage Script of the enemy player and inform them that they have been attacked.
				// Also set the projectile damage equal to the attackStat of the player
				if(hit.transform.tag == "RedTeamTrigger" && team == "blue")
				{
					PlayerStats plstats = spawn.bluespawn.GetComponent<PlayerStats>();
					HealthAndDamage HDScript = hit.transform.GetComponent<HealthAndDamage>();

					HDScript.attackDamage = plstats.attackStat;
					HDScript.iWasJustAttacked = true;
					HDScript.hitByProjectile = true;
				}
			}
		}
	}
	
	IEnumerator DestroyMyselfAfterSomeTime()
	{
		//Wait for the timer to count up to the expireTime and then destroy the projectile.
		yield return new WaitForSeconds(expireTime);
		Destroy(myTransform.gameObject);
	}
}









