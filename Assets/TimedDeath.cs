using UnityEngine;
using System.Collections;

public class TimedDeath : MonoBehaviour
{

	float timer = 0f, maxTime = 3f;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		if (timer >= maxTime)
			Destroy (this.gameObject);
	}
}
