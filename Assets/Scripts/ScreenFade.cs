using UnityEngine;
using System.Collections;

public class ScreenFade : MonoBehaviour
{

	AudioSource hitSound;
	public MeshRenderer hitMeshRenderer;
	public float rate = 2.5f;
	Material hit_mat;
	Collider myCollider;
	bool fading = false;
	// Use this for initialization
	void Start ()
	{
		hitMeshRenderer.enabled = false;
		hit_mat = hitMeshRenderer.material;
		hitSound = GetComponent<AudioSource> ();
		myCollider = GetComponent<Collider> ();
	}

	/// <summary>
	/// Check if it is fading.
	/// </summary>
	/// <returns><c>true</c>, if fading was ised, <c>false</c> otherwise.</returns>
	public bool isFading ()
	{
		return fading;
	}
	
	public IEnumerator doColorFade (Color color, float fadeTime =1f)
	{
		
		//hitSound.Play ();
		float time = 0f;
		hitMeshRenderer.enabled = true;
		while (time <fadeTime) {
			time += rate * Time.deltaTime;
			hit_mat.color = Color.Lerp (hit_mat.color, color, time / fadeTime);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		time = 0f;
		Color blank = new Color (0f, 0f, 0f, 0f);
		while (time <fadeTime) {
			time += rate * Time.deltaTime;
			hit_mat.color = Color.Lerp (hit_mat.color, blank, time / fadeTime);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		hitMeshRenderer.enabled = false;
	}
	
	public IEnumerator resetFade (float fadeTime =1f)
	{
		//hitSound.Play ();
		float time = 0f;
		Color blank = new Color (0f, 0f, 0f, 0f);
		while (time <fadeTime) {
			time += rate * Time.deltaTime;
			hit_mat.color = Color.Lerp (hit_mat.color, blank, time / fadeTime);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		hitMeshRenderer.enabled = false;
	}
}
