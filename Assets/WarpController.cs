using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	public Vector2 offset; 
	public ScrollTexture[] scrollTextureList; 
	Animator myAnim;
	// Use this for initialization
	void Start ()
	{
		myAnim = GetComponent<Animator> ();
		foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
		}
	}

	void Update ()
	{
		foreach (ScrollTexture sc in scrollTextureList) {
			sc.offset = offset;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Player")) {
			myAnim.SetTrigger ("PortalOn");
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Player")) {
			myAnim.SetTrigger ("StopPortal");
		}
	}


}
