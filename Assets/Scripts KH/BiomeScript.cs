using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BIOMES : int
{
	grass = 0,
	ice = 1,
	water = 2,
	sand = 3,
}

public class BiomeScript : Singleton<BiomeScript>
{
	VoxelExtractionPointCloud vxe;
	GameObject[,,] chunkObjs;

	[System.Serializable]
	public class BiomeArea
	{
		public int radius;
		public BIOMES biome;
		public Vector2 position;
	}

	public BiomeArea[] biomeArea;

	[System.Serializable]
	public struct BiomeSet
	{
		public Material tranformMat;
		public BIOMES biome;
	}

	[HideInInspector]
	public BIOMES[,]
		biomeMap; 

	public Material[] materials;

	int num_chunks_x;
	int num_chunks_y;
	int num_chunks_z;
	// Use this for initialization
	void Awake ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		chunkObjs = vxe.chunkGameObjects;
        
		num_chunks_x = vxe.num_chunks_x;
		num_chunks_y = vxe.num_chunks_y;
		num_chunks_z = vxe.num_chunks_z;
		
		initBiomes ();
	}

	void initBiomes ()
	{
		biomeMap = new BIOMES[num_chunks_x, num_chunks_z];

		for (int i=0; i<num_chunks_x; i++)
			for (int j=0; j<num_chunks_z; j++) {
				biomeMap [i, j] = BIOMES.sand;

				Vector2 myvec = new Vector2 (i, j);
				for (int k=0; k<biomeArea.Length; k++) {
					Vector2 localvec = myvec - biomeArea [k].position;

					if (localvec.sqrMagnitude < (biomeArea [k].radius * biomeArea [k].radius)) {
						biomeMap [i, j] = biomeArea [k].biome;
					}
				}
			}

		resetBiomes ();
	}

	/// <summary>
	/// Resets the biomes.
	/// </summary>
	public void resetBiomes ()
	{
		for (int i=0; i<num_chunks_x; i++)
			for (int j=0; j<num_chunks_z; j++)
				for (int k=0; k<num_chunks_y; k++) {
					chunkObjs [i, k, j].GetComponent<MeshRenderer> ().material = materials [(int)biomeMap [i, j]];
				}
	}

	/// <summary>
	/// Resets the biomes.
	/// </summary>
	public IEnumerator resetBiomesThread ()
	{
		for (int i=0; i<num_chunks_x; i++)
			for (int j=0; j<num_chunks_z; j++)
				for (int k=0; k<num_chunks_y; k++) {
					chunkObjs [i, k, j].GetComponent<MeshRenderer> ().material = materials [(int)biomeMap [i, j]];
					yield return null;
				}
	}

	/// <summary>
	/// Swaps the materials for the biome with NewMat, and then resets all the biomes materials
	/// </summary>
	/// <param name="newMat">New materials to use.</param>
	public void swapMaterials (ref Material[] newMat)
	{
		Material[] tempMat = materials;
		materials = newMat;
		newMat = tempMat;

		//setAllMaterials(materials[0]);
		resetBiomes ();
	}

	public void setAllMaterials (Material mat)
	{
		for (int i=0; i<num_chunks_x; i++)
			for (int j=0; j<num_chunks_z; j++)
				for (int k=0; k<num_chunks_y; k++) {
					chunkObjs [i, k, j].GetComponent<MeshRenderer> ().material = mat;
				}
	}

}
