using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetCounter : MonoBehaviour
{
	public GameObject Portal;
	public int numOfLilPets = 4;
	int count = 0;
	// Use this for initialization
	void Start ()
	{
		if (Portal == null)
			Portal = GameObject.FindGameObjectWithTag ("Portal");
		Portal.SetActive (false);
	}

	void Update ()
	{
		Portal.SetActive (count == numOfLilPets);
	}
	public void PetTriggered ()
	{
		count++;
	}
}
