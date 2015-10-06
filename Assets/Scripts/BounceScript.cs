using UnityEngine;
using System.Collections;

public class BounceScript : MonoBehaviour
{
	Vector3 localOrigin;
	public Vector3 moveBy;
	Transform myTrans;
	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		localOrigin = myTrans.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
		myTrans.localPosition += moveBy * Time.deltaTime;
	}

	public void ResetOrigin ()
	{
		myTrans.localPosition = localOrigin;
	}
}
