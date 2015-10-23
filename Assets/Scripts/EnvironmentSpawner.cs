using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Environment
{
	GameObject[] gameObjects;
	Dictionary<Vec3Int,GameObject> table;
}



public class EnvironmentSpawner: MonoBehaviour
{
	Vector3[] directions = {
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
	public Transform playerTrans;
	public GameObject desertGameObjects, grassGameObjects, IceGameObjects, marshGameObjects;
	//May want to use public string tags
	string desertTag, grassTag, IceTag, marshTag;
	
	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	BIOMES mybiome;

	List<GameObject> desertAssets, grassAssets, iceAssets, marshAssets;
	Dictionary<Vec3Int,GameObject> assetChunkTable;

	int framecount = 0;
	Chunks chunk;
	bool isSurface, inHashTable;
	List<Chunks> occupiedNearMe;
	Vec3Int playerCC;
	int chunkx, chunkz;
	bool isSpawning = false;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		mybiome = biome.biomeArea [0].biome;
		occupiedNearMe = new List<Chunks> ();
		assetChunkTable = new Dictionary<Vec3Int, GameObject> ();
		//desertEnvironment = new List<GameObject> ();
		if (playerTrans == null)
			playerTrans = GameObject.FindWithTag ("Player").GetComponent<Transform> ();

		/*if (desertBiome == null)
			desertBiome = GameObject.FindGameObjectWithTag ("Respawn");
		Transform[] children = desertBiome.GetComponentsInChildren<Transform> ();
		Transform listParentTrans = desertBiome.transform;

		desertAssets.Clear ();
		foreach (Transform obj in children) {
			if (!obj.Equals (listParentTrans)) {
				desertAssets.Add (obj.gameObject);
				obj.gameObject.SetActive (false);
			}
		}*/

		InitializeEnvironmentBiome (ref desertGameObjects, "DesertAssets", ref desertAssets);
		InitializeEnvironmentBiome (ref grassGameObjects, "GrassAssets", ref grassAssets);
		InitializeEnvironmentBiome (ref IceGameObjects, "IceAssets", ref iceAssets);
		InitializeEnvironmentBiome (ref marshGameObjects, "MarshAssets", ref marshAssets);
	}

	/// <summary>
	/// Initializes the environment biome.
	/// </summary>
	/// <param name="biome">Biome.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="assetList">Asset list.</param>
	void InitializeEnvironmentBiome (ref GameObject environmentGameObj, string tag, ref List<GameObject> assetList)
	{
		if (environmentGameObj == null)
			environmentGameObj = GameObject.FindGameObjectWithTag (tag);
		if (assetList == null)
			assetList = new List<GameObject> ();

		Transform myTrans = environmentGameObj.GetComponent<Transform> ();
		for (int i=0; i<myTrans.childCount; i++) {
			//if (!childList [i].CompareTag (biome.tag)) {
			assetList.Add (myTrans.GetChild (i).gameObject);
			assetList [i].SetActive (false);
		}
	
	}

	IEnumerator waitToInitialize ()
	{
		while (biome.biomeMap == null) {
			yield return new WaitForEndOfFrame ();
		}
		playerCC = vxe.getChunkCoords (playerTrans.position);
		mybiome = biome.biomeMap [playerCC.x, playerCC.z];
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
	}

	/// <summary>
	/// Sets the active.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	void SetActive (bool active)
	{
		desertGameObjects.SetActive (active);
		IceGameObjects.SetActive (active);
		marshGameObjects.SetActive (active);
		grassGameObjects.SetActive (active);
	}

	List<GameObject> SetEnvironmentListBasedOnBiome (Vector3 Pos)
	{
		playerCC = vxe.getChunkCoords (Pos);
		chunkx = playerCC.x;
		chunkz = playerCC.z;
		
		mybiome = biome.biomeMap [chunkx, chunkz];
		return getNextEnvironmentGameObject (mybiome);
	}

	/// <summary>
	/// Does a the whole pull spawn process in one place.
	/// </summary>
	/// <returns>The pull spawn.</returns>
	IEnumerator FullPullSpawn ()
	{
		Vector3 chunkBaseCoords;
		List<GameObject> assetList;

		/*OPTIMIZE THIS LATER, make it nlog(n) at least !!!!!!!!!!!!!!!
		 Mergesort
		 quicksort
		 COME ON MAN!!*/

		//LOOP through List of Environment Objects***********************************
		//Consider Looping just through the List of Occupied Chunks
		for (int i =directions.Length-1; i>-1; i--) {

			assetList = SetEnvironmentListBasedOnBiome (playerTrans.position + directions [i]);
			//Gets the Chunk at my position
			chunk = vxe.getChunkFromPt (playerTrans.position + directions [i]);
			Vec3Int chunkVXCoords = vxe.getChunkCoords (playerTrans.position + directions [i]);

			//Chunks World Coords
			chunkBaseCoords = vxe.FromGrid (chunkVXCoords * vxe.chunk_size);

			if (chunk == null || assetList.Count == 0)
				continue;
			isSurface = vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.4f);
			inHashTable = assetChunkTable.ContainsKey (chunkVXCoords);

			//If the chunk is a surface and is not in the HashTable, do Spawning code
			if (isSurface && !inHashTable && chunk.voxel_count > 10) {
				GameObject gbj = assetList [0];
				Vector3 pos = onVoxelPosition (chunk, chunkVXCoords);


				gbj.GetComponent<Transform> ().position = pos;
				gbj.SetActive (true);
				assetChunkTable.Add (chunkVXCoords, gbj);
				assetList.RemoveAt (0);
				goto doneSpawn;
			}
			yield return null;
		}
		doneSpawn:
		yield return new WaitForSeconds (1.0f);
		isSpawning = false;

	}

	/// <summary>
	/// Gets the next environment game object.
	/// </summary>
	/// <returns>The next environment game object.</returns>
	/// <param name="newBiome">New biome.</param>
	List<GameObject> getNextEnvironmentGameObject (BIOMES newBiome)
	{
		switch (newBiome) {
		case BIOMES.grass:
			return	grassAssets;
		case BIOMES.sand:
			return desertAssets;
		case BIOMES.ice:
			return iceAssets;
		default: //marsh
			return marshAssets;
		}
	}

	/// <summary>
	/// Ons the voxel position.
	/// </summary>
	/// <returns>The voxel position.</returns>
	/// <param name="floorChunkY">Floor chunk y.</param>
	/// <param name="range">Range.</param>
	/// <param name="chunk">Chunk.</param>
	/// <param name="chunkBaseCoords">Chunk base coords.</param>
	Vector3 onVoxelPosition (Chunks chunk, Vec3Int chunkGridCoord)
	{
		Vector3 chunkBCoords = new Vector3 (chunkGridCoord.x, chunkGridCoord.y, chunkGridCoord.z) * vxe.chunk_size;
				
		for (int x=0; x<vxe.chunk_size; x++)
			for (int z=0; z<vxe.chunk_size; z++)
				for (int y=vxe.chunk_size-1; y>=0; y--) {
					Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
					if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) {
						Vector3 voxelCoords = vxe.FromGridUnTrunc (chunkBCoords + new Vector3 (x, y, z) * vxe.voxel_size);
						//if (voxelCoords.y <= coords.y + items [currentItemToSpawn].minSpawnHeightOffFloor * vxe.voxel_size)
						//	continue;
						return voxelCoords + Vector3.up * vxe.voxel_size * 1.0f;
						//GameObject newItem = (GameObject)Instantiate (items [currentItemToSpawn].item, , Quaternion.identity);
						break;
					}
				}

		return chunkBCoords;
	}

	/// <summary>
	/// Swaps the biome sets as well as Destroys the GameObjects spawns (resets any counts)
	/// </summary>
	public void SwapEnvironments (ref Material[] newMat, ref GameObject[] list)
	{
		biome.swapMaterials (ref newMat);
		SetActive (false);

		Swap (ref desertGameObjects, ref list [0]);
		Swap (ref grassGameObjects, ref list [1]);
		Swap (ref IceGameObjects, ref list [2]);
		Swap (ref marshGameObjects, ref list [3]);
		//give SWAP Materials a parameter for ItemList Materials...or the WarpController Materials

		SetActive (true);
	}

	void Swap (ref GameObject a, ref GameObject b)
	{
		GameObject temp = a;
		a = b;
		b = temp;
	}
	
	/// <summary>
	/// Destroys the spawns, except those marked as Do Not Destroy
	/// </summary>
	void DestroySpawns ()
	{
	
	}

}
