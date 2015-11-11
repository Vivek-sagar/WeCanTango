using UnityEngine;
using System.Collections;

public class PokeDector : MonoBehaviour
{
	//TRIGGERED MUST START FALSE
	public bool triggered = false, waitForNoVoxel;
	public BoxCollider cubeswitch;
	public AudioSource audio;
	VoxelExtractionPointCloud vxe;
	Transform myTrans, cubeTrans;
	float frames = 0f, maxFrames = 60f;
	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		cubeTrans = cubeswitch.gameObject.transform;
		vxe = VoxelExtractionPointCloud.Instance;
	}
	
	/// <summary>
	/// Checks for voxels in collider.
	/// </summary>
	/// <returns><c>true</c>, if for voxels in collider was checked, <c>false</c> otherwise.</returns>
	bool checkForVoxelsInCollider ()
	{
		Vector3 max = cubeswitch.bounds.center + cubeswitch.bounds.extents;
		Vector3 min = cubeswitch.bounds.center - cubeswitch.bounds.extents;
		
		for (float i=min.x; i<=max.x; i+= vxe.voxel_size)
			for (float j=min.y; j<=max.y; j+= vxe.voxel_size)
				for (float k=min.z; k<=max.z; k+= vxe.voxel_size) {
					if (vxe.isVoxelThere (new Vector3 (i, j, k))) {
						if (audio != null)	
							audio.Play ();
						return true;
					}
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

		if (waitForNoVoxel) {
			frames ++;
			bool isVoxel = checkForVoxelsInCollider ();

			if (!isVoxel && frames > maxFrames) {
				triggered = vxe.occupiedChunks.getCount () > 20f;
				frames = 0;
			} else if (isVoxel) {
				transform.position += Vector3.up * vxe.voxel_size;
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
#if UNITY_EDITOR
		/*if (other.CompareTag ("PlayerCollider")) {
			triggered = true;
		}*/
#endif
	}
}
