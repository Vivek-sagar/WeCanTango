using UnityEngine;
using System.Collections;

public class SpawnBlockScript : MonoBehaviour {

    public float speed = 5.0f;
    private Vector3 destination;
    private float startTime;
    private Vector3 direction;

    public GameObject camera;



	// Use this for initialization
	void Start () {
        startTime = Time.time;
        direction = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(speed * transform.forward * Time.deltaTime);
        //if (Vector3.Magnitude(destination - transform.position) < 1f)
        //{
        //    Destroy(gameObject);
        //}
        if (Time.time > startTime + 5)
            Destroy(gameObject);
	}

    public void SetTarget(Vector3 dest)
    {
        destination = dest;
        transform.forward = destination - camera.transform.position;//transform.LookAt(dest);
        direction = Vector3.Normalize(destination-transform.position);
    }
}
