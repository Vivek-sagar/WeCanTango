using UnityEngine;
using System.Collections;

public class GrowScript : MonoBehaviour {
	VoxelExtractionPointCloud vxe;
	public GameObject obj;
	public ParticleSystem partsys;

	Vector3 pos;
	Vector3 norm;

	bool canGrow = false;

	void Awake()
	{

	}
	// Use this for initialization
	void Start () {
	
	}

	public void init(Vector3 _position, Vector3 _normal, bool CanGrow)
	{
		pos = _position;
		norm = _normal;
		obj.transform.up = norm;
		canGrow = CanGrow;
		this.GetComponent<Collider> ().enabled = canGrow;
		partsys.enableEmission = canGrow;

	}

	void OnTriggerEnter(Collider other) 
	{
		//Debug.Log ("HI");
		if(other.gameObject.tag == "Player")
		{
			obj.transform.localScale = new Vector3 (30, 30, 30);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
