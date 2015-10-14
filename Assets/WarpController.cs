﻿using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	public Vector2 offset; 
	public ScrollTexture[] scrollTextureList; 
	public bool portalColored;
	public Spawner spawner;

	public Camera leftcam;
	public Camera rightcam;
	public AudioClip idle, warp;
	public PlayerLazer playerScript;
	bool teleporting, fading;
	Animator myAnim;
	AudioSource au_source;
	Color currentColor = Color.black, OnColor = new Color (0, 127f, 1f);
	BiomeScript biome;
	CameraClearFlags defaultFlag;

	float timerStartFade = 0f;
	// Use this for initialization
	void Start ()
	{
		defaultFlag = leftcam.clearFlags;
		biome = BiomeScript.Instance;
		myAnim = GetComponent<Animator> ();
		au_source = GetComponent<AudioSource> ();
		//playerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerLazer> ();
		/*foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
		}*/

		au_source.clip = idle;
		au_source.loop = true;
		au_source.Play ();
	}

	void Update ()
	{
		currentColor = portalColored ? OnColor : Color.black;
		foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
			sc.beamColor = currentColor;
		}
	}

	void OnTriggerStay (Collider other)
	{

		if (other.CompareTag ("Player")) {
			//myAnim.SetTrigger ("PortalOn");
			//teleporting = true;
			//spawner.SwapBiomeSets ();


			if (!fading) {

				if (au_source.clip != warp)
					playWarpSound ();

				timerStartFade += Time.deltaTime;
				if (timerStartFade > 1f) {
					fading = true;
					StartCoroutine (playerScript.fadeToDeathScreen (new Color (0f, 181 / 255, 1f, 0.95f), 3f));
					worldWarp ();
				}
			}

			//if (!teleporting)

		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Player") && fading) {
			StartCoroutine (ResetTeleporting ());

			//StartCoroutine (playerScript.resetFade (0.5f));
		}
	}

	IEnumerator ResetTeleporting ()
	{
		//myAnim.SetTrigger ("StopPortal");
		//teleporting = false;
		yield return new WaitForSeconds (1f);


		au_source.clip = idle;
		au_source.loop = true;
		au_source.Play ();
		timerStartFade = 0f;
		fading = false;

	}

	void playWarpSound ()
	{
		//Audio play warp
		au_source.clip = warp;
		au_source.loop = false;
		au_source.Play ();
	}

	public void worldWarp (bool reset = false)
	{
		teleporting = true;



		//
		spawner.SwapBiomeSets ();
		biome.swapMaterials ();
		biome.resetBiomes ();

		//Add visual effect stuff here later
		leftcam.clearFlags = CameraClearFlags.Skybox;
		rightcam.clearFlags = CameraClearFlags.Skybox;
	}

}
