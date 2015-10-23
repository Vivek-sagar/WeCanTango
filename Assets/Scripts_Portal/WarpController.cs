using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	Vector2 offset; 
	ScrollTexture[] scrollTextureList; 
	bool portalColored;
	ItemsList itemsList;
	public AudioClip idle, warp;
	public PlayerLazer playerScript;	
	public Material[] newWorldMaterial;	
	public EnvironmentSpawner spawner;
	public GameObject[] portalEnvironmentList;
	bool teleporting, fading;
	Animator myAnim;
	AudioSource au_source;
	Color currentColor = Color.black, OnColor = new Color (0, 127f, 1f);
	CameraClearFlags defaultFlag;

	float timerStartFade = 0f;
	// Use this for initialization
	void Start ()
	{
		myAnim = GetComponent<Animator> ();
		au_source = GetComponent<AudioSource> ();
		playerScript = GameObject.FindGameObjectWithTag ("PlayerCollider").GetComponent<PlayerLazer> ();
		/*foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
		}*/

		au_source.clip = idle;
		au_source.loop = true;
		au_source.Play ();
	}

	/*void Update ()
	{
		currentColor = portalColored ? OnColor : Color.black;
		foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
			sc.beamColor = currentColor;
		}
	}*/

	void Update ()
	{

	}

	void OnTriggerStay (Collider other)
	{

		if (other.CompareTag ("PlayerCollider")) {
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
					warpToNewWorld ();
				}
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("PlayerCollider") && fading) {
			StartCoroutine (ResetTeleporting ());

			//StartCoroutine (playerScript.resetFade (0.5f));
		}
	}

	IEnumerator ResetTeleporting ()
	{
		//myAnim.SetTrigger ("StopPortal");
		//teleporting = false;
		au_source.Stop ();
		yield return new WaitForSeconds (2f);


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

	/// <summary>
	/// Warps to New World
	/// </summary>
	/// <param name="reset">If set to <c>true</c> reset.</param>
	public void warpToNewWorld (bool reset = false)
	{
		teleporting = true;

		//Swaps the Portals ItemList and Materials
		spawner.SwapEnvironments (ref newWorldMaterial, ref portalEnvironmentList);
		//biome.swapMaterials ();
		//biome.resetBiomes ();

	}

}
