using UnityEngine;
using System.Collections;


[System.Serializable]
public struct Spawns
{
	public GameObject[] obj;
	public BIOMES biome;
	public bool[] sticksToWalls;
}

public class Spawner : MonoBehaviour
{
	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	public GameObject portal;
	public Spawns[] spawns;
	public Camera camera;
	int count = 0;
	int framecount = 0;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		//for (int i=0; i<spawnObjects.Length; i++) 
		//	spawnObjects[i].SetActive (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		framecount++;
		if (framecount % 120 != 0 || count > 20)
			return;

		Random.seed = (int)(Time.deltaTime * 1000);

		//A Random Point in your Camera View Port
		Vector3 ranPt = new Vector3 (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), camera.nearClipPlane); 
		//The World Point you see thru camera
		Vector3 startpt = camera.ViewportToWorldPoint (ranPt);
		//The Direction vector from Camera to start point
		Vector3 dir = startpt - camera.transform.position;
		Vector3 pos = new Vector3 (), normal = new Vector3 ();

		if (vxe.RayCast (startpt, dir, 64, ref pos, ref normal)) {

			Vec3Int chunkcoord = vxe.ToGrid (pos) / vxe.chunk_size;
			BIOMES mybiome = biome.biomeMap [chunkcoord.x, chunkcoord.z];

			int animal = Random.Range (0, spawns [(int)mybiome].obj.Length);
			GameObject spawnObject = spawns [(int)mybiome].obj [animal];

			if (spawns [(int)mybiome].sticksToWalls [animal] || Vector3.Dot (normal, Vector3.up) > 0.999f) {
				GameObject newsphere = (GameObject)Instantiate (spawnObject, pos + normal * VoxelExtractionPointCloud.Instance.voxel_size * 0.5f, Quaternion.identity);
				newsphere.SetActive (true);
				SimpleAI ai = newsphere.GetComponent<SimpleAI> ();

				//newsphere.GetComponent<GrowScript>().init(pos, normal, (Vector3.Dot (normal,Vector3.up) > 0.999f) );
				count++;
			}
		}

	}


	bool chunkSurfaceUp (Vec3Int chunkPos, int countThreshold)
	{
		//voxel Volume = 512
		if (vxe.grid.isOccupied (chunkPos))
			return false;

		int count = 0;

		Vector3 chunkWorld = vxe.FromGrid (chunkPos);
		Vector3 upSum3 = Vector3.up * vxe.voxel_size * 2;
		//use Chunk coord
		Vector3 vxCoord = Vector3.zero, normal = Vector3.zero;
		//Returns a chunk vxe.grid.voxelGrid[,,]
		//if (chunk.voxels [x, y, z].isOccupied ())
		//	chunk.getVoxel (new Vec3Int (x, y, z));
		
		for (int x=0; x<vxe.chunk_size; x++) {
			for (int y=0; y<vxe.chunk_size; y++) {
				for (int z=0; z<vxe.chunk_size; z++) {
				
					//Need the voxels world position
					//Raycast above each Voxel world position
					Vector3 voxelInWorld = vxe.FromGrid (new Vec3Int (x, y, z));

					bool hit = vxe.RayCast ((chunkWorld) + upSum3, (chunkWorld + upSum3) - voxelInWorld, 2f, ref vxCoord, ref normal);

					if (hit && normal.Equals (Vector3.up)) {
						count++;
						Debug.Log (string.Format ("Voxel @ {0}, {1}, {2} normal up", x, y, z));
					}
					if (count >= countThreshold)
						return true;
				}
			}
		}
		return false;
	}


	void dropInPortal (DIR normalDir, Vector3 chunkCoord, float threshold =0.6f)
	{
		Chunks chunk = vxe.getChunkFromPt (chunkCoord);
		vxe.isChunkASurface (normalDir, chunk, threshold);

	}
}
