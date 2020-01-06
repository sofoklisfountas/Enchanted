using UnityEngine;
using System.Collections;

public class MovementUpdate : MonoBehaviour {

	// This script is attached to the player and ensures that every player's
	// position, rotation and scale are kept up to date across the network

	// Variables

	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private Transform myTransform;



	// Use this for initialization
	void Start () {

		if(networkView.isMine == true)
		{
			myTransform = transform;

			// ensure that everyone sees the player at the correct location
			// the moment they spawn

			networkView.RPC("updateMovement", RPCMode.OthersBuffered, 
			                myTransform.position, myTransform.rotation);
		}
		else
		{
			enabled = false;
		
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		// if the player has moved at all then fire off an RPC to update
		// the players position and rotation across the network

		if(Vector3.Distance(myTransform.position, lastPosition) >= 0.1)
		{
			// capture the player's position before the RPC is fired off
			// and use this to determine if the player has moved in the
			// if statement above

			lastPosition = myTransform.position;

			networkView.RPC("updateMovement", RPCMode.OthersBuffered, 
			                myTransform.position, myTransform.rotation);
		}

		if(Quaternion.Angle(myTransform.rotation, lastRotation) >= 1)
		{
			// capture the player's rotation before the RPC is fired off
			// and use this to determine if the player has turned in the
			// if statement above

			lastRotation = myTransform.rotation;

			networkView.RPC("updateMovement", RPCMode.OthersBuffered, 
			                myTransform.position, myTransform.rotation);
		}
	
	}

	[RPC]
	void updateMovement(Vector3 newPosition, Quaternion newRotation){

		transform.position = newPosition;
		transform.rotation = newRotation;
	}



}
