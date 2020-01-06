using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

	// References to other scripts
	private SpawnScript spawn;
	public MultiplayerScript multiplayerScript;

	// Audio
	public AudioSource source;

	public AudioClip menuMusic;
	public AudioClip gameMusic;

	public bool audioEnabled = false;


	// Use this for initialization
	void Start () 
	{
		spawn = GameObject.Find("SpawnManager").GetComponent<SpawnScript>();
		multiplayerScript = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerScript>();
		source = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Menu background music
		if(!audioEnabled)
		{
			if(!spawn.spawned)
			{
				audio.clip = menuMusic;
				
				source.PlayOneShot(audio.clip);
				
				audioEnabled = true;
				
				StartCoroutine("waitForAudioEndMenu");
			}
		}

		// Check if the player has chosen to create a server and mute the audio
		if(multiplayerScript.server)
		{
			source.mute = true;
		}
		else
		{
			source.mute = false;
		}

		// In-Game background music
		if(audioEnabled)
		{
			if (spawn.spawned) 
			{
				audio.clip = gameMusic;
				
				source.PlayOneShot(audio.clip);
				
				audioEnabled = false;
				
				StartCoroutine("waitForAudioEndGame");
			}
		}
	}


	// Used in playling the audio clip only once in the Update function and starting over when it's finished
	IEnumerator waitForAudioEndMenu()
	{
		yield return new WaitForSeconds(128);

		audioEnabled = false;
	}

	IEnumerator waitForAudioEndGame()
	{
		yield return new WaitForSeconds(132);
		
		audioEnabled = true;
	}
}
