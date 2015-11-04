using UnityEngine;
using System.Collections;

public class IntroTutorialManager : MonoBehaviour
{

	public enum TutorialPhase
	{
		Wait,
		PocketWatchSwing,
		SheepPopOut,
		SheepGazeLeft,
		SheepGazeRight,
		SheepGazeDown,
		OpenPortal,
		Finish,
	};
	public Transform[] gazeTargets; //Need the places the sheep moves to for the player to gaze
	public TutorialGaze gazeScript;
	public TutorialSheep sheepScript;
	TutorialPhase tutorialPhase = TutorialPhase.Wait;
	int gazeCount = 0;
	// Use this for initialization
	void Start ()
	{
		tutorialPhase = TutorialPhase.SheepGazeLeft;
		sheepScript.ChangeTarget (gazeTargets [gazeCount]);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		WaitForGaze ();

	}

	public void WaitForGaze ()
	{
		gazeScript.runGaze = sheepScript.atGazeTarget;

		//If the sheep is not at the target, then do Nothing
		if (!sheepScript.atGazeTarget) {
			return;
		} 
		//If the sheep is at the target, check to see if the Player Gazed at it
		else if (sheepScript.atGazeTarget) {
			if (gazeScript.gotHit) {
				gazeCount++;

				if (gazeCount < 3)
					sheepScript.ChangeTarget (gazeTargets [gazeCount]);
			}
		}
	}
}
