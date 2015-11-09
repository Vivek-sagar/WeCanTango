//#define GazeTut
using UnityEngine;
using System.Collections;

public class IntroTutorialManager : MonoBehaviour
{

	public enum TutorialPhase
	{
		Wait,
		PocketWatchSwing,
		SheepAppear,
		SheepGaze,
		DoneSwing,
		OpenPortal,
		Finish,
	};
	public TutorialPhase tutorialPhase;
	public GameObject player;
	public TutorialGaze playerGazeScript;
	public TutorialSheep sheepScript;
	public PokeDector pokeScript;
	public GameObject pocketWatch, ThePet;
	public Transform watchTrans;
	public GameObject tutorialSheep, gazeTutorialGameObjects;
	public Transform[] gazeTargets; //Need the places the sheep moves to for the player to gaze

	
	public GameObject ItemSpawner, EnvironmentSpawner;
	public BiomeScript biome;
	//public Camera backCam;
	public Material[] voxelMats;

	public Animator myAnim;
	public AudioSource auSource;
	public GameObject mainLight;
	public GameObject textObj;

	int gazeCount = 0;
	// Use this for initialization
	void Start ()
	{
		this.transform.position = new Vector3 (0, player.transform.position.y, -1.5f);
		auSource.pitch = 0.75f;
		ThePet.transform.position = watchTrans.position;

		tutorialPhase = TutorialPhase.Wait;
		playerGazeScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<TutorialGaze> ();
		SetMeshRenderersInChildren (tutorialSheep, false);
		SetMeshRenderersInChildren (gazeTutorialGameObjects, false);
		SetMainGameObjects (false);
	}

	void SetMainGameObjects (bool state)
	{
		ThePet.SetActive (state);
		ItemSpawner.SetActive (state);
		mainLight.SetActive (state);
		EnvironmentSpawner.SetActive (state);
	}

	void Update ()
	{
		//if (Input.GetKeyDown (KeyCode.Space))
		//	tutorialPhase = TutorialPhase.PocketWatchSwing;

	}

	// Update is called once per frame
	void FixedUpdate ()
	{

		if (tutorialPhase == TutorialPhase.Wait && pokeScript.triggered) {
			pocketWatch.transform.LookAt (player.transform.position);
			tutorialPhase = TutorialPhase.PocketWatchSwing;
		} else if (tutorialPhase == TutorialPhase.PocketWatchSwing) {
			// Debug.LogError("AT PocketWatchSwing !!!");
			//Disable PocketWatch SetMeshRenderersInChildren (pocketWatch, false);
			//auSource.pitch = 1f;
			myAnim.SetTrigger ("OpenPocketWatch");
			textObj.SetActive (false);

			tutorialPhase = TutorialPhase.SheepAppear;
		} else if (tutorialPhase == TutorialPhase.DoneSwing) {
			DonePocketWatchSwing ();

			//REMOVE THIS LATER
			//Enviroment Spawner should be setActive true in Finish Gaze function
			SetMainGameObjects (true);
			tutorialPhase = TutorialPhase.Finish;
		} else if (tutorialPhase == TutorialPhase.SheepGaze) {
			WaitForGaze ();
		} 
		
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="phase"></param>
	public void SetTutorialPhase (TutorialPhase phase)
	{
		tutorialPhase = phase;
	}

	public void PlayAudio ()
	{
		auSource.Play ();
	}

	/// <summary>
	/// BLASAEOILAHEAEU Unity wont let me use any public function for animation events ~~~!!
	/// 
	/// </summary>
	public void SetSheepOn ()
	{
#if GazeTut
		//Disabling Sheep for now
		SetMeshRenderersInChildren (tutorialSheep, true);
#endif
		auSource.Stop ();
		tutorialPhase = TutorialPhase.DoneSwing;
	}

	/// <summary>
	/// Sets the mesh renderers in children.
	/// </summary>
	/// <param name="parent">Parent.</param>
	/// <param name="state">If set to <c>true</c> state.</param>
	public void SetMeshRenderersInChildren (GameObject parent, bool state)
	{
		MeshRenderer[] renders = parent.GetComponentsInChildren<MeshRenderer> ();

		foreach (MeshRenderer child in renders) {
			child.enabled = state;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void WaitForGaze ()
	{
		playerGazeScript.runGaze = sheepScript.atGazeTarget;

		//If the sheep is not at the target, then do Nothing
		if (!sheepScript.atGazeTarget) {
			return;
		} 
		//If the sheep is at the target, check to see if the Player Gazed at it
		else if (sheepScript.atGazeTarget) {
			if (playerGazeScript.gotHit) {
				gazeCount++;

				if (gazeCount > 2) {
					FinishGaze ();
				} else
					sheepScript.ChangeTarget (gazeTargets [gazeCount]);
			}
		}
	}

	/// <summary>
	/// Done the pocket watch swing.
	/// </summary>
	void DonePocketWatchSwing ()
	{
		SetMeshRenderersInChildren (pocketWatch, false);

#if GazeTut
		//After Pocket Watch Swing is done, allow the TutorialSheep and TutorialGaze 
		//script to start doing stuff
		sheepScript.waitForAnimationEnd = false;
		playerGazeScript.waitForAnimationEnd = false;

		//Set the Sheep's Gaze Target
		sheepScript.ChangeTarget (gazeTargets [gazeCount]);
		//Now Start the Sheep Gaze 
		tutorialPhase = TutorialPhase.SheepGaze;
#endif
		mainLight.SetActive (true);
		mainLight.transform.rotation = Quaternion.Euler (386f, 71f, 126f);
		biome.resetBiomes ();
		biome.swapMaterials (ref voxelMats);
	}

	/// <summary>
	/// Finishs the gaze.
	/// </summary>
	void FinishGaze ()
	{
		ThePet.SetActive (true);
		ItemSpawner.SetActive (true);
		EnvironmentSpawner.SetActive (true);
		sheepScript.DeActivate ();
		tutorialPhase = TutorialPhase.Finish;
	}
}
