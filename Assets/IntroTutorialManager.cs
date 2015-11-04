using UnityEngine;
using System.Collections;

public class IntroTutorialManager : MonoBehaviour
{

	public enum TutorialPhase
	{
		Wait,
		PocketWatchSwing,
		SheepGaze,
		OpenPortal,
		Finish,
	};
	TutorialPhase tutorialPhase = TutorialPhase.Wait;
	public TutorialGaze playerGazeScript;
	public TutorialSheep sheepScript;
	public GameObject pocketWatch, tutorialSheep, gazeTutorialGameObjects;
	public Transform[] gazeTargets; //Need the places the sheep moves to for the player to gaze

	Animator myAnim;
	int gazeCount = 0;
	// Use this for initialization
	void Start ()
	{
		myAnim = GetComponent<Animator> ();
	
		playerGazeScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<TutorialGaze> ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (gazeCount > 2)
			tutorialPhase = TutorialPhase.OpenPortal;
		  
		switch (tutorialPhase) {
		case TutorialPhase.Wait:	
			break;
		case TutorialPhase.PocketWatchSwing:

			//Disable PocketWatch
			pocketWatch.SetActive (false);

			//After Pocket Watch Swing is done, allow the TutorialSheep and TutorialGaze 
			//script to start doing stuff
			sheepScript.waitForAnimationEnd = false;
			sheepScript.ChangeTarget (gazeTargets [gazeCount]);
			playerGazeScript.waitForAnimationEnd = false;
			gazeTutorialGameObjects.SetActive (true);
			tutorialPhase = TutorialPhase.SheepGaze;
			break;
		case TutorialPhase.SheepGaze:			
			WaitForGaze ();
			break;
		case TutorialPhase.OpenPortal:
			tutorialPhase = TutorialPhase.Finish;
			break;
		case TutorialPhase.Finish:
			break;
		}

	}

	public void SetTutorialPhase (TutorialPhase phase)
	{
		tutorialPhase = phase;
	}

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
