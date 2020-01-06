using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour {

	private Transform myTransform;
	private int expireTime = 2;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		
		//As soon as the projectile is created start a countdown to destroy it.
		StartCoroutine(DestroyMyselfAfterSomeTime());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator DestroyMyselfAfterSomeTime()
	{
		//Wait for the timer to count up to the expireTime and then destroy the projectile.
		yield return new WaitForSeconds(expireTime);
		Destroy(myTransform.gameObject);

	}
}
