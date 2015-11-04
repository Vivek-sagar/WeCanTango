using UnityEngine;
using System.Collections;

public class TutorialSheep : MonoBehaviour
{

	public Transform target; 
	public float speed;
	public bool waitForAnimationEnd, atGazeTarget;
	AudioSource au_source;
	Transform myTrans;
	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		au_source = GetComponent<AudioSource> ();
		//DO NOT REMOVE 
		//Code must not run until Animation Ends
		waitForAnimationEnd = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (waitForAnimationEnd)
			return;

		//If you are at the GazeTarget do not move
		if (atGazeTarget) {
			DoGetAttention ();
			return;
		}

		Vector3 direction = target.position - myTrans.position;
		myTrans.position += direction.normalized * Time.deltaTime * speed;
	}

	void OnTriggerEnter (Collider other)
	{
		//If this is a Gaze Target and is my correct Gaze Target
		if (other.CompareTag ("GazeTarget") && target.position == other.gameObject.transform.position) {
			atGazeTarget = true;
		}
	}

	public void DoGetAttention ()
	{

	}
	/// <summary>
	/// Changes the target.
	/// </summary>
	/// <param name="t">T.</param>
	public void ChangeTarget (Transform t)
	{
		DoSheepFeedback ();
		target = t;
		atGazeTarget = false;
	}

	public void DoSheepFeedback ()
	{
		au_source.Play ();
	}
}
