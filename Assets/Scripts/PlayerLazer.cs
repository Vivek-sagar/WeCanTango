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
	public float timerStartFade = 0f;
	// Use this for initialization
	void Start ()
	{
		hit_mat = hitMeshRenderer.material;
		hitSound = GetComponent<AudioSource> ();
		myCollider = GetComponent<Collider> ();
	}

	public IEnumerator fadeToDeathScreen (Color color, float fadeTime =1f)
	{

		//hitSound.Play ();
		float time = 0f;

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
	}

	/*void OnTriggerStay (Collider other)
	{
		if (other.CompareTag ("Bull")) {
			if (!fading)
				StartCoroutine (fadeToDeathScreen (new Color(1f, 0, 0, 0.5f)));
			//hitSound.PlayOneShot (hitSound.clip);
		}
		if (other.CompareTag ("Portal")) {
			timerStartFade += Time.deltaTime;
			if (!fading && timerStartFade > .5f) {
				fading = true;
				StartCoroutine (fadeToDeathScreen (new Color (0f, 181 / 255, 1f, 0.95f), 3f));
			}
			//hitSound.PlayOneShot (hitSound.clip);
		}
	}*/

	/*void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Bull")) {
			hitSound.Stop ();
		}
		if (other.CompareTag ("Portal")) {
			fading = false;
			timerStartFade = 0f;
			StartCoroutine (resetFade (0.5f));

		}
	}*/
}
