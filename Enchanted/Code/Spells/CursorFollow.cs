using UnityEngine;
using System.Collections;

public class CursorFollow : MonoBehaviour {
	
	private Transform myTransform;
	public LayerMask myLayerMask;
	
	// Use this for initialization
	void Start ()
	{
		myTransform	 = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
		// Sets the position of this gameObject to be equal to the position of the mouse cursor.
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100, myLayerMask))
		{
			myTransform.position = hit.point;
			
		}
		
		//Destroy this gameObject if leftMouseButton has been pressed
		if(Input.GetButtonDown("leftMouseButton"))
		{
			Destroy(myTransform.gameObject);
		}
		
	}
}
