using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public struct BiomeSpawn
{ 
	public BIOMES biome;
	public SpawnStuff[] SpawnList;
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
	public BiomeSpawn[] currentBiomeSpawn, portalBiomeSpawn;
	public Camera backCam;
	public Transform playerTrans;
    
	int framecount = 0;
	int maxToSpawnInBiome = 4;
	//List of GameObjects Spawned
	List<GameObject> thingsSpawned;
	//Keeps track of spawnCount in each biome, so 1 biome does not overflow with spawns
	int[] spawnCountInBoime = { 0, 0, 0, 0 };
	//Make it so the BIOMES do not have to be in number order to be set
	Dictionary<BIOMES,BiomeSpawn> spawnTable = new Dictionary<BIOMES, BiomeSpawn> ();
	Vector3 chunkFloorPos = Vector3.zero;

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
		StartCoroutine (findFloor ());
		foreach (BiomeSpawn bm in currentBiomeSpawn) {
			spawnTable.Add (bm.biome, bm);
		}
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		thingsSpawned = new List<GameObject> ();

		if (playerTrans == null)
			playerTrans = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		//Debug Test for whether to use ActiveInHiearchy vs ActiveSelf
		//Debug.Log (string.Format ("Portal Active In Hiearchay {0} , Portal Active Self {1} ", portal.activeInHierarchy, portal.activeSelf));

		
	}

	// Update is called once per frame
	void Update ()
	{
		framecount++;

		//
		if ((framecount % 10 != 0 || (thingsSpawned.Count > maxToSpawnInBiome * 4)) && portal.activeInHierarchy)
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

			//SPAWN PORTAL:********************************************************************* 
			//If the portal is not active, try spawning one
			if (!portal.activeInHierarchy) {
				spawnPortal (pos, 0.6f, chunkFloorPos.y);
			}

			///SPAWNING FOR ANIMALS*********************************************************************
			//Convert the voxel position into a Chunk World Position
			Vec3Int chunkcoord = vxe.ToGrid (pos) / vxe.chunk_size;

			//Loads up the BiomeMap through the grid size
			BIOMES mybiome = biome.biomeMap [chunkcoord.x, chunkcoord.z];

			//If there are too many spawns in this Biome, do not spawn more
			if (spawnCountInBoime [(int)mybiome] > maxToSpawnInBiome)
				return;

			//randomly chooses an animal to spawn
			int animalIndex = Random.Range (0, spawnTable [mybiome].SpawnList.Length);
			GameObject spawnObject = spawnTable [mybiome].SpawnList [animalIndex].gameObject;

			Chunks chunk = vxe.getChunkFromPt (pos);
			bool isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, .65f);

			if (spawnTable [mybiome].SpawnList [animalIndex].sticksToWalls || isSurface) { // Vector3.Dot (normal, Vector3.up) > 0.999f) {
				GameObject newsphere = (GameObject)Instantiate (spawnObject, pos + normal * VoxelExtractionPointCloud.Instance.voxel_size * 0.5f, Quaternion.identity);
				newsphere.SetActive (true);
				//SimpleAI ai = newsphere.GetComponent<SimpleAI> ();
				thingsSpawned.Add (newsphere);
				//newsphere.GetComponent<GrowScript>().init(pos, normal, (Vector3.Dot (normal,Vector3.up) > 0.999f) );
                
				spawnCountInBoime [(int)mybiome]++;
			}		
		}

	}

	/// <summary>
	/// Raycasts to the ground to find the floor
	/// </summary>
	/// <returns></returns>
	IEnumerator findFloor ()
	{
		//The Direction vector from Camera to start point
		Vector3 pos = new Vector3 (), normal = new Vector3 ();
		bool foundFloor = false;

		while (!foundFloor) {
			foundFloor = (vxe.RayCast (playerTrans.position, Vector3.down, 64, ref pos, ref normal));
			yield return new WaitForSeconds (1f);
		}
		chunkFloorPos = pos;
		yield return null;
	}

	/// <summary>
	/// Drops in the portal if there is a surface around the chunk World Coordinate.
	/// </summary>
	/// <param name="normalDir">Normal direction.</param>
	/// <param name="chunkCoord">Chunk coordinate.</param>
	/// <param name="threshold">Threshold.</param>
	void spawnPortal (Vector3 chunkCoord, float threshold =0.6f, float floorHieght=0f)
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


		if (surfaceCount > 5) {
			Vector3 pos = new Vector3 (chunkCoord.x, floorHieght, chunkCoord.z);
			portal.transform.position = chunkCoord;
			portal.SetActive (true);
		}
		//vxe.isChunkASurface (normalDir, chunkCenter, threshold);

	}

	/// <summary>
	/// Drops in the portal if there is a surface around the chunk World Coordinate.
	/// </summary>
	/// <param name="normalDir">Normal direction.</param>
	/// <param name="chunkCoord">Chunk coordinate.</param>
	/// <param name="threshold">Threshold.</param>
	IEnumerator spawnPortalThread (Vector3 chunkCoord, float threshold = 0.6f, bool forceSpawn = false)
	{
		DIR normalDir = DIR.DIR_UP;
		//Chunks chunkCenter = vxe.getChunkFromPt (chunkCoord);
		//Vec3Int chunkVxCoord = vxe.getChunkCoords (chunkCoord);

		int surfaceCount = 0;
		bool[] isChunkSurface = new bool[directions.Length];
		Chunks chunk;
		for (int i = 0; i < isChunkSurface.Length; i++) {
			//chunksAround [i] = vxe.getChunkFromPt (chunkVxCoord + );
			chunk = vxe.getChunkFromPt (chunkCoord + directions [i] * 0.1f);
			isChunkSurface [i] = vxe.isChunkASurface (normalDir, chunk, threshold);
			if (isChunkSurface [i])
				surfaceCount++;
			yield return new WaitForSeconds (0.5f);
		}

		if (surfaceCount > 5 || forceSpawn) {
			portal.transform.position = chunkCoord;
			portal.SetActive (true);
		}
		//vxe.isChunkASurface (normalDir, chunkCenter, threshold);

	}

	/// <summary>
	/// Swaps the biome sets as well as Destroys the GameObjects spawns (resets any counts)
	/// </summary>
	public void SwapBiomeSets ()
	{
		BiomeSpawn[] tempSet = currentBiomeSpawn;
		currentBiomeSpawn = portalBiomeSpawn;
		portalBiomeSpawn = tempSet;
		spawnTable.Clear ();		
		for (int i =0; i<currentBiomeSpawn.Length; i++) {
			spawnTable.Add (currentBiomeSpawn [i].biome, currentBiomeSpawn [i]);
			spawnCountInBoime [i] = 0;
		}
		DestroySpawns ();
      
	}

	void DestroySpawns ()
	{
		GameObject temp;
		for (int i=thingsSpawned.Count - 1; i > -1; i--) {
			temp = thingsSpawned [i];
			Destroy (temp);
		}
		thingsSpawned.Clear ();

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
