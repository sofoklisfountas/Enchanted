using UnityEngine;
using System.Collections;

public class LogoScreen : MonoBehaviour {

	private float nativeWidth = 1920;
	private float nativeHeight = 1080;

	public Texture logo;
	public Texture backgroundImage;


	// Use this for initialization
	void Start () {
	
		StartCoroutine (waitForLogo ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / nativeWidth, (float)Screen.height / nativeHeight);
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (resizeRatio.x, resizeRatio.y, 1.0f)); 


		GUI.DrawTexture(new Rect(0, 0, nativeWidth, nativeHeight), backgroundImage);
		GUI.DrawTexture(new Rect(nativeWidth/2-709, nativeHeight/2-425.5f, 1418, 851), logo);
	}

	IEnumerator waitForLogo()
	{
		float fadeInTime = GameObject.Find ("Fading").GetComponent<Fading>().BeginFade(-1);
		yield return new WaitForSeconds (fadeInTime+1);
		yield return new WaitForSeconds (2);
		float fadeOutTime = GameObject.Find ("Fading").GetComponent<Fading>().BeginFade(1);
		yield return new WaitForSeconds (fadeOutTime+1);

		Application.LoadLevel(("Game"));
	}


}
