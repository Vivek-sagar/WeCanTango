using UnityEngine;
using System.Collections;

public class WaterAnimationScript : MonoBehaviour 
{
	Renderer renderer;
	Vector2 offset;
	public Material material;
	// Use this for initialization
	void Start () {
		offset = new Vector2 (0, 0);
		StartCoroutine (WaterMoving ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator WaterMoving()
	{
		while(true)
		{
			offset = offset + new Vector2(Time.deltaTime * 0.2f, Time.deltaTime * 0.2f);
			material.SetTextureOffset("_MainTex", offset); 

			yield return null;
		}
	}	
}
