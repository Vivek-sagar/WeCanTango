using UnityEngine;
using System.Collections;

public class TutorialSheep : MonoBehaviour
{

	public Transform target; 
	public float speed;
	public bool waitForAnimationEnd, atGazeTarget;
	public Vector3 direction;

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

		direction = (target.position - myTrans.position) * Time.deltaTime * speed;
		myTrans.position += direction;
	}

	void OnTriggerStay (Collider other)
	{
		//If this is a Gaze Target and is my target Gaze Target
		if (other.CompareTag ("GazeTarget") && target.position == other.gameObject.transform.position) {
			atGazeTarget = true;
			//Debug.LogError ("Sheep At GazeTarget");
		}
	}

	void OnTriggerExit (Collider other)
	{
		//If this is a Gaze Target and is my target Gaze Target
		if (other.CompareTag ("GazeTarget") && target.position == other.gameObject.transform.position) {
			atGazeTarget = false;
			//Debug.LogError ("Sheep At GazeTarget");
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
		// Debug.LogError("Sheep Change Target "+t.name);
		DoSheepFeedback ();
		target = t;
		atGazeTarget = false;
	}

	public void DoSheepFeedback ()
	{
		au_source.Play ();
	}
}
