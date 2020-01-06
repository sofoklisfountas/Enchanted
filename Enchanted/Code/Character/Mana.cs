using UnityEngine;
using System.Collections;

public class Mana : MonoBehaviour {

	public float myMana = 100;
	public float maxMana = 100;
	public float manaRegenRate = 1.3f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(myMana < maxMana)
		{
			myMana = myMana + manaRegenRate * Time.deltaTime;
		}

		if(myMana > maxMana)
		{
			myMana = maxMana;
		}
	}

	public void ManaCost(float manaCost)
	{
		myMana = myMana - manaCost;
	}

	public void ManaRegenBuff(float rate, int duration)
	{
		manaRegenRate *= rate;
		StartCoroutine(waitForBuff(duration));
	}

	IEnumerator waitForBuff(int time)
	{
		yield return new WaitForSeconds (time);
		manaRegenRate = 1.3f;
	}

	public void manaPotion()
	{
		myMana = myMana + 40f;
	}


}
