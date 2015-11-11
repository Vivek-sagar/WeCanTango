//#define GazeTut
using UnityEngine;
using System.Collections;

public class IntroTutorialManager : MonoBehaviour
{

	public enum TutorialPhase
	{
		SetUpHypno,
		Wait,
		PocketWatchSwing,
		SheepAppear,
		SheepGaze,
		DoneSwing,
		OpenPortal,
		Finish,
	};
	public TutorialPhase tutorialPhase;
	public Transform playerTrans, watchTrans;
	public TutorialSheep sheepScript;
	public PokeDector pWatchPokeScript, setUpPokeScript;
	public GameObject TheSheepDog, tutorialSheep, ItemSpawner, EnvironmentSpawner, MainLight, textObj;

	public BiomeScript biome;
	//public Camera backCam;
	public Material[] voxelMats;

	VoxelExtractionPointCloud vxe;
	Animator myAnim;
	AudioSource auSource;

	Transform[] gazeTargets; //The places the sheep moves to for the player to gaze
	GameObject pocketWatch, gazeTutorialGameObjects;
	TutorialGaze playerGazeScript;
	ScreenFade screenFadeScript;
	int gazeCount = 0;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		myAnim = GetComponent<Animator> ();
		auSource = GetComponent<AudioSource> ();
		screenFadeScript = playerTrans.GetComponent<ScreenFade> ();
		playerGazeScript = playerTrans.GetComponent<TutorialGaze> ();

		pocketWatch = pWatchPokeScript.gameObject;
		pWatchPokeScript.enabled = false;
		SetMeshRenderersInChildren (pocketWatch, false);

		auSource.pitch = 0.75f;
		TheSheepDog.transform.position = watchTrans.position;


		tutorialPhase = TutorialPhase.SetUpHypno;
		SetMeshRenderersInChildren (tutorialSheep, false);
		//Disable for now, will use GazeTutorial Later
		//SetMeshRenderersInChildren (gazeTutorialGameObjects, false);
		SetMainGameObjects (false);
	}

	void SetMainGameObjects (bool state)
	{
		TheSheepDog.SetActive (state);
		ItemSpawner.SetActive (state);
		MainLight.SetActive (state);
		EnvironmentSpawner.SetActive (state);
	}

	IEnumerator setupPocketWatch ()
	{
		Transform pocketTrans = pocketWatch.transform;
		pocketTrans.position = setUpPokeScript.getSafeSpawnPos ();
		setUpPokeScript.gameObject.SetActive (false);

		pWatchPokeScript.enabled = true;
		SetMeshRenderersInChildren (pocketWatch, true);

		yield return null;
		auSource.Play ();

		/*bool hit = false;
		RaycastHit hitInfo;

		while (!hit) {
			hit = Physics.Raycast (pocketWatch.transform, pocketWatch.transform.forward, 2f, out hitInfo);
			hit = hitInfo.transform.CompareTag ("Player");

		}*/

		pocketTrans.LookAt (playerTrans.position);
		pocketTrans.rotation = Quaternion.Euler (new Vector3 (0, pocketTrans.rotation.eulerAngles.y, 0));
		textObj.SetActive (true);

	}
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (tutorialPhase == TutorialPhase.SetUpHypno && setUpPokeScript.triggered) {
			StartCoroutine (setupPocketWatch ());
			tutorialPhase = TutorialPhase.Wait;
		} else if (tutorialPhase == TutorialPhase.Wait && pWatchPokeScript.triggered) {
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

#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.Q)) {
			Vec3Int chunkCoords = vxe.getChunkCoords (watchTrans.position);
			//		
			biome.swapMaterialsThread (ref voxelMats, chunkCoords.x, chunkCoords.z);

		} 
		if (Input.GetKeyDown (KeyCode.E)) {
			biome.swapMaterials (ref voxelMats);

		}
#endif
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
		Destroy (pocketWatch);
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
		//StartCoroutine (screenFadeScript.doColorFade (Color.black));
		MainLight.SetActive (true);
		MainLight.transform.rotation = Quaternion.Euler (386f, 71f, 126f);
		//biome.resetBiomes ();

		Vec3Int chunkCoords = vxe.getChunkCoords (watchTrans.position);
//		Debug.LogError ("Chunk Coords " + chunkCoords.x + " " + chunkCoords.y + " " + chunkCoords.z);
		biome.swapMaterialsThread (ref voxelMats, chunkCoords.x, chunkCoords.z);
	}

	/// <summary>
	/// Finishs the gaze.
	/// </summary>
	void FinishGaze ()
	{
		TheSheepDog.SetActive (true);
		ItemSpawner.SetActive (true);
		EnvironmentSpawner.SetActive (true);
		sheepScript.DeActivate ();
		tutorialPhase = TutorialPhase.Finish;
	}
}
