using UnityEngine;
using System.Collections;

public class FireSheepOut : MonoBehaviour
{

	public GameObject sheep;
	public Transform positionTrans;
	public float fireSpeed = 2.0f;
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if (Input.GetKeyDown (KeyCode.W))
		//	FireSheep ();
	}

	public void FireSheep ()
	{
		GameObject gbj = Instantiate (sheep, positionTrans.position, Quaternion.identity) as GameObject;
		gbj.SetActive (true);
		Rigidbody body = gbj.GetComponent<Rigidbody> ();
		body.velocity = positionTrans.forward * fireSpeed;
	}
}
