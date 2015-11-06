using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour
{

	MeshRenderer myMeshRenderer;
	public Vector2 offset;
	public Color beamColor = Color.black;
	public bool ignoreEmiss;
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
		if (!ignoreEmiss)
			myMat.SetColor ("_EmissionColor", beamColor);
	}

	/* _Color
 _MainTex
 _Cutoff
 _Glossiness
 _Metallic
 _MetallicGlossMap
 _BumpScale
 _BumpMap
 _Parallax
 _ParallaxMap
 _OcclusionStrength
 _OcclusionMap
 _EmissionColor
 _EmissionMap
 _DetailMask
 _DetailAlbedoMap
 _DetailNormalMapScale
 _DetailNormalMap
 _UVSec
 _EmissionScaleUI
 _EmissionColorUI
 _Mode
 _SrcBlend
 _DstBlend
 _ZWrite
 */
}
