using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLazer : MonoBehaviour
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
		hit_mat = hitMeshRenderer.material;
		hitSound = GetComponent<AudioSource> ();
		myCollider = GetComponent<Collider> ();
	}

	IEnumerator fadeToDeathScreen ()
	{
		hitSound.Play ();
		Color red = new Color (1f, 0, 0, 0.5f);
		float time = 0f;
		fading = true;
		while (time <1f) {
			time += rate * Time.deltaTime;
			hit_mat.color = Color.Lerp (hit_mat.color, red, time);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		time = 0f;
		Color blank = new Color (0f, 0f, 0f, 0f);

		while (time <1f) {
			time += rate * Time.deltaTime;
			hit_mat.color = Color.Lerp (hit_mat.color, blank, time);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		fading = false;
	}

	void OnTriggerStay (Collider other)
	{
		if (other.CompareTag ("Bull")) {
			if (!fading)
				StartCoroutine (fadeToDeathScreen ());
			//hitSound.PlayOneShot (hitSound.clip);

		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Bull")) {
			hitSound.Stop ();
		}
	}
}
