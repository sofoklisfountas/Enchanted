using UnityEngine;
using System.Collections;

public class PointAndClickMovement : MonoBehaviour {
	
	public float defaultSpeed;
	public float currentSpeed;

	private Vector3 target;
	public CharacterController controller;

	public AnimationClip idle;
	public AnimationClip run;

	private CharacterSelection teamSelectionScript;
	private PlayerStats plstats;
	private InventoryGrid inv;
	private ShopKeeper sk;
	private SpawnScript spawn;
	private Menu menu;

	public bool eventGui = false;

	public LayerMask myLayermask;

	public float animSpeed;

	void Start()
	{
		if(networkView.isMine == true)
		{
			controller = GetComponent<CharacterController>();
			teamSelectionScript = GameObject.Find("SpawnManager").GetComponent<CharacterSelection>();
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
			plstats = gameObject.GetComponent<PlayerStats>();
			inv = gameObject.GetComponent<InventoryGrid>();
			sk = gameObject.GetComponent<ShopKeeper>();
			menu = GameObject.Find("MultiplayerManager").GetComponent<Menu>();

			defaultSpeed = plstats.speedStat;
			currentSpeed = defaultSpeed;
		}
		else
		{
			enabled = false;
		}

	}

	void FixedUpdate()
	{
		if(Input.GetMouseButton(1) && eventGui)
		{
			// Locate where the player clicks on the terrain
			FindTarget();
		}


		// Move the player to the target
		Move();

		StopMoving ();

		networkView.RPC("UpdateMySpeed", RPCMode.AllBuffered, currentSpeed);

	}

	public void speedPotion()
	{
		defaultSpeed = defaultSpeed + 10f;
		networkView.RPC ("UpdateMySpeed", RPCMode.AllBuffered, defaultSpeed);
	}

	void FindTarget()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Physics.Raycast(ray, out hit, 100, myLayermask))
		{
			target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
		}
	}

	public void Move()
	{

		if(Vector3.Distance(transform.position, target) > 1)
		{
			if(spawn.spawned == true)
			{
				if(teamSelectionScript.selectedPlayer == 1)
				{
					networkView.RPC("runAnimation", RPCMode.All, 1.2f);
				}
				if(teamSelectionScript.selectedPlayer == 2)
				{
					networkView.RPC("runAnimation", RPCMode.All, 1.5f);
				}
				if(teamSelectionScript.selectedPlayer == 3)
				{
					networkView.RPC("runAnimation", RPCMode.All, 1.5f);

				}
				if(teamSelectionScript.selectedPlayer == 4)
				{
					networkView.RPC("runAnimation", RPCMode.All, 0.8f);

				}
				if(teamSelectionScript.selectedPlayer == 5)
				{
					networkView.RPC("runAnimation", RPCMode.All, 1f);

				}
				
				Quaternion newRotation = Quaternion.LookRotation(target - transform.position);
				
				newRotation.x = 0f;
				newRotation.z = 0f;

				//currentSpeed = speed;

				transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10);   // 10 is the speed of the rotation
				controller.SimpleMove(transform.forward * currentSpeed);

			}
		}
		else
		{

			if(teamSelectionScript.selectedPlayer == 1)
			{
				networkView.RPC("idleAnimation", RPCMode.All);
			}
			if(teamSelectionScript.selectedPlayer == 2)
			{
				networkView.RPC("idleAnimation", RPCMode.All);
			}
			if(teamSelectionScript.selectedPlayer == 3)
			{
				networkView.RPC("idleAnimation", RPCMode.All);
			}
			if(teamSelectionScript.selectedPlayer == 4)
			{
				networkView.RPC("idleAnimation", RPCMode.All);
			}
			if(teamSelectionScript.selectedPlayer == 5)
			{
				networkView.RPC("idleAnimation", RPCMode.All);
			}
		}
	}

	public void Slow(float rate, int duration)
	{
		currentSpeed /= rate;
		StartCoroutine(waitForSlow(duration));
	}

	IEnumerator waitForSlow(int time)
	{
		yield return new WaitForSeconds (time);
		currentSpeed = defaultSpeed;
	}

	public void SpeedBuff(float rate, int duration)
	{
		currentSpeed *= rate;
		StartCoroutine(waitForBuff(duration));
	}

	IEnumerator waitForBuff(int time)
	{
		yield return new WaitForSeconds (time);
		currentSpeed = defaultSpeed;
	}

	void StopMoving()
	{

		// Prevent the player from moving when opening inventory or shop
		
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
	void UpdateMySpeed(float moveSpeed)
	{
		currentSpeed = moveSpeed;
	}

	[RPC]
	void runAnimation(float speed)
	{
		animation[run.name].speed = speed;
		animation.CrossFade(run.name);
	}


	[RPC]
	void idleAnimation()
	{
		animation[idle.name].wrapMode = WrapMode.Loop;
		animation.CrossFade(idle.name);
	}

}

