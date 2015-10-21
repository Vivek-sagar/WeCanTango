using UnityEngine;
using System.Collections;

public class XZBillboard : Singleton<XZBillboard> {
	public Camera camera;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Vector3 dir = camera.transform.position - transform.position;
		dir = Vector3.ProjectOnPlane (dir, Vector3.up);
		this.transform.forward = -dir;
	}

	public void changeTexture(Texture tex)
	{
		Material material = GetComponent<MeshRenderer> ().material;
		material.SetTexture ("_MainTex", tex);
	}

	public void hide()
	{
		Material material = GetComponent<MeshRenderer> ().material;
		material.SetFloat ("_Alpha", 0);
	}
	public void show()
	{
		Material material = GetComponent<MeshRenderer> ().material;
		material.SetFloat ("_Alpha", 1);
	}

}
