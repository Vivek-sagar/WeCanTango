using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour
{
	
	GameObject obj;
	VoxelExtractionPointCloud vxe;
	Vector3 onVoxelDown, vxCoord, normal;
	bool voxelBelow, stuckInVoxel;
	Transform myTrans;

	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		obj = this.gameObject;
		myTrans = obj.transform;
		onVoxelDown = Vector3.down * vxe.voxel_size;
		vxCoord = Vector3.zero;
		normal = Vector3.zero;
	}

	void Update ()
	{
		voxelBelow = vxe.RayCast (myTrans.position + Vector3.up * vxe.voxel_size, Vector3.down, 1f, ref vxCoord, ref normal, 0.2f);
		stuckInVoxel = vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size);

		if (stuckInVoxel) {
			myTrans.position += Vector3.up * vxe.voxel_size;
		} else if (!voxelBelow) {
			myTrans.position -= Vector3.up * vxe.voxel_size;
		}

	}

}
