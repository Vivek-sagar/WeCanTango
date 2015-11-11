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

	public void checkWeirdPosition ()
	{
		//Don't know yet why, but for some SpawnObjects, they have not had their Start function occur so vxe would be null
		if (vxe == null)
			vxe = VoxelExtractionPointCloud.Instance;
		float distsqr = (transform.position - vxe.camera.transform.position).sqrMagnitude;

		if (distsqr < minDistSqr) {
			//low quality check
			voxelBelow = vxe.isVoxelThere (myTrans.position + Vector3.down * 0.5f * vxe.voxel_size);
			//voxelBelow |= vxe.isVoxelThere (myTrans.position + Vector3.down * 0.5f * vxe.voxel_size + Vector3.right * 0.2f * vxe.voxel_size);
			//voxelBelow |= vxe.isVoxelThere (myTrans.position + Vector3.down * 0.5f * vxe.voxel_size + Vector3.left * 0.2f * vxe.voxel_size);
			//voxelBelow |= vxe.isVoxelThere (myTrans.position + Vector3.down * 0.5f * vxe.voxel_size + Vector3.forward * 0.2f * vxe.voxel_size);
			//voxelBelow |= vxe.isVoxelThere (myTrans.position + Vector3.down * 0.5f * vxe.voxel_size + Vector3.back * 0.2f * vxe.voxel_size);

			stuckInVoxel = vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size);
			//stuckInVoxel |= vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size + Vector3.right * 0.2f * vxe.voxel_size);
			//stuckInVoxel |= vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size + Vector3.left * 0.2f * vxe.voxel_size);
			//stuckInVoxel |= vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size + Vector3.forward * 0.2f * vxe.voxel_size);
			//stuckInVoxel |= vxe.isVoxelThere (myTrans.position + Vector3.up * 0.5f * vxe.voxel_size + Vector3.back * 0.2f * vxe.voxel_size);

			if (stuckInVoxel) {
				myTrans.position += Vector3.up * vxe.voxel_size;
			} else if (!voxelBelow) {
				myTrans.position -= Vector3.up * vxe.voxel_size;
			}
		}
	}

}
