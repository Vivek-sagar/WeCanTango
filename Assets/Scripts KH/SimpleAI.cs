//#define DEBUG_THIS
using UnityEngine;
using System.Collections;

public enum AI_STATE
{
	STOPPED = 0,
	MOVING = 1,
	RUNNING =2,
	AFFECTION =3,
	ATTACKING =4,
}

public enum AI_TYPE
{
	NICE = 0,
	MEAN = 1,	
	AFRAID = 2,
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



public class SimpleAI : MonoBehaviour
{
	VoxelExtractionPointCloud vxe;
	public AI_STATE ai_state;
	Vector3 lastposition;

	Transform myTrans;
	int runStep = 1;
	AudioSource au_source;
	Animator myAnim;

	public GameObject emotion;
	public AI_TYPE ai_type;
	public AudioClip idleClip, emotionClip;
	public float xRot = 0;
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
		myAnim = GetComponent<Animator> ();
		myTrans = GetComponent<Transform> ();
		ai_state = AI_STATE.STOPPED;
		vxe = VoxelExtractionPointCloud.Instance;
		lastposition = new Vector3 ();
		emotion.SetActive (false);
		au_source = GetComponent<AudioSource> ();
		init ();
	}

	void Update ()
	{
		myTrans.localRotation = Quaternion.Euler (xRot, myTrans.localRotation.eulerAngles.y, myTrans.localRotation.eulerAngles.z);
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
			if (!vxe.RayCast (transform.position, dir, STEP, ref coords, ref norm)) {
				return dir;
			}
		}

		for (int i=0; i<4; i++) {
			Vector3 dir = directions [i];
			if (!vxe.RayCast (transform.position, dir, STEP, ref coords, ref norm)) {
				return dir;
			}
		}

		transform.position += Vector3.up * vxe.voxel_size;

		return Vector3.zero;

	}

	IEnumerator startMoving ()
	{
		ai_state = AI_STATE.MOVING;
		PlayIdleNoises ();

		while ((int)ai_state < 2) {
			Vector3 dir = getNextMove ();
			myTrans.forward = dir;

			for (float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f) {
				//condition ? result if true : result if false ;
				runStep = (ai_state == AI_STATE.RUNNING) ? 2 : 1;
				myTrans.position += dir * vxe.voxel_size * runStep * STEP * Time.deltaTime * 0.2f;
				yield return null;
			}
		}
	}

	public void PlayAngryNoises ()
	{
		if (ai_state != AI_STATE.ATTACKING) {
			au_source.Stop ();
			au_source.clip = (idleClip);
			return;
		}
		if (!au_source.isPlaying) {
			au_source.clip = (emotionClip);
			au_source.loop = true;
			au_source.Play ();
		}
	}

	public void PlayIdleNoises ()
	{
		if (ai_state != AI_STATE.MOVING) {
			au_source.Stop ();
			return;
		}
		if (!au_source.isPlaying) {
			au_source.clip = (idleClip);
			au_source.loop = true;
			au_source.Play ();
		}
	}

	IEnumerator startAttacking (Transform target)
	{
		ai_state = AI_STATE.ATTACKING;
		myAnim.SetBool ("isAngry", true);
		
		Vector3 dir = target.position - myTrans.position;
		//myTrans.LookAt (target);
		for (float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f) {
			//condition ? result if true : result if false ;
			//myTrans.forward = dir;

			myTrans.position += dir * vxe.voxel_size * 2 * STEP * Time.deltaTime * 0.8f;
			yield return null;
		}
		ai_state = AI_STATE.MOVING;
		myAnim.SetBool ("isAngry", false);
	}

	public void init ()
	{
		StartCoroutine (startMoving ());
	}

	IEnumerator BounceHeart (Vector3 target)
	{
		ai_state = AI_STATE.AFFECTION;
		myTrans.LookAt (target);
		myAnim.SetBool ("showAffection", true);
		while (ai_state == AI_STATE.AFFECTION) {
			yield return null;
		}
		myAnim.SetBool ("showAffection", false);
		/*Vector3 dir = emotion.transform.up;
			emotion.transform.up = dir;
			
			for (float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f) {
				emotion.transform.position += dir * vxe.voxel_size * STEP * Time.deltaTime * 0.2f;
				yield return null;
			}

			dir = emotion.transform.up * -1f;
			
			for (float i=0; i< vxe.voxel_size * STEP; i+=Time.deltaTime * 0.2f) {
				emotion.transform.position += dir * vxe.voxel_size * STEP * Time.deltaTime * 0.2f;
				yield return null;
			}
		}
		emotion.transform.position = origin;*/
		yield return null;
	}


	public void OnTriggerStay (Collider other)
	{
		if (other.CompareTag ("Player")) {
			switch (ai_type) {
			case AI_TYPE.AFRAID:
				ai_state = AI_STATE.RUNNING;
				break;
			case AI_TYPE.MEAN:
				if (ai_state != AI_STATE.ATTACKING)
					StartCoroutine (startAttacking (other.transform));
				break;
			case AI_TYPE.NICE:
				if (ai_state != AI_STATE.AFFECTION)
					StartCoroutine (BounceHeart (other.transform.position));
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
			case AI_TYPE.NICE:
				ai_state = AI_STATE.MOVING;
				break;		
			}
			emotion.SetActive (false);
		}
	}
}
