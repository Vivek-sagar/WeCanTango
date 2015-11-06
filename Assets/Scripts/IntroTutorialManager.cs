using UnityEngine;
using System.Collections;

public class IntroTutorialManager : MonoBehaviour
{

	public enum TutorialPhase
	{
		Wait,
		PocketWatchSwing,
		PreGaze,
		SheepGaze,
		DoneSwing,
		OpenPortal,
		Finish,
	};
	public TutorialPhase tutorialPhase;
	public TutorialGaze playerGazeScript;
	public TutorialSheep sheepScript;
	public PokeDector pokeScript;
	public GameObject pocketWatch, tutorialSheep, gazeTutorialGameObjects;
	public Transform[] gazeTargets; //Need the places the sheep moves to for the player to gaze
	int gazeCount = 0;
	public Animator myAnim;
	public AudioSource auSource;
	public GameObject player;
	public Vector3 lightON;
	public Transform mainLightTrans;
	public GameObject textObj;
	// Use this for initialization
	void Start ()
	{
		this.transform.position = new Vector3 (0, player.transform.position.y, 3f);
		tutorialPhase = TutorialPhase.Wait;
		playerGazeScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<TutorialGaze> ();
		SetMeshRenderersInChildren (tutorialSheep, false);
		SetMeshRenderersInChildren (gazeTutorialGameObjects, false);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (tutorialPhase == TutorialPhase.Wait && pokeScript.triggered) {
			tutorialPhase = TutorialPhase.PocketWatchSwing;
		} else if (tutorialPhase == TutorialPhase.PocketWatchSwing) {
			// Debug.LogError("AT PocketWatchSwing !!!");
			//Disable PocketWatch SetMeshRenderersInChildren (pocketWatch, false);
			myAnim.SetTrigger ("OpenPocketWatch");
			textObj.SetActive (false);

			tutorialPhase = TutorialPhase.PreGaze;
		} else if (tutorialPhase == TutorialPhase.DoneSwing) {
			//After Pocket Watch Swing is done, allow the TutorialSheep and TutorialGaze 
			//script to start doing stuff
			sheepScript.waitForAnimationEnd = false;
			playerGazeScript.waitForAnimationEnd = false;
			SetMeshRenderersInChildren (gazeTutorialGameObjects, true);

			//Set the Sheep's Gaze Target
			sheepScript.ChangeTarget (gazeTargets [gazeCount]);
			mainLightTrans.rotation = Quaternion.Euler (lightON);
			tutorialPhase = TutorialPhase.SheepGaze;
		} else if (tutorialPhase == TutorialPhase.SheepGaze) {
			WaitForGaze ();
		} else if (tutorialPhase == TutorialPhase.Finish) {	
			return;
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
		SetMeshRenderersInChildren (tutorialSheep, true);
		auSource.Stop ();
		tutorialPhase = TutorialPhase.DoneSwing;
	}

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

				//if (gazeCount < 3)
				sheepScript.ChangeTarget (gazeTargets [gazeCount]);
			}
		}
	}
}
