using UnityEngine;
using System.Collections;

public class TutorialGaze : MonoBehaviour
{

	public Transform camTrans;
	public float distance = 10f;
	public bool runGaze, waitForAnimationEnd, gotHit;
	Ray ray;
	RaycastHit hit;
	// Use this for initialization
	void Start ()
	{
		//DO NOT REMOVE 
		//Code must not run until Animation Ends
		waitForAnimationEnd = true;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!runGaze || waitForAnimationEnd)
			return;

		bool isHit = Physics.Raycast (camTrans.position, camTrans.forward * distance, out hit);
		//Debug.DrawRay (camTrans.position, camTrans.forward * distance);
		gotHit = isHit && hit.collider.CompareTag ("TUTsheep");
	}
	
}
