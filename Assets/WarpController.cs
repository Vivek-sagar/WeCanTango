using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	public Vector2 offset; 
	public ScrollTexture[] scrollTextureList; 
	public bool portalColored;
	public Spawner spawner;

    public Camera leftcam;
    public Camera rightcam;
    bool teleporting;
	Animator myAnim;
	Color currentColor = Color.black, OnColor = new Color (0, 127f, 1f);
    BiomeScript biome;
    CameraClearFlags defaultFlag;

    // Use this for initialization
    void Start ()
	{
        defaultFlag = leftcam.clearFlags;
        biome = BiomeScript.Instance;
        myAnim = GetComponent<Animator> ();
        spawner.SwapBiomeSets();
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

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player") && !teleporting)
        {
            //myAnim.SetTrigger ("PortalOn");
            //teleporting = true;
            spawner.SwapBiomeSets();

            worldWarp();
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
		//myAnim.SetTrigger ("StopPortal");
		teleporting = false;
	}

    public void worldWarp(bool reset = false)
    {
        biome.swapMaterials();
        biome.resetBiomes();

        //Add visual effect stuff here later
        leftcam.clearFlags = CameraClearFlags.Skybox;
        rightcam.clearFlags = CameraClearFlags.Skybox;
    }

}
