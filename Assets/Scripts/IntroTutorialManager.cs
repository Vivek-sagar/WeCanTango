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
		OpenPortal,
		Finish,
	};
	public TutorialPhase tutorialPhase;
	public TutorialGaze playerGazeScript;
	public TutorialSheep sheepScript;
	public GameObject pocketWatch, tutorialSheep, gazeTutorialGameObjects;
	public Transform[] gazeTargets; //Need the places the sheep moves to for the player to gaze
	int gazeCount = 0;
	// Use this for initialization
	void Start ()
	{
		tutorialPhase = TutorialPhase.Wait;
		playerGazeScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<TutorialGaze> ();
        SetMeshRenderersInChildren(tutorialSheep, false);
        SetMeshRenderersInChildren(gazeTutorialGameObjects, false);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (gazeCount > 2)
			tutorialPhase = TutorialPhase.OpenPortal;
		  
	
		if(tutorialPhase== TutorialPhase.Wait){
            return;
        }
		else if(tutorialPhase==  TutorialPhase.PocketWatchSwing){

            //Disable PocketWatch
            SetMeshRenderersInChildren(pocketWatch, false);

            //After Pocket Watch Swing is done, allow the TutorialSheep and TutorialGaze 
            //script to start doing stuff
            sheepScript.waitForAnimationEnd = false;
            playerGazeScript.waitForAnimationEnd = false;

            //Set the Sheep's Gaze Target
            sheepScript.ChangeTarget(gazeTargets[gazeCount]);
           
            tutorialPhase = TutorialPhase.SheepGaze;
        }
		else if(tutorialPhase== TutorialPhase.SheepGaze){
            WaitForGaze ();
        }			
		else if(tutorialPhase== TutorialPhase.OpenPortal){	
            tutorialPhase = TutorialPhase.Finish;
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


    /// <summary>
    /// BLASAEOILAHEAEU Unity wont let me use any public function for animation events ~~~!!
    /// 
    /// </summary>
    public void SetSheepOn()
    {
        SetMeshRenderersInChildren(tutorialSheep, true);
    }

    public void SetMeshRenderersInChildren(GameObject parent, bool state)
    {
        MeshRenderer[] renders = parent.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer child in renders)
        {
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
