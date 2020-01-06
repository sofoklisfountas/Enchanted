using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	
	// Variables
	
	private Camera myCamera;
	private Transform targetTransform;
	private SpawnScript spawn;

	//Variables from camera zoom
	private float distance = 60;
	private float sensitivityDistance = 5;
	private float damping = 2.5f;
	private float minFOV = 40;
	private float maxFOV = 70;
	
	void Start () {

		if(networkView.isMine == true)
		{	
			myCamera = Camera.main;  // find the camera
			
			distance = myCamera.fieldOfView;
			targetTransform = transform.FindChild("target");
			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
			
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		if(spawn.amIOnTheRedTeam)
		{
			myCamera.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y + 10, targetTransform.position.z - 10);
			myCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
		}
		if(spawn.amIOnTheBlueTeam)
		{
			myCamera.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y + 10, targetTransform.position.z + 10);
			myCamera.transform.rotation = Quaternion.Euler(45, 180, 0);
		}

		distance -= Input.GetAxis("Mouse ScrollWheel") * sensitivityDistance;
		distance = Mathf.Clamp(distance, minFOV, maxFOV);
		myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, distance, Time.deltaTime * damping);
	}
	
}
