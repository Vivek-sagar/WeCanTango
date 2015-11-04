using UnityEngine;
using System.Collections;

public class PetSpawner : MonoBehaviour {
	public GameObject pet;

	VoxelExtractionPointCloud vxe;
	public Camera camera;
	int framecount = 0;
	public int spawnInterval = 30;
	public bool spawned = false;

	void Start () {
		vxe = VoxelExtractionPointCloud.Instance;
		spawned = false;
	}
	
	// Update is called once per frame
	void Update () {
		framecount++;
		if(spawned || framecount % spawnInterval != 0)
			return;
		
		Vector3 pos = new Vector3 (), normal = new Vector3 ();
		
		if (vxe.RayCast (camera.transform.position, camera.transform.forward, 64, ref pos, ref normal)) {
			
			Vec3Int chunkcoord = vxe.ToGrid(pos) / vxe.chunk_size;

			if(Vector3.Dot (normal,Vector3.up) > 0.999f)
			{
				GameObject newsphere = (GameObject)Instantiate (pet, pos + normal * VoxelExtractionPointCloud.Instance.voxel_size * 0.5f, Quaternion.identity);
				newsphere.SetActive (true);
				spawned = true;
			}
		}
		
	}
}
