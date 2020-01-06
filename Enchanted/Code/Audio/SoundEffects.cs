using UnityEngine;
using System.Collections;

// This script contains an array of audio clips for GUI interactions and changes in flag states during the game.
// These audio clips are the same for all players.

public class SoundEffects : MonoBehaviour {

	public AudioClip[] audioClips = new AudioClip[12];
	public AudioSource source;
	
	// Use this for initialization
	void Start () 
	{
		source = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// This method is accessed by other scripts when an audio clip needs to be played
	public void PlayAudio(int index)
	{
		source.PlayOneShot(audioClips[index]);
	}
}
