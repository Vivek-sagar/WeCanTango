using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemInfo
{
	public GameObject item;
	public BIOMES biome;
	public int minSpawnHeightOffFloor;
	public float maxSpawnHeightOffFloor;
}

public class ItemSpawner : Singleton<ItemSpawner>
{
	public Camera camera;
	public ItemInfo[] items;

	[HideInInspector]
	public GameObject[]
		spawneditems;

	public GameObject bushObject;

	[HideInInspector]
	public bool canSpawn = true;

	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	public int currentItemToSpawn = 0;
	int floorChunkY = 0;

	const int range = 2;

	void Start ()
	{
		spawneditems = new GameObject[items.Length];

		for (int i=0; i<items.Length; i++)
			spawneditems [i] = null;

		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		StartCoroutine (SpawnItems ());
	}

	IEnumerator SpawnItems ()
	{
		yield return new WaitForSeconds (10.0f);
		Vector3 coords = Vector3.zero, norm = Vector3.zero;

		bool hitsomething = false;
		while (!hitsomething) {
			hitsomething = vxe.RayCast (camera.transform.position, Vector3.down, 64, ref coords, ref norm, 1.0f);
			yield return null;
		}

		floorChunkY = vxe.getChunkCoords (coords).y;

		for (int i=0; i<items.Length; i++) {

			TriggerScript sheepTrigger = null;

			bool spawned = false;
			while (!spawned) {
				int chunkx;
				int chunkz;

				while (true) {
					Vec3Int randomCC = vxe.occupiedChunks.peek (Random.Range (0, vxe.occupiedChunks.getCount ()));
					chunkx = randomCC.x;
					chunkz = randomCC.z;

					BIOMES mybiome = biome.biomeMap [chunkx, chunkz];
					if (mybiome == items [currentItemToSpawn].biome)
						break;
					yield return null;
				}

				Chunks chunk = null;

				for (int k=floorChunkY + range; k >= floorChunkY; k--) 
				{
					chunk = vxe.grid.voxelGrid [chunkx, k, chunkz];
					if (chunk != null && chunk.voxel_count > 20) 
					{
						Vector3 chunkBaseCoords = new Vector3 (chunkx, k, chunkz) * vxe.chunk_size;

						for (int x=0; x<vxe.chunk_size; x++)
							for (int z=0; z<vxe.chunk_size; z++)
								for (int y=vxe.chunk_size-1; y>=0; y--) 
							{
									Voxel vx = chunk.getVoxel (new Vec3Int (x, y, z));
									if (vx.isOccupied () && vxe.voxelHasSurface (vx, VF.VX_TOP_SHOWN)) 
								{
										Vector3 voxelCoords = vxe.FromGridUnTrunc (chunkBaseCoords + new Vector3 (x, y, z));
										if (voxelCoords.y < coords.y + items [currentItemToSpawn].minSpawnHeightOffFloor * vxe.voxel_size || voxelCoords.y > coords.y + items [currentItemToSpawn].maxSpawnHeightOffFloor * vxe.voxel_size)
											continue;

										GameObject newItem = (GameObject)Instantiate (items [currentItemToSpawn].item, voxelCoords + Vector3.up * vxe.voxel_size * 1.0f, Quaternion.identity);
										newItem.SetActive (true);
										
										GameObject newBushItem = (GameObject)Instantiate (bushObject, voxelCoords + Vector3.up * vxe.voxel_size * 1.0f, Quaternion.identity);
										newBushItem.SetActive (true);

										newBushItem.GetComponent<TriggerScript>().littleSheep = newItem;

										spawneditems [currentItemToSpawn] = newBushItem;
										vxe.chunkGameObjects [chunkx, k, chunkz].GetComponent<MeshRenderer> ().material = vxe.debugMaterial;
										currentItemToSpawn++;
										spawned = true;
										Debug.Log ("spawned!");
										canSpawn = false;
										goto imout;
									}
									yield return null;
								}
					}
				}
					
				imout:
				
				while(!canSpawn)
					yield return new WaitForSeconds (1.0f);
			}

		}
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (1500, 10, 100, 100), "ITEMS SPAWNED:" + currentItemToSpawn + "Floor chunk: " + floorChunkY);
	}
}
