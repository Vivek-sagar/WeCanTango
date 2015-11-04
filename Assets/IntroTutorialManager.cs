using UnityEngine;
using System.Collections;

public class IntroTutorialManager : MonoBehaviour
{

	public enum TutorialPhase
	{
		Wait,
		PocketWatchSwing,
		SheepPopOut,
		SheepGaze,
		OpenPortal,
		Finish,
	};
	public TutorialGaze playerGazeScript;
	public TutorialSheep sheepScript;
	public GameObject pocketWatch, sheepHolder, gazeTutorialGameObject;
	public Transform[] gazeTargets; //Need the places the sheep moves to for the player to gaze
	TutorialPhase tutorialPhase = TutorialPhase.Wait;
	Animator myAnim;
	int gazeCount = 0;
	// Use this for initialization
	void Start ()
	{
		tutorialPhase = TutorialPhase.PocketWatchSwing;
		sheepScript.ChangeTarget (gazeTargets [gazeCount]);
		pocketWatch.SetActive (false);
		sheepHolder.SetActive (false);
		gazeTutorialGameObject.SetActive (false);

		myAnim = GetComponent<Animator> ();
		myAnim.enabled = pocketWatch.activeSelf;
		playerGazeScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<TutorialGaze> ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		switch (tutorialPhase) {
		case TutorialPhase.Wait:	
			break;
		case TutorialPhase.PocketWatchSwing:
			break;
		case TutorialPhase.SheepGaze:			
			WaitForGaze ();
			break;
		}

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

				if (gazeCount < 3)
					sheepScript.ChangeTarget (gazeTargets [gazeCount]);
			}
		}
	}
}
