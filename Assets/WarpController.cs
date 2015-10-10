﻿using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	public Vector2 offset; 
	public ScrollTexture[] scrollTextureList; 
	public bool portalColored;
	public Spawner spawner;
	bool teleporting;
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
		currentColor = portalColored ? OnColor : Color.black;
		foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
			sc.beamColor = currentColor;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Player") && !teleporting) {
			myAnim.SetTrigger ("PortalOn");
			teleporting = true;
			spawner.SwapBiomeSets ();
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Player")) {
			ResetTeleporting ();
		}
	}

	public void ResetTeleporting ()
	{
		myAnim.SetTrigger ("StopPortal");
		teleporting = false;
	}


}
