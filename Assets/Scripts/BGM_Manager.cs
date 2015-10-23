using UnityEngine;
using System.Collections;

public class BGM_Manager : MonoBehaviour
{

	BIOMES mybiome, prevBiome;
	BiomeScript biome;
	VoxelExtractionPointCloud vxe;
	Vec3Int playerCC;
	
	public Transform playerTrans;
	public AudioClip grassAudio, marshAudio, sandAudio, iceAudio;
	AudioSource au_source;

	const int spawnInterval = 300;
	int framecount = 0;
	// Use this for initialization
	void Start ()
	{
		//Initialize VBariables
		biome = BiomeScript.Instance;	
		vxe = VoxelExtractionPointCloud.Instance;
		au_source = GetComponent<AudioSource> ();
		if (playerTrans == null)
			playerTrans = GameObject.FindWithTag ("Player").GetComponent<Transform> ();

		/*playerCC = vxe.getChunkCoords (playerTrans.position);
		mybiome = biome.biomeMap [playerCC.x, playerCC.z];
		prevBiome = mybiome;
		switchBiomeAudio (mybiome);
		*/
		StartCoroutine (waitToInitialize ());

	}

	IEnumerator waitToInitialize ()
	{
		while (biome.biomeMap == null) {
			yield return new WaitForEndOfFrame ();
		}
		playerCC = vxe.getChunkCoords (playerTrans.position);
		mybiome = biome.biomeMap [playerCC.x, playerCC.z];
		prevBiome = mybiome;
		switchBiomeAudio (mybiome);
	}
	
	// Update is called once per frame
	void Update ()
	{
		framecount++;
		if (framecount % spawnInterval != 0 || biome.biomeMap == null) {
			return;
		}

		if (checkChangeBiome ()) {
			prevBiome = mybiome;
			switchBiomeAudio (mybiome);
		}
	}

	/// <summary>
	/// Changes the biome.
	/// </summary>
	/// <returns><c>true</c>, if biome was changed, <c>false</c> otherwise.</returns>
	bool checkChangeBiome ()
	{
		if (biome.biomeMap == null)
			return false;
		playerCC = vxe.getChunkCoords (playerTrans.position);
		mybiome = biome.biomeMap [playerCC.x, playerCC.z];

		return (mybiome != prevBiome);
	}

	void switchBiomeAudio (BIOMES newBiome)
	{

		switch (newBiome) {
		case BIOMES.grass:
			au_source.clip = grassAudio;
			break;
		case BIOMES.sand:
			au_source.clip = sandAudio;
			break;
		case BIOMES.ice:
			au_source.clip = iceAudio;
			break;
		case BIOMES.water:
			au_source.clip = marshAudio;
			break;

		}
		au_source.Play ();
	}
}
