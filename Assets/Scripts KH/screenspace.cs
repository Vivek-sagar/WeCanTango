using UnityEngine;
using System.Collections;

public class screenspace : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera cam = Camera.main;
		
		float pos = (cam.nearClipPlane + 0.01f);
		
		transform.position = cam.transform.position + cam.transform.forward * pos;
		
		float h = Mathf.Tan(cam.fov*Mathf.Deg2Rad*0.5f)*pos*2f;
		
		transform.localScale = new Vector3(h*cam.aspect,h,0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
