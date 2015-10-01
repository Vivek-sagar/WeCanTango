using UnityEngine;
using System.Collections;

public class OrientationTrigger : MonoBehaviour {

	public bool triggered = false;

	void OnTriggerEnter(Collider other) 
	{
		if (triggered)
			return;

		if(other.gameObject.tag == "Player")
		{
			this.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.cyan );
			triggered = true;
		}
	}

}
