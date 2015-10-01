using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {
	BiomeScript biome;
	public Material tranformMat;
	bool triggered = false;
	public Camera leftcam;
	public Camera rightcam;
	public Light light;
	public ParticleSystem partsys;
	public GameObject obj;

	CameraClearFlags defaultFlag;
	float defaultLightIntensity;
	// Use this for initialization
	void Start () {
		biome = BiomeScript.Instance;
		defaultFlag = leftcam.clearFlags;
		defaultLightIntensity = light.intensity;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if (triggered)
			return;
		GameObject othergo = other.gameObject;

		if (othergo.tag == "Player") 
		{
			partsys.startLifetime = 3;
			partsys.startColor = Color.cyan;
			partsys.startSpeed = 2.0f;
			partsys.startSize = 0.3f;
			partsys.maxParticles = 500;
			partsys.Clear ();
			partsys.Stop ();
			partsys.Emit (500);
			obj.SetActive (false);

			StartCoroutine (worldTransform ());
			triggered = true;
		}
	}

	IEnumerator worldTransform()
	{
		biome.setAllMaterials (tranformMat);
		leftcam.clearFlags = CameraClearFlags.Skybox;
		rightcam.clearFlags = CameraClearFlags.Skybox;
		light.intensity = 1.0f;
		yield return new WaitForSeconds(10.0f);
		leftcam.clearFlags = defaultFlag;
		rightcam.clearFlags = defaultFlag;
		light.intensity = defaultLightIntensity;
		biome.resetBiomes ();
	}
}
