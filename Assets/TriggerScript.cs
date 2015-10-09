using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour
{
	BiomeScript biome;
	public Material tranformMat;
	bool triggered = false;
	public Camera leftcam;
	public Camera rightcam;
	public Light light;
	public ParticleSystem partsys;
	public GameObject obj;
	public BoxCollider cubeswitch;
	Vector3[] fourpts;
	VoxelExtractionPointCloud vxe;

	CameraClearFlags defaultFlag;
	float defaultLightIntensity;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		defaultFlag = leftcam.clearFlags;
		defaultLightIntensity = light.intensity;
		fourpts = new Vector3[3];

		if (cubeswitch != null) {
			updatePts ();
		}
	}

	void updatePts ()
	{
		fourpts [0] = cubeswitch.gameObject.transform.position + cubeswitch.bounds.extents;
		fourpts [1] = cubeswitch.gameObject.transform.position - cubeswitch.bounds.extents;
		fourpts [2] = cubeswitch.gameObject.transform.position;
	}


	// Update is called once per frame
	void Update ()
	{
		updatePts ();
		//If this has not been triggered, has a cube switch and there is a voxel somewhere near it from positive to negative extents 
		if (!triggered && cubeswitch != null && 
			(vxe.isVoxelThere (fourpts [0]) || vxe.isVoxelThere (fourpts [1]) || vxe.isVoxelThere (fourpts [2]))
		   ) {
			triggeredEvent ();
		}
	}

	void OnTriggerEnter (Collider other)
	{
		//if (triggered)
		//	return;
		//GameObject othergo = other.gameObject;

		//if (othergo.tag == "Player") 
		//{
		//	triggeredEvent();
		//}
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
		obj.SetActive (false);
		if (cubeswitch != null)
			cubeswitch.gameObject.SetActive (false);
		
		StartCoroutine (worldTransform (true));
		triggered = true;
	}

	/// <summary>
	/// Transforms the world.
	/// </summary>
	public IEnumerator worldTransform (bool reset=false)
	{
		//Have parameter for BIOME instead of Material, 
		//so a whole Biome is set, and the old biome (animals, and environment included are destroyed)
		biome.setAllMaterials (tranformMat);
		leftcam.clearFlags = CameraClearFlags.Skybox;
		rightcam.clearFlags = CameraClearFlags.Skybox;
		light.intensity = 1.0f;
		if (reset) {
			yield return new WaitForSeconds (10.0f);
			leftcam.clearFlags = defaultFlag;
			rightcam.clearFlags = defaultFlag;
			light.intensity = defaultLightIntensity;
			biome.resetBiomes ();
		}
		yield return null;
	}

}
