using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour
{

	public bool triggered = false;
	public bool debug = false;
	//public bool init = false;
	public ParticleSystem partsys;
	public GameObject littleSheep;
	private bool isSleeping = true;


	Vector3[] fourpts;
	VoxelExtractionPointCloud vxe;

	CameraClearFlags defaultFlag;
	float defaultLightIntensity;
	BoxCollider mycollider;
	// Use this for initialization
	void Awake()
	{
		mycollider = GetComponent<BoxCollider> ();
	}

	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		partsys.Stop ();
		//cubeswitch.gameObject.SetActive (false);
	}


	bool checkForVoxelsInCollider ()
	{
		Vector3 max = mycollider.bounds.center + mycollider.bounds.extents;
		Vector3 min = mycollider.bounds.center - mycollider.bounds.extents;

		for (float i=min.x; i<=max.x; i+= vxe.voxel_size)
			for (float j=min.y; j<=max.y; j+= vxe.voxel_size)
				for (float k=min.z; k<=max.z; k+= vxe.voxel_size) {
					if (vxe.isVoxelThere (new Vector3 (i, j, k)))
						return true;
				}
		
		return false;
	}


	// Update is called once per frame
	void Update ()
	{
		if (!isSleeping) 
		{
			if (!triggered && checkForVoxelsInCollider ()) {
				triggeredEvent ();
			}
		}

		if (isSleeping && 
		    	(vxe.isVoxelThere (littleSheep.transform.position) || checkForVoxelsInCollider()) 
		    )
		{
			littleSheep.transform.position += Vector3.up * vxe.voxel_size;
			transform.position += Vector3.up * vxe.voxel_size;
		}
		
	}

	void OnTriggerEnter (Collider other)
	{
		if (triggered)
			return;
		GameObject othergo = other.gameObject;

		if (othergo.tag == "Pet") {
			//triggeredEvent();
			partsys.Emit (100);

			if (isSleeping) {
				isSleeping = false;
				partsys.Play();
			}
		}
	}

	void triggeredEvent ()
	{
		partsys.startLifetime = 3;
		partsys.startColor = Color.cyan;
		partsys.startSpeed = 2.0f;
		partsys.startSize = 0.3f;
		partsys.maxParticles = 500;
		partsys.Clear ();
		partsys.Stop ();
		partsys.Emit (500);

		littleSheep.GetComponent<JumpingAI> ().init ();


		PetManager.Instance.setThankYou ();

		triggered = true;
		//petcounter.PetTriggered ();

		Vec3Int cc = vxe.getChunkCoords (transform.position);

		BiomeScript.Instance.doRandomChange (cc.x, cc.z);
		ItemSpawner.Instance.canSpawn = true;
	}

}
