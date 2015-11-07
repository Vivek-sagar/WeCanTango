using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour
{
	
	GameObject obj;
	VoxelExtractionPointCloud vxe;
	Vector3 onVoxelDown, vxCoord, normal;
	bool voxelBelow, stuckInVoxel;
	Transform myTrans;
    float minDistSqr;

	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		obj = this.gameObject;
		myTrans = obj.transform;
		onVoxelDown = Vector3.down * vxe.voxel_size;
		vxCoord = Vector3.zero;
		normal = Vector3.zero;
		minDistSqr = 10 * vxe.voxel_size;
		minDistSqr = minDistSqr * minDistSqr;
	}
	/*
	void FixedUpdate ()
	{

		float distsqr = (transform.position - vxe.camera.transform.position).sqrMagnitude;

		if (distsqr < minDistSqr) 
		{
			voxelBelow = vxe.isVoxelThere (myTrans.position + Vector3.down * 0.5f * vxe.voxel_size);
			stuckInVoxel = vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size);

			if (stuckInVoxel) {
				myTrans.position += Vector3.up * vxe.voxel_size;
			} else if (!voxelBelow) {
				myTrans.position -= Vector3.up * vxe.voxel_size;
			}
		}
	}*/

}
