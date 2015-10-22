using UnityEngine;
using System.Collections;

public class LazerScript : MonoBehaviour
{
	GameObject lazerPointPrefab;
	public Transform startPt, endPt;
	public float textureScrollSpeed = 1f;
	public float min, max;
	Vector3 lazerDirection;
	public MeshRenderer myMat;
	Transform particleTransform;
	Transform myTrans;
	ParticleSystem myParticleSystem;
	Material texture;
	Collider myCollider;
	//Current XOffset for Texture
	float yOffSet = 0f, moveTimer = 0f, startTimer = 0f, startMax;
	public Vector3 moveSpeed;
	public bool canMove = true;
	bool hitPlayer;
	bool isVert;
	//To be used to increase particle life depending on size
	float defaultStartLife;

	// Use this for initialization
	void Start ()
	{
		isVert = Mathf.Abs (moveSpeed.y) > 0;
		myTrans = GetComponent<Transform> ();
		myCollider = GetComponent<Collider> ();
		texture = myMat.material;
		//Get the Particle Sytems transform
		//particleTransform = GetComponentInChildren<Transform> ();
		//myParticleSystem = GetComponentInChildren<ParticleSystem> ();
		startMax = Random.Range (0f, 2f);
		if (startPt == null || endPt == null) {	
			//		myParticleSystem.gameObject.SetActive (false);

			StartCoroutine (CreateStartPoint ());
		} else {
			//	particleTransform.position = startPt.position;
			//	particleTransform.LookAt (endPt);

			//work on formula to determine how fast and far particles should fly
			//		myParticleSystem.startSpeed = (startPt.position - endPt.position).magnitude * 5f;

			//	myCollider.height = (endPt.position - startPt.position).magnitude;

			/*lazerDirection = (endPt.position - startPt.position).normalized;

			//myCollider.center = Vector3.forward * (myCollider.height / 2f);

			if (lazerDirection == startPt.up) {
				moveSpeed = startPt.right;
			} else {
				moveSpeed = startPt.up;

			}*/
			/*else if (direction.y > 0)
				myCollider.center = Vector3.Cross ((endPt.position - startPt.position), Vector3.) / 2f;*/
			//use DIRECTION TO CONTROL WHEATHER LASER IS VERTICAL or horizontal (Check already done, now Must center them)
		}
	}

	void OnTriggerStay (Collider other)
	{
		if (other.CompareTag ("Player")) {
			hitPlayer = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Player")) {
			hitPlayer = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (startPt == null || endPt == null) {
			return;
		}
		if (startTimer > startMax)
			canMove = true;
		else
			startTimer += Time.deltaTime;

		if (canMove)
			slideMovement ();
		/*particleTrans.LookAt (endPt);



//		collider.height = (startPt.position - endPt.position).magnitude;*/
		yOffSet += textureScrollSpeed * Time.deltaTime;
		if (yOffSet > 10)
			yOffSet = 0;
		texture.SetTextureOffset ("_MainTex", Vector2.up * yOffSet * Time.deltaTime);
	}

	/// <summary>
	/// Creates the start point, and then raycasts to find an end point.
	/// </summary>
	/// <param name="startVec3">Start vec3.</param>
	public IEnumerator CreateStartPoint ()
	{
		while (startPt ==null) {
			yield return null;
		}
		/*	endPt = new GameObject ("LazerEndPt").transform;//(Instantiate (lazerPointPrefab, Vector3.zero, Quaternion.identity) as GameObject).transform;
		//new GameObject ("LazerEndPt").transform;
		endPt.position = startPt.position + lazerDirection * 5f;

		//myParticleSystem.gameObject.SetActive (true);

		//particleTransform.position = startPt.position;
		//particleTransform.LookAt (endPt);

		//myParticleSystem.startSpeed = (startPt.position - endPt.position).magnitude * 5f;
		//myCollider.height = (startPt.position - endPt.position).magnitude;
		//myCollider.center = Vector3.forward * (myCollider.height / 2f);

		if (lazerDirection == startPt.up) {
			moveSpeed = startPt.right;
		} else {
			moveSpeed = startPt.up;
			
		}*/
		//StartCoroutine (SetEndPt ());
	}

	void slideMovement ()
	{

		moveTimer += Time.deltaTime;
		//startPt.position = Vector3.SmoothDamp (startPt.position, target, ref moveSpeed, 1f);
		//endPt.position = Vector3.SmoothDamp (endPt.position, target, ref moveSpeed, 1f);
		myTrans.localPosition += moveSpeed * Time.deltaTime; 
		/*startPt.position += moveSpeed * Time.deltaTime; 
		endPt.position += moveSpeed * Time.deltaTime; 
*/
		//either -+ x or -+y
		//Mathf.Approximately (myTrans.position.y, target.y);
		if (isVert && (myTrans.localPosition.y < min || myTrans.localPosition.y > max)) {
			moveSpeed *= -1f;
			if (myTrans.localPosition.y < min)
				myTrans.localPosition = new Vector3 (0, min + 0.2f, 0);
			else
				myTrans.localPosition = new Vector3 (0, max - 0.2f, 0);
			/*moveTimer = 0f;
			if(&& Mathf.Approximately (myTrans.position.y, target.y)) {
			moveSpeed *= -1f;
			target.y *= -1f;
			}
		} else if (lazerDirection == startPt.up && Mathf.Approximately (myTrans.position.x, target.x)) {
			moveSpeed *= -1f;
			target.x *= -1f;*/
		} else if ((myTrans.localPosition.z < min || myTrans.localPosition.z > max)) {
			moveSpeed *= -1f;
			if (myTrans.localPosition.z < min)
				myTrans.localPosition = new Vector3 (0, myTrans.localPosition.y, min + 0.2f);
			else
				myTrans.localPosition = new Vector3 (0, myTrans.localPosition.y, max - 0.2f);
		}
		//Vector3.SmoothDamp (myTrans.position, target, ref moveSpeed, 1f);

	}

}
