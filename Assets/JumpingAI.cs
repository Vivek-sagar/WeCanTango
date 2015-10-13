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
	ItemSpawner itemspawn;
	static Vector3[] directions = { new Vector3(0,0,1),new Vector3(0,0,-1),new Vector3(1,0,0),new Vector3(-1,0,0),new Vector3(0,1,0),new Vector3(0,-1,0) };
	const int STEP = 5;
	const int BIG_STEP = 64;
	float stepdownThreshold;


	const int MAX_JUMP_HEIGHT = 3;
	const int JUMP_RANGE = 3;
	// Use this for initialization
	void Start () {
		ai_state = AI_STATE.STOPPED;
		vxe = VoxelExtractionPointCloud.Instance;
		itemspawn = ItemSpawner.Instance;
		lastposition = new Vector3 ();
		stepdownThreshold = vxe.voxel_size * 2;
		stepdownThreshold = stepdownThreshold * stepdownThreshold;
		init ();
	}

	bool checkForJumpPositions(Vector3 dir, out Vector3 out_minAngleDir)
	{
		Vec3Int jumpCoords = vxe.ToGrid (transform.position);
		
		bool canJump = false;
		float maxdotprod = float.MinValue;
		Vector3 minAngleDir = Vector3.zero;
		
		for(int i=-JUMP_RANGE;i<=JUMP_RANGE;i++)
			for(int j=-JUMP_RANGE;j<=JUMP_RANGE;j++)
				for(int k=MAX_JUMP_HEIGHT;k>=1;k--)
			{
				Vec3Int vcoords = jumpCoords + new Vec3Int(i,k,j);
				Voxel vx = vxe.grid.getVoxel(vcoords);
				if(vx.isOccupied() &&  vxe.voxelHasSurface(vx,VF.VX_TOP_SHOWN))
				{
					Vector3 wrldcoords = vxe.FromGridUnTrunc(vcoords.ToVec3() + new Vector3(0.5f,1.0f,0.5f));
					Vector3 vdir = Vector3.ProjectOnPlane((wrldcoords - transform.position),Vector3.up).normalized;
					float dotprod = Vector3.Dot(dir,vdir);
					if(dotprod > 0)
					{
						canJump = true;
						if(dotprod > maxdotprod)
						{
							maxdotprod = dotprod;
							minAngleDir = vdir;
							jumpPosition = wrldcoords;
						}
					}
				}
			}
		


			out_minAngleDir = minAngleDir;
			return canJump;

	}


	Vector3 getNextMoveLimited()
	{
		Vector3 targetPosition = camera.transform.position;

		for(int i=0;i<itemspawn.items.Length;i++)
		{
			if(itemspawn.spawneditems[i] == null || itemspawn.spawneditems[i].GetComponent<TriggerScript>().triggered)
				continue;

			float groundLength = (itemspawn.spawneditems[i].transform.position - transform.position).magnitude;
			if( groundLength < vxe.voxel_size * 20 )
			{
				targetPosition = itemspawn.spawneditems[i].transform.position;
				break;
			}
		}


		Vector3 dir = Vector3.ProjectOnPlane((targetPosition - transform.position),Vector3.up).normalized;

		Vector3 coords = Vector3.zero;
		Vector3 norm = Vector3.zero;
		bool notgrounded = false;

		bool hit = vxe.GroundedRayCast (transform.position, dir, STEP, ref coords, ref norm, ref notgrounded, 0.5f);


		Vector3 jumpDir = Vector3.zero;
		bool canJump = checkForJumpPositions (dir, out jumpDir);
		float jumpPositionToTarget = (jumpPosition - targetPosition).sqrMagnitude;
		float movePositionToTarget = (coords - targetPosition).sqrMagnitude;

		if(canJump && jumpPositionToTarget < movePositionToTarget)
		{
			ai_state = AI_STATE.JUMPING;
			return jumpDir;
		}


		if(!hit)
		{
			if(notgrounded)
			{
				bool isThereGround = vxe.RayCast (coords, Vector3.down, BIG_STEP, ref fallPosition, ref norm, 0.5f);
				if(isThereGround)
				{
					fallPosition += Vector3.up * vxe.voxel_size * 0.5f;
					ai_state = AI_STATE.FALLING;
					return dir;
				}			
			}
			else
			{
				movePosition = coords;
				ai_state = AI_STATE.MOVING;
				return dir;
			}
		}
		else
		{
			bool isThereSurface = vxe.OccupiedRayCast (coords, Vector3.up, JUMP_RANGE, ref jumpPosition, ref norm);
			if(isThereSurface)
			{
				jumpPosition -= Vector3.up * vxe.voxel_size * 0.5f;
				ai_state = AI_STATE.JUMPING;
				return dir;
			}
		}

		ai_state = AI_STATE.STOPPED;
		return dir;

	}


	Vector3 getNextMove()
	{
		Vector3 coords = Vector3.zero;
		Vector3 norm = Vector3.zero;
		bool notgrounded = false;

		Vector3 dir = Vector3.ProjectOnPlane((camera.transform.position - transform.position),Vector3.up).normalized;
		bool hit = vxe.GroundedRayCast (transform.position, dir, STEP, ref coords, ref norm, ref notgrounded, 0.5f);

		if(!hit)
		{
			if(notgrounded)
			{
				bool isThereGround = vxe.RayCast (coords, Vector3.down, BIG_STEP, ref fallPosition, ref norm, 0.5f);
				if(isThereGround)
				{
					fallPosition += Vector3.up * vxe.voxel_size * 0.5f;
					ai_state = AI_STATE.FALLING;
					return dir;
				}			
			}
			else
			{
					movePosition = coords;
					ai_state = AI_STATE.MOVING;
					return dir;
			}
		}
		else
		{
			bool isThereSurface = vxe.OccupiedRayCast (coords, Vector3.up, BIG_STEP, ref jumpPosition, ref norm);
			//bool ICannotJump = vxe.CheapRayCast(transform.position,Vector3.up, 5);
			if(isThereSurface)
			{
				jumpPosition -= Vector3.up * vxe.voxel_size * 0.5f;
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
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(1.0f);
		while(true)
		{	
			Vector3 dir = getNextMoveLimited ();
			transform.forward = dir;
			Vector3 currentPosition = transform.position;

			switch(ai_state)
			{
			case AI_STATE.MOVING:
				for(float i=0; i< 1.0f; i+=Time.deltaTime * 0.5f)
				{
					transform.position = Vector3.Lerp(currentPosition,movePosition,i);
					yield return null;
				}
				transform.position = movePosition;
				yield return new WaitForSeconds(0.5f);
				break;
			case AI_STATE.JUMPING:
				{
					Vector3 highpt = (currentPosition + jumpPosition) * 0.5f;
					highpt.y += vxe.voxel_size * 7.0f;
					for(float i=0; i< 1.0f; i+=Time.deltaTime * 0.75f)
					{
						transform.position = getQuadBeizierPt(currentPosition,highpt,jumpPosition,i);
						yield return null;
					}
					transform.position = jumpPosition;
					yield return new WaitForSeconds(1.0f);
				}
				break;
			case AI_STATE.FALLING:
				{
					Vector3 highpt = (currentPosition + fallPosition) * 0.5f;
					highpt.y += vxe.voxel_size * 5.0f;
					for(float i=0; i< 1.0f; i+=Time.deltaTime * 0.75f)
					{
						transform.position = getQuadBeizierPt(currentPosition,highpt,fallPosition,i);
						yield return null;
					}
					transform.position = fallPosition;
					yield return new WaitForSeconds(1.0f);
				}
				break;
			case AI_STATE.STOPPED:
			default:
				yield return new WaitForSeconds(0.5f);
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
