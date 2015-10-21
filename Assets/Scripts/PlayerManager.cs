using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
	Vector3[] directions = {
		new Vector3 (0, 0, 0),
		new Vector3 (-1, 0, 0),
		new Vector3 (1, 0, 0),
		new Vector3 (0, 0, -1),
		new Vector3 (0, 0, 1),
		new Vector3 (-1, 0, 1),
		new Vector3 (1, 0, 1),
		new Vector3 (-1, 0, -1),
		new Vector3 (1, 0, -1),
	};

	public int spawnInterval = 30;
	public List<GameObject> desertEnvironment;
	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	public Transform playerTrans;
	int framecount = 0;
	Chunks chunk;
	bool isSurface, isInTable;
	List<Chunks> occupiedNearMe;
	Vec3Int playerCC;
	int chunkx, chunkz;
	BIOMES mybiome;
	Dictionary<Vec3Int,GameObject> desertTable;
	bool isSpawning = false;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		occupiedNearMe = new List<Chunks> ();
		desertTable = new Dictionary<Vec3Int, GameObject> ();
		//desertEnvironment = new List<GameObject> ();
		if (playerTrans == null)
			playerTrans = GameObject.FindWithTag ("Player").GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		framecount++;
		if (framecount % spawnInterval != 0 || isSpawning) {
			return;
		}
		isSpawning = true;
		StartCoroutine (FullPullSpawn ());
		/*Player Chunk Coord
		 * */
	

		/*playerCC = vxe.getChunkCoords (playerTrans.position);
		chunkx = playerCC.x;
		chunkz = playerCC.z;
	
		mybiome = biome.biomeMap [chunkx, chunkz];
		if (mybiome != BIOMES.sand)
			return;

		OPTIMIZE THIS LATER, make it nlog(n) at least !!!!!!!!!!!!!!!
		 Mergesort
		 quicksort
		 COME ON MAN!!
		for (int i =0; i<directions.Length; i++) {
			//Gets the Chunk at my position
			chunk = vxe.getChunkFromPt (playerTrans.position + directions [i]);
			Vec3Int chunkVXCoords = vxe.getChunkCoords (playerTrans.position + directions [i]);
			if (chunk == null)
				continue;
			isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.55f);
			hasKey = desertTable.ContainsKey (chunkVXCoords);
			if (isSurface && !hasKey && !isSpawning) {
				//PullSpawnObject (playerTrans.position + directions [i], chunkVXCoords, chunk);
				StartCoroutine (PullSpawnObjectRoutine (playerTrans.position + directions [i], chunkVXCoords, chunk));
				break;
			}
		}*/
	}

	IEnumerator FullPullSpawn ()
	{
		
		playerCC = vxe.getChunkCoords (playerTrans.position);
		chunkx = playerCC.x;
		chunkz = playerCC.z;
		
		mybiome = biome.biomeMap [chunkx, chunkz];
		/*if (mybiome != BIOMES.sand)
			return;*/
		
		/*OPTIMIZE THIS LATER, make it nlog(n) at least !!!!!!!!!!!!!!!
		 Mergesort
		 quicksort
		 COME ON MAN!!*/
		Vector3 chunkBaseCoords;
		//LOOP through List of Environment Objects***********************************
		for (int i =directions.Length-1; i>-1 && desertEnvironment.Count > 0; i--) {

			//Gets the Chunk at my position
			chunk = vxe.getChunkFromPt (playerTrans.position + directions [i]);
			Vec3Int chunkVXCoords = vxe.getChunkCoords (playerTrans.position + directions [i]);

			//Chunks World Coords
			chunkBaseCoords = vxe.FromGrid (chunkVXCoords * vxe.chunk_size);
			if (chunk == null)
				continue;
			isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.55f);
			isInTable = desertTable.ContainsKey (chunkVXCoords);

			//If the chunk is a surface and is not in the HashTable, do Spawning code
			if (isSurface && !isInTable) {
				//PullSpawnObject (playerTrans.position + directions [i], chunkVXCoords, chunk);
				//Vector3 chunkWorldCoord = vxe.FromGrid (chunkVXCoords) / vxe.chunk_size;
				GameObject gbj = desertEnvironment [0];
				//LOOP thru each voxel in the chunk*******************************
				for (int x=0; x<vxe.chunk_size; x++)
					for (int z=0; z<vxe.chunk_size; z++) {
						for (int y=vxe.chunk_size-1; y>=0; y--) {
							Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
							//if vx is occ and has a sruface, spawn on top
							if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) {
								
								//Vector3 voxelCoords = chunkBaseCoords + new Vector3 (x, y, z) / vxe.voxel_size;
								Vector3 voxelCoords = chunkBaseCoords + new Vector3 (x, y, z) * vxe.voxel_size;//vxe.FromGrid (new Vec3Int (x, y, z));
								gbj.GetComponent<Transform> ().position = voxelCoords;
								gbj.SetActive (true);
								desertEnvironment.RemoveAt (0);
								desertTable.Add (chunkVXCoords, gbj);
								//Debug.Log (string.Format ("ChunkWorldCoord {4} VoxelGridCoord ({1} ,{2}, {3} VoxelWorldCoord {0}  )", voxelCoords, x, y, z, chunkWorldCoord));
								
								//Keep THIS here so less Loops each frame
								goto doneSpawn;
							}
						}
						yield return new WaitForEndOfFrame ();
					}

			}
		}
		doneSpawn:
		yield return new WaitForEndOfFrame ();
		isSpawning = false;

	}
	void PullSpawnObject (Vector3 chunkBaseCoords, Vec3Int chunkVXCoords, Chunks chunk)
	{

		//Vector3 chunkWorldCoord = vxe.FromGrid (chunkVXCoords) / vxe.chunk_size;
		if (desertEnvironment.Count > 0) {

			GameObject gbj = desertEnvironment [0];
			bool hasKey = desertTable.ContainsKey (chunkVXCoords);
			//Loop thru each voxel in the chunk
			for (int x=0; x<vxe.chunk_size; x++)
				for (int z=0; z<vxe.chunk_size; z++)
					for (int y=vxe.chunk_size-1; y>=0; y--) {
						Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
						//if vx is occ and has a sruface, spawn on top
						if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) {

							//Vector3 voxelCoords = chunkBaseCoords + new Vector3 (x, y, z) / vxe.voxel_size;

							//voxelCoords should be the voxels Local Coordinates
							Vector3 voxelCoords = chunkBaseCoords + new Vector3 (x, y, z) * vxe.voxel_size + Vector3.up * vxe.voxel_size;//vxe.FromGrid (new Vec3Int (x, y, z));
							gbj.GetComponent<Transform> ().position = voxelCoords;
							gbj.SetActive (true);
							desertEnvironment.RemoveAt (0);
							desertTable.Add (chunkVXCoords, gbj);
							//Debug.Log (string.Format ("ChunkWorldCoord {4} VoxelGridCoord ({1} ,{2}, {3} VoxelWorldCoord {0}  )", voxelCoords, x, y, z, chunkWorldCoord));
							return;
						}
						//Keep THIS here so less Loops each frame
						
					}
		}
	}

	IEnumerator PullSpawnObjectRoutine (Vector3 chunkBaseCoords, Vec3Int chunkVXCoords, Chunks chunk)
	{
		//Vector3 chunkWorldCoord = vxe.FromGrid (chunkVXCoords) / vxe.chunk_size;
		if (desertEnvironment.Count > 0) {
			GameObject gbj = desertEnvironment [0];
			bool hasKey = desertTable.ContainsKey (chunkVXCoords);
			//Loop thru each voxel in the chunk
			for (int x=0; x<vxe.chunk_size; x++)
				for (int z=0; z<vxe.chunk_size; z++) {
					for (int y=vxe.chunk_size-1; y>=0; y--) {
						Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
						//if vx is occ and has a sruface, spawn on top
						if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) {
						
							//Vector3 voxelCoords = chunkBaseCoords + new Vector3 (x, y, z) / vxe.voxel_size;
							Vector3 voxelCoords = chunkBaseCoords + new Vector3 (x, y, z) * vxe.voxel_size;//vxe.FromGrid (new Vec3Int (x, y, z));
							gbj.GetComponent<Transform> ().position = voxelCoords;
							gbj.SetActive (true);
							desertEnvironment.RemoveAt (0);
							desertTable.Add (chunkVXCoords, gbj);
							//Debug.Log (string.Format ("ChunkWorldCoord {4} VoxelGridCoord ({1} ,{2}, {3} VoxelWorldCoord {0}  )", voxelCoords, x, y, z, chunkWorldCoord));
							
							goto doneSpawn;
						}

						
						//Keep THIS here so less Loops each frame
					
					}
					yield return new WaitForEndOfFrame ();
				}
		}
		doneSpawn:
		yield return null;
	}

	void SpawnNearPlayer ()
	{
		int chunkx, chunkz;
		Chunks chunk;
		Vec3Int playerCC;
		bool isSurface;
		BIOMES mybiome;
		while (true) {
			//Player Chunk Coord
			playerCC = vxe.getChunkCoords (playerTrans.position);
			chunkx = playerCC.x;
			chunkz = playerCC.z;

			
			/*mybiome = biome.biomeMap [chunkx, chunkz];

			for (int i =0; i<directions.Length; i++) {
				chunk = vxe.getChunkFromPt (playerTrans.position + directions [i]);
				isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.55f);
				if (isSurface) {
					PullSpawnObject (playerTrans.position + directions [i]);
					break;
				}
			}*/

			//if (mybiome == myItemList.ItemInfoList [currentItemToSpawn].biome)
			//break;
			//yield return null;
			
			/*Scan through for all chunks that are surfaces 1st, also remember their biome,
			 * If they are a surface spawn something from that Biome
			 * I should have a list for each Biome...
			 * So if I find a surface, spawn something randomly from that BiomeList??
			 * Also try spawning certain things based on their height too
			 * Later may be an issue with over spawning, but will get to that later
				*/
		}
	}
	
}
