using UnityEngine;
using System.Collections;

public class JumpingAI : MonoBehaviour {
	VoxelExtractionPointCloud vxe;
	AI_STATE ai_state;
	Vector3 lastposition;
	Vector3 jumpPosition;
	Vector3 fallPosition;
	Vector3 movePosition;
	public Camera camera;
	static Vector3[] directions = { new Vector3(0,0,1),new Vector3(0,0,-1),new Vector3(1,0,0),new Vector3(-1,0,0),new Vector3(0,1,0),new Vector3(0,-1,0) };
	const int STEP = 5;
	const int BIG_STEP = 100;
	float stepdownThreshold;
	public bool start = false;
	// Use this for initialization
	void Start () {
		ai_state = AI_STATE.STOPPED;
		vxe = VoxelExtractionPointCloud.Instance;
		lastposition = new Vector3 ();
		stepdownThreshold = vxe.voxel_size;
		stepdownThreshold = stepdownThreshold * stepdownThreshold;
		init ();
	}

	
	Vector3 getNextMove()
	{
		Vector3 coords = Vector3.zero;
		Vector3 norm = Vector3.zero;
		

		Vector3 dir = Vector3.ProjectOnPlane((camera.transform.position - transform.position),Vector3.up).normalized;
		bool hit = vxe.RayCast (transform.position, dir, STEP, ref coords, ref norm, 0.5f);

		if(!hit)
		{
			bool isThereGround = vxe.RayCast (coords, Vector3.down, BIG_STEP, ref fallPosition, ref norm, 0.5f);

			if(isThereGround)
			{
				if((coords - fallPosition).sqrMagnitude < stepdownThreshold)
				{
					movePosition = coords;
					ai_state = AI_STATE.MOVING;
				}
				else
				{
					fallPosition += Vector3.up * vxe.voxel_size;
					ai_state = AI_STATE.FALLING;
				}

				return dir;
			}
			/*
			else
			{
				movePosition = coords;
				ai_state = AI_STATE.MOVING;
				return dir;
			}*/
		}
		else
		{
			bool isThereSurface = vxe.OccupiedRayCast (coords, Vector3.up, BIG_STEP, ref jumpPosition, ref norm);
			if(isThereSurface)
			{
				jumpPosition += Vector3.up * vxe.voxel_size;
				ai_state = AI_STATE.JUMPING;
				return dir;
			}
		}

		ai_state = AI_STATE.STOPPED;
		return Vector3.zero;
		
	}

	Vector3 getQuadBeizierPt(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		float oneMinusT = (1 - t);
		return oneMinusT * oneMinusT * p0 + 2 * oneMinusT * t * p1 + t * t * p2;
	}


	IEnumerator moveit()
	{
		while(true)
		{	
			Vector3 dir = getNextMove ();
			transform.forward = dir;
			Vector3 currentPosition = transform.position;

			switch(ai_state)
			{
			case AI_STATE.MOVING:
				for(float i=0; i< 1.0f; i+=Time.deltaTime * 0.2f)
				{
					transform.position = Vector3.Lerp(currentPosition,movePosition,i);
					yield return null;
				}
				transform.position = movePosition;
				yield return new WaitForSeconds(2.0f);
				break;
			case AI_STATE.JUMPING:
				Vector3 highpt = (currentPosition + jumpPosition) * 0.5f;
				highpt.y += vxe.voxel_size * 7.0f;
				for(float i=0; i< 1.0f; i+=Time.deltaTime * 0.2f)
				{
					transform.position = getQuadBeizierPt(currentPosition,highpt,jumpPosition,i);//Vector3.Lerp(currentPosition,jumpPosition,i);
					yield return null;
				}
				transform.position = jumpPosition;
				yield return new WaitForSeconds(2.0f);
				break;
			case AI_STATE.FALLING:
				for(float i=0; i< 1.0f; i+=Time.deltaTime * 0.2f)
				{
					transform.position = Vector3.Lerp(currentPosition,fallPosition,i);
					yield return null;
				}
				transform.position = fallPosition;
				yield return new WaitForSeconds(2.0f);
				break;
			case AI_STATE.STOPPED:
			default:
				yield return new WaitForSeconds(2.0f);
				break;
			}
		}
	}

	
	public void init()
	{
		ai_state = AI_STATE.MOVING;
		StartCoroutine (moveit ());
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{
		switch(ai_state)
		{
		case AI_STATE.MOVING:
			GUI.Label (new Rect (1000,10,100,100),"MOVING");
			//Debug.Log ("MOVING");
			break;
		case AI_STATE.JUMPING:
			GUI.Label (new Rect (1000,10,100,100),"JUMPING");
			//Debug.Log ("JUMPING");
			break;
		case AI_STATE.FALLING:
			GUI.Label (new Rect (1000,10,100,100),"FALLING");
			//Debug.Log ("FALLING");
			break;
		case AI_STATE.STOPPED:
			GUI.Label (new Rect (1000,10,100,100),"STOPPED");
			//Debug.Log ("STOPPED");
			break;
		default:
			break;
			
		}

	}
}
