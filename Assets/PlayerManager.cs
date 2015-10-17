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
	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	Transform playerTrans;
	int framecount = 0;

	List<Chunks> occupiedNearMe;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		occupiedNearMe = new List<Chunks> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		framecount++;
		if (framecount % spawnInterval != 0)
			return;
	}

	void SpawnNearPlayer ()
	{
		int chunkx, chunkz;
		Chunks chunk;
		Vec3Int randomCC;
		bool isSurface;
		while (true) {
			//Random Chunk Coord
			randomCC = vxe.getChunkCoords (playerTrans.position);
			chunkx = randomCC.x;
			chunkz = randomCC.z;
			chunk = vxe.getChunkFromPt (playerTrans.position);
			isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.55f);
			
			BIOMES mybiome = biome.biomeMap [chunkx, chunkz];
			//if (mybiome == myItemList.ItemInfoList [currentItemToSpawn].biome)
			break;
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
