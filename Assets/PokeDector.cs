using UnityEngine;
using System.Collections;

public class PokeDector : MonoBehaviour
{

	public bool triggered = false;
	public BoxCollider cubeswitch;
	private bool isSleeping = true;
	public Transform playerTrans;
	VoxelExtractionPointCloud vxe;
	Transform myTrans;

	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		vxe = VoxelExtractionPointCloud.Instance;
	}
	
	/// <summary>
	/// Checks for voxels in collider.
	/// </summary>
	/// <returns><c>true</c>, if for voxels in collider was checked, <c>false</c> otherwise.</returns>
	bool checkForVoxelsInCollider ()
	{
		Vector3 max = cubeswitch.gameObject.transform.position + cubeswitch.bounds.extents;
		Vector3 min = cubeswitch.gameObject.transform.position - cubeswitch.bounds.extents;
		
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

		if (!triggered && cubeswitch.gameObject.activeSelf && checkForVoxelsInCollider ()) {
			triggered = true;
		}
		
		if (vxe.isVoxelThere (myTrans.position)) {
			transform.position += Vector3.up * vxe.voxel_size;
		}
	}
}
