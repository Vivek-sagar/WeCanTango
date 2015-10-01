using UnityEngine;
using System.Collections;


[System.Serializable]
public struct Spawns
{
	public GameObject[] obj;
	public BIOMES biome;
	public bool[] sticksToWalls;
}

public class Spawner : MonoBehaviour {
	VoxelExtractionPointCloud vxe;
	BiomeScript biome;
	public Spawns[] spawns;
	public Camera camera;
	int count = 0;
	int framecount = 0;
	// Use this for initialization
	void Start () {
		vxe = VoxelExtractionPointCloud.Instance;
		biome = BiomeScript.Instance;
		//for (int i=0; i<spawnObjects.Length; i++) 
		//	spawnObjects[i].SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		framecount++;
		if(framecount % 120 != 0 || count > 20)
			return;

		Random.seed = (int)(Time.deltaTime * 1000);

		Vector3 ranPt = new Vector3 (Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f), camera.nearClipPlane); 
		Vector3 startpt = camera.ViewportToWorldPoint (ranPt);
		Vector3 dir = startpt - camera.transform.position;
		Vector3 pos = new Vector3 (), normal = new Vector3 ();

		if (vxe.RayCast (startpt, dir, 64, ref pos, ref normal)) {

			Vec3Int chunkcoord = vxe.ToGrid(pos) / vxe.chunk_size;
			BIOMES mybiome = biome.biomeMap[chunkcoord.x,chunkcoord.z];

			int animal = Random.Range(0, spawns[(int)mybiome].obj.Length);
			GameObject spawnObject = spawns[(int)mybiome].obj[animal];

			if(spawns[(int)mybiome].sticksToWalls[animal] || Vector3.Dot (normal,Vector3.up) > 0.999f)
			{
				GameObject newsphere = (GameObject)Instantiate (spawnObject, pos + normal * VoxelExtractionPointCloud.Instance.voxel_size * 0.5f, Quaternion.identity);
				newsphere.SetActive (true);
				SimpleAI ai = newsphere.GetComponent<SimpleAI>();

				//newsphere.GetComponent<GrowScript>().init(pos, normal, (Vector3.Dot (normal,Vector3.up) > 0.999f) );
				count++;
			}
		}

	}
}
