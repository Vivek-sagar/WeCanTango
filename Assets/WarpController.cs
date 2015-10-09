﻿using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	public Vector2 offset; 
	public ScrollTexture[] scrollTextureList; 
	Animator myAnim;
	Color currentColor = Color.black, OnColor = new Color (0, 127f, 1f);
	// Use this for initialization
	void Start ()
	{
		myAnim = GetComponent<Animator> ();
		/*foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
		}*/
	}

	void Update ()
	{
		foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
			sc.beamColor = currentColor;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Player")) {
			myAnim.SetTrigger ("PortalOn");
			currentColor = OnColor;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Player")) {
			myAnim.SetTrigger ("StopPortal");
			currentColor = Color.black;
		}
	}


}
