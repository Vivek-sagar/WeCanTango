using UnityEngine;
using System.Collections;

public class NodderControl : MonoBehaviour {

	public GameObject sphere; 

	Vector3[] positions;
	float maxacc;
	// Use this for initialization
	void Start () {
		//StartCoroutine (detectCoroutine ());
		positions = new Vector3[3];
		positions [0] = transform.position;
		positions [1] = transform.position;
		positions [2] = transform.position;
		maxacc = 0;

		sphere.GetComponent<Animator> ().StopPlayback ();
	}

	Vector3 getAcceleration()
	{
		float rdt = 1.0f / Time.deltaTime;
		return (positions [2] - 2 * positions [1] + positions [0]) * rdt * rdt * rdt;
	}

	// Update is called once per frame
	void Update () {
		positions [0] = positions [1];
		positions [1] = positions [2];
		positions [2] = transform.position;

		if(getAcceleration ().magnitude > 4000)
		{
			sphere.GetComponent<Animator> ().Play("fire");
		}

		maxacc = Mathf.Max (getAcceleration ().magnitude, maxacc);

	}

	void OnGUI()
	{
		GUI.Label (new Rect (500,10,1000,500), "ACC: " + maxacc);
	}

	//IEnumerator detectCoroutine()
	//{

	//}
}
