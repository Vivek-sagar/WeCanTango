using UnityEngine;
using System.Collections;

public class AIBehaviour : MonoBehaviour
{
	public float jumpHeight = 3f, jumpSpeed = 0.5f;
	Transform myTrans;
	Vector3 startJump, direction;

	public Voxel currentVoxel;

	public enum AI_State
	{
		None,
		Idle,
		NoVoxel,
		SeeVoxel,
		Jump
	}
	;
	public AI_State myState;

	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		startJump = myTrans.position;
		myState = AI_State.None;
		direction = Vector3.one;
		 
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (myState) {
		case AI_State.Idle:
			break;
		case AI_State.Jump:
			float gravity = 1f;
			if (direction.y < 0)
				gravity = 1f;

			myTrans.position += Vector3.up * direction.y * gravity * jumpSpeed * Time.deltaTime;
			keepInBounds ();
			if (myTrans.position.y >= startJump.y + jumpHeight || myTrans.position.y <= 0f)
				direction.y *= -1;
			break;
		}
	
	}

	void DetectVoxels ()
	{
		if (currentVoxel == null)
			currentVoxel = new Voxel ();


	}


	void keepInBounds ()
	{
		if (myTrans.position.y > startJump.y + jumpHeight)
			myTrans.position = new Vector3 (myTrans.position.x, startJump.y + jumpHeight, myTrans.position.z);
		else if (myTrans.position.y < 0f)
			myTrans.position = new Vector3 (myTrans.position.x, 0f, myTrans.position.z);
	}
}
