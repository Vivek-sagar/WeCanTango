using UnityEngine;
using System.Collections;

public class TutorialGaze : MonoBehaviour
{

	public Transform camTrans;
	public float distance = 10f;
	public bool runGaze, gotHit;
	Ray ray;
	RaycastHit hit;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!runGaze)
			return;

		bool isHit = Physics.Raycast (camTrans.position, camTrans.forward * distance, out hit);
		Debug.DrawRay (camTrans.position, camTrans.forward * distance);
		gotHit = isHit && hit.collider.CompareTag ("TUTsheep");
	}
	
}
