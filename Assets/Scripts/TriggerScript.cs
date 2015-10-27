﻿using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour
{

	public bool triggered = false;
	public ParticleSystem partsys;
	public GameObject obj;
	public BoxCollider cubeswitch;
	public PetCounter petcounter;
	private bool isSleeping = true;
	Vector3[] fourpts;
	VoxelExtractionPointCloud vxe;

	CameraClearFlags defaultFlag;
	float defaultLightIntensity;
	// Use this for initialization
	void Start ()
	{
		vxe = VoxelExtractionPointCloud.Instance;
		cubeswitch.gameObject.SetActive (false);
	}


	bool checkForVoxelsInCollider ()
	{
		Vector3 max = cubeswitch.gameObject.transform.position + cubeswitch.bounds.extents;
		Vector3 min = cubeswitch.gameObject.transform.position - cubeswitch.bounds.extents;

		for (float i=min.x; i<=max.x; i+= vxe.voxel_size)
			for (float j=min.y; j<=max.y; j+= vxe.voxel_size)
				for (float k=min.z; k<=max.z; k+= vxe.voxel_size) {
					if (vxe.isVoxelThere (new Vector3 (i, j, k)))
						return true;
				}
		
		return false;
	}


	// Update is called once per frame
	void Update ()
	{
		if (!triggered && cubeswitch.gameObject.activeSelf && checkForVoxelsInCollider ()) {
			triggeredEvent ();
		}

		if (vxe.isVoxelThere (obj.transform.position)) {
			transform.position += Vector3.up * vxe.voxel_size;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (triggered)
			return;
		GameObject othergo = other.gameObject;

		if (othergo.tag == "Pet") {
			//triggeredEvent();
			partsys.Emit (100);
			cubeswitch.gameObject.SetActive (true);
			if (isSleeping) {
				//obj.transform.Translate(new Vector3(-0.06f, -0.06f, 0f));
				obj.transform.Rotate (new Vector3 (-90, 0, 0));
				isSleeping = false;
			}
		}
	}

	void triggeredEvent ()
	{
		partsys.startLifetime = 3;
		partsys.startColor = Color.cyan;
		partsys.startSpeed = 2.0f;
		partsys.startSize = 0.3f;
		partsys.maxParticles = 500;
		partsys.Clear ();
		partsys.Stop ();
		partsys.Emit (500);

		GetComponent<JumpingAI> ().init ();


		PetMessage.Instance.setThankYou ();
		cubeswitch.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		cubeswitch.gameObject.SetActive (false);

		triggered = true;
		petcounter.PetTriggered ();
	}

}
