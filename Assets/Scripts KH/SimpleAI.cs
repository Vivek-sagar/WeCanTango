using UnityEngine;
using System.Collections;

public enum AI_STATE
{
	MOVING = 0,
	RUNNING = 1,
	STOPPED = 2,
	ATTACKING =3
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

public enum AI_TYPE : int
{
	AFRAID = 0,
	MEAN = 1,
	NICE =2
}

public class SimpleAI : MonoBehaviour
{
	VoxelExtractionPointCloud vxe;
	AI_STATE ai_state;
	Vector3 lastposition;
	Transform myTrans;
	int runStep = 1;

	public GameObject emotion;
	public AI_TYPE ai_type;

	static Vector3[] directions = {
		new Vector3 (0, 0, 1),
		new Vector3 (0, 0, -1),
		new Vector3 (1, 0, 0),
		new Vector3 (-1, 0, 0),
		new Vector3 (0, 1, 0),
		new Vector3 (0, -1, 0)
	};
	const int STEP = 5;
	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		ai_state = AI_STATE.STOPPED;
		vxe = VoxelExtractionPointCloud.Instance;
		lastposition = new Vector3 ();
		emotion.SetActive (false);
		init ();
	}

	Vector3 getRandomDir ()
	{
		DIRECTIONS dir = (DIRECTIONS)Random.Range (0, 4);
		return directions [(int)dir];
	}

	Vector3 getNextMove ()
	{
		Vector3 coords = Vector3.zero;
		Vector3 norm = Vector3.zero;

		for (int i=0; i<4; i++) {
			Vector3 dir = getRandomDir ();
			if (!vxe.RayCast (myTrans.position, dir, runStep * STEP, ref coords, ref norm)) {
				return dir;
			}
		}

		for (int i=0; i<4; i++) {
			Vector3 dir = directions [i];
			if (!vxe.RayCast (myTrans.position, dir, runStep * STEP, ref coords, ref norm)) {
				return dir;
			}
		}

		myTrans.position += Vector3.up * vxe.voxel_size;
		lastposition = myTrans.position;

		return Vector3.zero;

	}

	IEnumerator startMoving ()
	{
		ai_state = AI_STATE.MOVING;
		while ((int)ai_state > 1) {
			Vector3 dir = getNextMove ();
			myTrans.forward = dir;

			for (float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f) {
				//condition ? result if true : result if false ;
				runStep = (ai_state == AI_STATE.RUNNING) ? 10 : 1;
				myTrans.position += dir * vxe.voxel_size * runStep * STEP * Time.deltaTime * 0.2f;
				yield return null;
			}
		}
	}

	IEnumerator startAttacking (Vector3 target)
	{
		ai_state = AI_STATE.ATTACKING;

		while (ai_state == AI_STATE.ATTACKING) {
			myTrans.LookAt (target);
			for (float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f) {
				//condition ? result if true : result if false ;
				runStep = (ai_state == AI_STATE.RUNNING) ? 10 : 1;
				myTrans.position += myTrans.forward.normalized * vxe.voxel_size * runStep * STEP * Time.deltaTime * 0.8f;
				yield return null;
			}
		}
	}

	public void init ()
	{
		StartCoroutine (startMoving ());
	}


	public void OnTriggerStay (Collider other)
	{
		if (other.CompareTag ("Player")) {
			switch (ai_type) {
			case AI_TYPE.AFRAID:
				ai_state = AI_STATE.RUNNING;
				break;
			case AI_TYPE.MEAN:
				ai_state = AI_STATE.ATTACKING;
				break;
			}

			emotion.SetActive (true);
		}

	}

	public void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Player")) {
			switch (ai_type) {
			case AI_TYPE.AFRAID:
			case AI_TYPE.MEAN:
				ai_state = AI_STATE.MOVING;
				break;
			
			}
			emotion.SetActive (false);
		}
	}
}
