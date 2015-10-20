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
	bool isSurface, hasKey;
	List<Chunks> occupiedNearMe;
	Vec3Int playerCC;
	int chunkx, chunkz;
	BIOMES mybiome;
	Dictionary<Vec3Int,GameObject> desertTable;
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
		if (framecount % spawnInterval != 0) {
			return;
		}


		/*Player Chunk Coord
		 * */
	

		playerCC = vxe.getChunkCoords (playerTrans.position);
		chunkx = playerCC.x;
		chunkz = playerCC.z;
	
		mybiome = biome.biomeMap [chunkx, chunkz];
		/*if (mybiome != BIOMES.sand)
			return;*/
		for (int i =0; i<directions.Length; i++) {
			chunk = vxe.getChunkFromPt (playerTrans.position + directions [i]);
			Vec3Int chunkVXCoords = vxe.getChunkCoords (playerTrans.position + directions [i]);
			if (chunk == null)
				continue;
			isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.55f);
			hasKey = desertTable.ContainsKey (chunkVXCoords);
			if (isSurface && !hasKey) {
				PullSpawnObject (playerTrans.position + directions [i], chunkVXCoords, chunk);
				break;
			}
		}
	}

	void PullSpawnObject (Vector3 chunkBaseCoords, Vec3Int chunkVXCoords, Chunks chunk)
	{
		if (desertEnvironment.Count > 0) {
			GameObject gbj = desertEnvironment [0];

			//Loop thru each voxel in the chunk
			for (int x=0; x<vxe.chunk_size; x++)
				for (int z=0; z<vxe.chunk_size; z++)
					for (int y=vxe.chunk_size-1; y>=0; y--) {
						Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
						//if vx is occ and has a sruface, spawn on top
						if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) {
							Vector3 voxelCoords = vxe.FromGridUnTrunc (chunkBaseCoords + new Vector3 (x, y, z));
						
							gbj.GetComponent<Transform> ().position = voxelCoords;
							gbj.SetActive (true);
							desertEnvironment.RemoveAt (0);
							desertTable.Add (chunkVXCoords, gbj);
							break;
						}
						//Keep THIS here so less Loops each frame
						
					}
		}
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
