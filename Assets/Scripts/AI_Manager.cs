using UnityEngine;
using System.Collections;

public class AI_Manager : MonoBehaviour
{

	public VoxelGrid voxelGrid;
	public VoxelExtractionPointCloud voxelPointCloud;
	public GameObject tanPrefab;

	// Use this for initialization
	void Start ()
	{
	
	}

	IEnumerator SpawnTan ()
	{
		Vector3 position = Vector3.up * voxelPointCloud.num_voxels_y;

		GameObject AIobj = Instantiate (tanPrefab, position, Quaternion.identity) as GameObject;
		AIBehaviour ai = AIobj.GetComponent<AIBehaviour> ();

		ai.jumpSpeed = voxelPointCloud.voxel_size;
		ai.myState = AIBehaviour.AI_State.Jump;
		yield return null;
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}
