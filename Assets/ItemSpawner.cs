using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*[System.Serializable]
public class ItemInfo
{
	public GameObject item;
	public BIOMES biome;
	public int minSpawnHeightOffFloor;	
	public bool spawnOnce;
	public bool DoNotDestroy;
	[HideInInspector]
	public int
		spawnCount;
}*/

[RequireComponent(typeof(ItemsList))]
public class ItemSpawner : Singleton<ItemSpawner>
{
	public Camera camera;
	//public ItemInfo[] items;

	[HideInInspector]
	public ItemsList
		myItemList;
	[HideInInspector]
	public List<GameObject>
		spawnedItems, dontDestroySpawnedItems;

	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	int currentItemToSpawn = 0;
	int floorChunkY = 0;

	const int range = 2;

	void Start ()
	{
		myItemList = GetComponent<ItemsList> ();
		spawnedItems = new List<GameObject> ();// new GameObject[items.Length];
		dontDestroySpawnedItems = new List<GameObject> ();

		/*for (int i=0; i<items.Length; i++)
			spawnedItems [i] = null;
*/
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		StartCoroutine (SpawnItems ());
	}

	IEnumerator SpawnItems ()
	{
		yield return new WaitForSeconds (30.0f);
		Vector3 floorCoords = Vector3.zero, norm = Vector3.zero;

		bool hitsomething = false;
		while (!hitsomething) {
			hitsomething = vxe.RayCast (camera.transform.position, Vector3.down, 64, ref floorCoords, ref norm, 1.0f);
			yield return null;
		}

		//detects the floor chunk and Y coord
		floorChunkY = vxe.getChunkCoords (floorCoords).y;

		for (int i=0; i<myItemList.ItemInfoList.Length; i++) {
			bool spawned = false;
			while (!spawned) {
				int chunkx;
				int chunkz;

				//checks Occ Chunks for biomes (vxe NEW feature has Occ Chunk Stack
				while (true) {
					Vec3Int randomCC = vxe.occupiedChunks.peek (Random.Range (0, vxe.occupiedChunks.getCount ()));
					chunkx = randomCC.x;
					chunkz = randomCC.z;

					BIOMES mybiome = biome.biomeMap [chunkx, chunkz];
					if (mybiome == myItemList.ItemInfoList [currentItemToSpawn].biome)
						break;
					yield return null;
				}

				Chunks chunk = null;
				//
				/*Search from top of cuhnk down, for the top face appearing. 
*Spawns one item a time
*spawns on surfaces only
*can easily be modified to spawn on sides
*
				 */
				for (int k=floorChunkY + range; k >= floorChunkY; k--) {
					chunk = vxe.grid.voxelGrid [chunkx, k, chunkz];
					if (chunk != null && chunk.voxel_count > 10 && vxe.isChunkASurface (DIR.DIR_UP, chunk, 0.4f)) {
						//The sushi mat to see if detecting right chunk
						vxe.chunkGameObjects [chunkx, k, chunkz].GetComponent<MeshRenderer> ().material = vxe.debugMaterial;

						Vector3 chunkBaseCoords = new Vector3 (chunkx, k, chunkz) * vxe.chunk_size;
						bool alreadySpawnOnce;

						//Loop thru each voxel in the chunk
						for (int x=0; x<vxe.chunk_size; x++)
							for (int z=0; z<vxe.chunk_size; z++)
								for (int y=vxe.chunk_size-1; y>=0; y--) {
									Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
									//if vx is occ and has a sruface, spawn on top
									if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) {
										Vector3 voxelCoords = vxe.FromGridUnTrunc (chunkBaseCoords + new Vector3 (x, y, z));
										//simple checks if the item can spawn at this voxel height 	
										alreadySpawnOnce = (myItemList.ItemInfoList [currentItemToSpawn].spawnOnce 
											&& myItemList.ItemInfoList [currentItemToSpawn].spawnCount > 0);

										if (voxelCoords.y <= floorCoords.y + myItemList.ItemInfoList [currentItemToSpawn].maxSpawnHeightOffFloor * vxe.voxel_size 
											|| alreadySpawnOnce)
											continue;

										GameObject newItem = (GameObject)Instantiate (myItemList.ItemInfoList [currentItemToSpawn].item, voxelCoords + Vector3.up * vxe.voxel_size * 1.0f, Quaternion.identity);
										myItemList.ItemInfoList [currentItemToSpawn].spawnCount++;
										newItem.SetActive (true);
										
										if (myItemList.ItemInfoList [currentItemToSpawn].DoNotDestroy)
											dontDestroySpawnedItems.Add (newItem);
										else
											spawnedItems.Add (newItem);
										//spawnedItems [currentItemToSpawn] = newItem;

										currentItemToSpawn++;
										spawned = true;
										goto imout;
									}
									//Keep THIS here so less Loops each frame
									yield return null;
								}
					}
				}
					
				imout:

				yield return new WaitForSeconds (1.0f);
			}
		}
	}

	/// <summary>
	/// Swaps the biome sets as well as Destroys the GameObjects spawns (resets any counts)
	/// </summary>
	public void SwapItemLists (ref ItemsList.ItemInfo[] newList, ref Material[] newMat)
	{
		ItemsList.ItemInfo[] tempList = myItemList.ItemInfoList;
		myItemList.ItemInfoList = newList;
		newList = tempList;
		DestroySpawns ();	
		//give SWAP Materials a parameter for ItemList Materials...or the WarpController Materials
		biome.swapMaterials (ref newMat);
	}

	/// <summary>
	/// Destroys the spawns, except those marked as Do Not Destroy
	/// </summary>
	void DestroySpawns ()
	{
		GameObject temp;
		for (int i=spawnedItems.Count - 1; i > -1; i--) {
			temp = spawnedItems [i];
			Destroy (temp);
		}
		spawnedItems.Clear ();
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (1500, 10, 100, 100), "ITEMS SPAWNED:" + currentItemToSpawn + "Floor chunk: " + floorChunkY);
	}
}
