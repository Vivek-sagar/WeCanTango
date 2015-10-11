using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public struct BiomeSpawn
{ 
	//public GameObject[] obj;
	//Make it so the BIOMES do not have to be in number order to be set
	public BIOMES biome;
	public SpawnStuff[] SpawnList;
	//public bool[] sticksToWalls;
}

[System.Serializable]
public struct SpawnStuff
{
	public GameObject gameObject;
	//public float spawnPercent;
	public bool sticksToWalls;
}

public class Spawner : MonoBehaviour
{
	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	public GameObject portal;
	public BiomeSpawn[] currentBioSet, portalBioSet;
	public Camera backCam;
	public Transform playerTrans;
	int count = 0;
	int framecount = 0;
    List<GameObject> spawnList;

	public int spawnMax;
	bool isSpawningPortal;
	Dictionary<BIOMES,BiomeSpawn> spawnTable = new Dictionary<BIOMES, BiomeSpawn> ();

	static Vector3[] directions = {
		new Vector3 (0, 0, 0),
		new Vector3 (0, 0, 1),
		new Vector3 (0, 0, -1),
		new Vector3 (1, 0, 0),
		new Vector3 (-1, 0, 0),
		new Vector3 (0, 1, 0),
	};

	// Use this for initialization
	void Start ()
	{
		/*foreach (BiomeSet bm in currentBioSet) {
			BiomeTable.Add (bm.biome, bm);
		}*/
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
        spawnList = new List<GameObject>();

        if (playerTrans == null)
			playerTrans = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		//Debug Test for whether to use ActiveInHiearchy vs ActiveSelf
		//Debug.Log (string.Format ("Portal Active In Hiearchay {0} , Portal Active Self {1} ", portal.activeInHierarchy, portal.activeSelf));

		//for (int i=0; i<spawnObjects.Length; i++) 
		//	spawnObjects[i].SetActive (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		framecount++;


		if (framecount % 60 != 0 || (count > 20))
			return;

		Random.seed = (int)(Time.deltaTime * 1000);

		//A Random Point in your Camera View Port
		Vector3 ranPt = new Vector3 (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), backCam.nearClipPlane); 
		//The World Point you see thru camera
		Vector3 startpt = backCam.ViewportToWorldPoint (ranPt);
		//The Direction vector from Camera to start point
		Vector3 dir = startpt - backCam.transform.position;
		Vector3 pos = new Vector3 (), normal = new Vector3 ();

		if (vxe.RayCast (startpt, dir, 64, ref pos, ref normal)) {

			//If the portal is not active, try spawning one
			if (!portal.activeInHierarchy) {
				spawnPortal (pos, 0.6f);
			}

			//Convert the voxel position into a Chunk World Position
			Vec3Int chunkcoord = vxe.ToGrid (pos) / vxe.chunk_size;

			//Loads up the BiomeMap through the grid size
			BIOMES mybiome = biome.biomeMap [chunkcoord.x, chunkcoord.z];

			//randomly chooses an animal to spawn
			int animalIndex = Random.Range (0, spawnTable [mybiome].SpawnList.Length);
			GameObject spawnObject = spawnTable [mybiome].SpawnList [animalIndex].gameObject;

			//
			if (spawnTable [mybiome].SpawnList [animalIndex].sticksToWalls || Vector3.Dot (normal, Vector3.up) > 0.999f) {
				GameObject newsphere = (GameObject)Instantiate (spawnObject, pos + normal * VoxelExtractionPointCloud.Instance.voxel_size * 0.5f, Quaternion.identity);
				newsphere.SetActive (true);
				SimpleAI ai = newsphere.GetComponent<SimpleAI> ();
                spawnList.Add(newsphere);
                //newsphere.GetComponent<GrowScript>().init(pos, normal, (Vector3.Dot (normal,Vector3.up) > 0.999f) );
                count++;
			}		
		}

	}

/// <summary>
/// Drops in the portal if there is a surface around the chunk World Coordinate.
/// </summary>
/// <param name="normalDir">Normal direction.</param>
/// <param name="chunkCoord">Chunk coordinate.</param>
/// <param name="threshold">Threshold.</param>
	void spawnPortal (Vector3 chunkCoord, float threshold =0.6f, bool forceSpawn=false)
	{
		DIR normalDir = DIR.DIR_UP;
		//Chunks chunkCenter = vxe.getChunkFromPt (chunkCoord);
		//Vec3Int chunkVxCoord = vxe.getChunkCoords (chunkCoord);

		int surfaceCount = 0;
		bool[] isChunkSurface = new bool[directions.Length];
		Chunks chunk; 
		for (int i=0; i<isChunkSurface.Length; i++) {
			//chunksAround [i] = vxe.getChunkFromPt (chunkVxCoord + );

			chunk = vxe.getChunkFromPt (chunkCoord + directions [i] * 0.1f);
			isChunkSurface [i] = vxe.isChunkASurface (normalDir, chunk, threshold);
			if (isChunkSurface [i])
				surfaceCount ++;
		}	

		if (surfaceCount > 5 || forceSpawn) {
			portal.transform.position = chunkCoord;
			portal.SetActive (true);
		}

		//vxe.isChunkASurface (normalDir, chunkCenter, threshold);

	}

	/// <summary>
	/// Swaps the biome sets.
	/// </summary>
	public void SwapBiomeSets ()
	{
        BiomeSpawn[] tempSet = currentBioSet;
		currentBioSet = portalBioSet;
		portalBioSet = tempSet;
		spawnTable.Clear ();		
		for (int i =0; i<currentBioSet.Length; i++) {
			spawnTable.Add (currentBioSet [i].biome, currentBioSet [i]);
		}
        DestroySpawns();
        
	}

    void DestroySpawns()
    {
        GameObject temp;
        for(int i=spawnList.Count - 1; i > -1; i--)
        {
            temp = spawnList[i];
            Destroy(temp);
        }
        spawnList.Clear();

    }
	/*void spawnPortal (Vec3Int chunkVxCoord, float threshold =0.6f)
	{
		Vector3 chunkWorldCoord = vxe.getChunkFromPt (chunkVxCoord);
		
		//vxe.isChunkASurface (normalDir, chunkCenter, threshold);
		
	}*/

	/*bool chunkSurfaceUp (Vec3Int chunkPos, int countThreshold)
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
	}*/
}
