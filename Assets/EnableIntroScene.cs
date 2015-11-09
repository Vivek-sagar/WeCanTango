using UnityEngine;
using System.Collections;

public class EnableIntroScene : MonoBehaviour
{

	public GameObject[] sceneObjects;
	public Canvas canvas;
	// Use this for initialization
	void Start ()
	{
		foreach (GameObject obj in sceneObjects)
			obj.SetActive (false);
	}

	public void StartIntroScene ()
	{
		foreach (GameObject obj in sceneObjects)
			obj.SetActive (true);
		canvas.gameObject.SetActive (false);
	}
}
