using UnityEngine;
using System.Collections;

public class WarpController : MonoBehaviour
{
	public Vector2 offset; 
	public ScrollTexture[] scrollTextureList; 
	// Use this for initialization
	void Start ()
	{
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

}
