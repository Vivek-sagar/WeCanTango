using UnityEngine;
using System.Collections;

public class FollowScript : MonoBehaviour
{

	public Transform target;
	Transform myTrans;
	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		myTrans.rotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update ()
	{
		myTrans.position = target.position;
		myTrans.rotation = Quaternion.Euler (new Vector3 (0, myTrans.rotation.eulerAngles.y, 0));

	}
}
