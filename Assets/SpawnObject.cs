using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour
{
	
	GameObject obj;
	VoxelExtractionPointCloud vxe;

	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		obj = this.gameObject;
	}

	void Update ()
	{
		if (vxe.isVoxelThere (obj.transform.position)) {
			transform.position += Vector3.up * vxe.voxel_size;
		} 
		//Disable the script when it is no longer in the Voxel???
		/*else
			this.enabled = false;*/
	}

}
