using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour
{

	MeshRenderer myMeshRenderer;
	public Vector2 offset;
	Material myMat;
	bool fading = false;
	Vector2 origOffset;
	// Use this for initialization
	void Start ()
	{
		myMeshRenderer = GetComponent<MeshRenderer> ();
		myMat = myMeshRenderer.material;
		origOffset = myMat.GetTextureOffset ("_MainTex");
	}
	
	// Update is called once per frame
	void Update ()
	{
		origOffset += offset * Time.deltaTime;
		myMat.SetTextureOffset ("_MainTex", origOffset);
	}
}
