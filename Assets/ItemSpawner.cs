using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemInfo
{
	public GameObject item;
	public BIOMES biome;
}

public class ItemSpawner : Singleton<ItemSpawner> {
	public Camera camera;
	public ItemInfo[] items;

	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	int currentItemToSpawn = 0;
	int floorChunkY = 0;

	const int range = 2;
	const int minSpawnHeightOffFloor = 3;

	void Start () {
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		StartCoroutine (SpawnItems ());
	}

	IEnumerator SpawnItems()
	{
		yield return new WaitForSeconds(30.0f);
		Vector3 coords = Vector3.zero, norm = Vector3.zero;

		bool hitsomething = false;
		while(!hitsomething)
		{
			hitsomething = vxe.RayCast(camera.transform.position,Vector3.down,64,ref coords,ref norm,1.0f);
			yield return null;
		}

		floorChunkY = vxe.getChunkCoords (coords).y;

		for(int i=0;i<items.Length;i++)
		{
			bool spawned = false;
			while(!spawned)
			{
				int chunkx;
				int chunkz;

				while(true)
				{
					chunkx = (Random.Range(0,vxe.num_chunks_x) + Random.Range(0,vxe.num_chunks_x) + Random.Range(0,vxe.num_chunks_x)) % vxe.num_chunks_x;
					chunkz = (Random.Range(0,vxe.num_chunks_z) + Random.Range(0,vxe.num_chunks_z) + Random.Range(0,vxe.num_chunks_z)) % vxe.num_chunks_z;

					BIOMES mybiome = biome.biomeMap[chunkx,chunkz];
					if(mybiome == items[currentItemToSpawn].biome)
						break;
					yield return null;
				}

				Chunks chunk = null;

				for(int k=floorChunkY + range; k >= floorChunkY;k--)
				{
					chunk = vxe.grid.voxelGrid[chunkx,k,chunkz];
					if(chunk!=null && chunk.voxel_count > 10 && vxe.isChunkASurface(DIR.DIR_UP,chunk,0.3f))
					{
						vxe.chunkGameObjects[chunkx,k,chunkz].GetComponent<MeshRenderer>().material = vxe.debugMaterial;

						Vector3 chunkBaseCoords = new Vector3(chunkx,k,chunkz) * vxe.chunk_size;

						for(int x=0;x<vxe.chunk_size;x++)
							for(int z=0;z<vxe.chunk_size;z++)
								for(int y=vxe.chunk_size-1;y>=0;y--)
						{
							Voxel vx = chunk.getVoxel(new Vec3Int(x,y,z));
							if(vx.isOccupied() &&  Vector3.Dot (vxe.getVoxelNormal(vx),Vector3.up) > 0.99f)
							{
								Vector3 voxelCoords = vxe.FromGridUnTrunc(chunkBaseCoords + new Vector3(x,y,z));
								if(voxelCoords.y <= coords.y + minSpawnHeightOffFloor * vxe.voxel_size)
										continue;

								GameObject newItem = (GameObject)Instantiate (items[currentItemToSpawn].item, voxelCoords + Vector3.up * vxe.voxel_size * 1.0f, Quaternion.identity);
								newItem.SetActive (true);
								
								currentItemToSpawn++;
								spawned = true;
								goto imout;
							}
							yield return null;
						}
					}
				}
					
				imout:

				yield return new WaitForSeconds(1.0f);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		

		
	}

	void OnGUI()
	{
		GUI.Label (new Rect (1500,10,100,100),"ITEMS SPAWNED:" + currentItemToSpawn + "Floor chunk: " + floorChunkY);
	}
}
