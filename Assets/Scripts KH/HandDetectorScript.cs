using UnityEngine;
using System.Collections;

public class HandDetectorScript : Singleton<HandDetectorScript> {

	public IndexStack<Vector3> pointStack;
	Vector3[] pointArr;
	Vector3 centroid;
	public GameObject controlObj;

	// Use this for initialization
	void Start () {
		pointArr = new Vector3[1000];
		pointStack = new IndexStack<Vector3> (pointArr);
		centroid = new Vector3 (0, 0, 1);
		controlObj.GetComponent<MeshRenderer>().enabled = false;
	}

	public void addPoint(Vector3 pt)
	{
		if(pointStack.getCount() < pointArr.Length)
		{
			pointStack.push (pt);
		}
	}

	public void calculatePosition()
	{
		Vector3 center = new Vector3 ();

		int count = pointStack.getCount ();

		if (count < 5)
		{
			controlObj.GetComponent<MeshRenderer>().enabled = false;
			return;
		}
		else
			controlObj.GetComponent<MeshRenderer>().enabled = true;

		for(int i=0;i<count;i++)
		{
			Vector3 p = pointStack.peek (i);
			center += p;
		}
		pointStack.clear ();
		centroid = center / count;
	}

	// Update is called once per frame
	void Update () {
		controlObj.transform.position = centroid;
	}
}
