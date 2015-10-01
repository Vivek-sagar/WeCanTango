using UnityEngine;
using System.Collections;
using Tango;
public class CameraEmulation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		#if !UNITY_ANDROID || UNITY_EDITOR
		Vector3 tempPosition = transform.position;
		Quaternion tempRotation = transform.rotation;
		PoseProvider.GetMouseEmulation(ref tempPosition, ref tempRotation);
		Camera.main.transform.rotation = tempRotation;
		Camera.main.transform.position = tempPosition;

		
		#endif
		

	}
}
