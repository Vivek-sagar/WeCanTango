using UnityEngine;
using System.Collections;

public enum AI_STATE
{
	MOVING = 0,
	STOPPED = 1
}

enum DIRECTIONS : int
{
	FORWARD = 0,
	BACKWARD = 1,
	LEFT = 2,
	RIGHT = 3,
	UP = 4,
	DOWN = 5
}



public class SimpleAI : MonoBehaviour {
	VoxelExtractionPointCloud vxe;
	AI_STATE ai_state;
	Vector3 lastposition;
	static Vector3[] directions = { new Vector3(0,0,1),new Vector3(0,0,-1),new Vector3(1,0,0),new Vector3(-1,0,0),new Vector3(0,1,0),new Vector3(0,-1,0) };
	const int STEP = 5;
	// Use this for initialization
	void Start () {
		ai_state = AI_STATE.STOPPED;
		vxe = VoxelExtractionPointCloud.Instance;
		lastposition = new Vector3 ();
		init ();
	}

	Vector3 getRandomDir()
	{
		DIRECTIONS dir = (DIRECTIONS)Random.Range (0, 4);
		return directions[(int)dir];
	}

	Vector3 getNextMove()
	{
		Vector3 coords = Vector3.zero;
		Vector3 norm = Vector3.zero;

		for(int i=0;i<4;i++)
		{
			Vector3 dir = getRandomDir();
			if(!vxe.RayCast(transform.position,dir,STEP,ref coords,ref norm))
			{
				return dir;
			}
		}

		for(int i=0;i<4;i++)
		{
			Vector3 dir = directions[i];
			if(!vxe.RayCast(transform.position,dir,STEP,ref coords,ref norm))
			{
				return dir;
			}
		}

		transform.position += Vector3.up * vxe.voxel_size;

		return Vector3.zero;

	}

	IEnumerator startMoving()
	{
		while(true)
		{
			Vector3 dir = getNextMove ();
			transform.forward = dir;
			for(float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f)
			{
				transform.position += dir * vxe.voxel_size * STEP * Time.deltaTime * 0.2f;
				yield return null;
			}
		}
	}

	public void init()
	{
		StartCoroutine (startMoving ());
	}

	// Update is called once per frame
	void Update () {
	
	}
}
