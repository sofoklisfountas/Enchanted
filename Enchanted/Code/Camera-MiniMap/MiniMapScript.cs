using UnityEngine;
using System.Collections;

public class MiniMapScript : MonoBehaviour {

	private float nativeWidth = 1920;
	private float nativeHeight = 1080;
	
	private Transform myTransform;
	private Transform targetTransform;

	private Camera miniMapCamera;

	public Texture minimapTex;

	private SpawnScript spawn;

	// Use this for initialization
	void Start () {
		if(networkView.isMine)
		{
			miniMapCamera = GameObject.Find("MiniMapCamera").camera;
			miniMapCamera.camera.enabled = true;
			targetTransform = transform.FindChild("target");

			spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
		}
		else
		{
			enabled = false;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// Minimap Camera follows the player but doesn't exceed map borders

		if(spawn.amIOnTheRedTeam)
		{
			miniMapCamera.transform.position = new Vector3(Mathf.Clamp(targetTransform.position.x, 40, 60), targetTransform.position.y + 10, Mathf.Clamp(targetTransform.position.z, 30, 170));
			miniMapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

		}
		if(spawn.amIOnTheBlueTeam)
		{
			miniMapCamera.transform.position = new Vector3(Mathf.Clamp(targetTransform.position.x, 40, 60), targetTransform.position.y + 10, Mathf.Clamp(targetTransform.position.z, 30, 170));
			miniMapCamera.transform.rotation = Quaternion.Euler(90, 180, 0);
		}
	}

	void OnGUI()
	{
		
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f));

		if (spawn.spawned) 
		{
			GUI.DrawTexture(new Rect(nativeWidth-350, nativeHeight-320, 350, 320), minimapTex);	
		}
	}
}
