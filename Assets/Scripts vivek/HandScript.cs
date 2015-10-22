using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class points
{
	public Vector3 position;
	public int classId = 0;

	public points(Vector3 pos, int Id)
	{
		position = pos;
		classId = Id;
	}
}

public class HandScript : Singleton<HandScript>
{

	private Mesh mesh;
	private Renderer renderer;
	private bool isHandVisible;

	private Queue<Vector3> handQueue;
	private Vector3 initialHandObjPos, finalHandObjPos;
	private int numPoints;
	private Vector3 location;
	private bool firstTime = true;

	public GameObject camera;
	public GameObject handObject;
	public GameObject spawnObject;
	public float handRadius = 0.05f;
	const int HAND_POINT_THRESH = 500;

	// Use this for initialization
	void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		firstTime = true;

		handQueue = new Queue<Vector3>();

		renderer = GetComponent<MeshRenderer>();
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 500, 1000, 500), "Points: " + numPoints);
		GUI.Label(new Rect(10, 550, 1000, 500), "Location: " + location);
	}

	// Update is called once per frame
	void Update()
	{
	
	}

	private int cluster(List<points> points, points pt, int numPoints, int id)
	{
		foreach (points point in points) {
			if (point == pt) {
				continue;
			}
			if (point.classId == 0) {
				if (Vector3.Distance(pt.position, point.position) < handRadius) {
					point.classId = id;
					cluster(points, point, numPoints, id);
					numPoints++;
				}
			}
		}
		return numPoints;
	}

	public IEnumerator SpawnBlock()
	{
		while (true) {
			if (isHandVisible) {
				Vector3 vxCoord = new Vector3();
				Vector3 normal = new Vector3();
				float dist = 100;
				if (VoxelExtractionPointCloud.Instance.RayCast(finalHandObjPos, finalHandObjPos - camera.transform.position, dist, ref vxCoord, ref normal)) {
					GameObject newObj = Instantiate(spawnObject, finalHandObjPos, Quaternion.identity) as GameObject;
					newObj.SendMessage("SetTarget", (vxCoord + normal * VoxelExtractionPointCloud.Instance.voxel_size));
					//spawnObject.transform.position = vxCoord;
				}
			}
			yield return new WaitForSeconds(1);
		}
	}

	//public void RenderHand(TangoPointCloud pointCloud)
	//{
	//    List<int> indices = new List<int>();
	//    List<points> closePoints = new List<points>();
        
	//    int idx = 0;
	//    for (int i=0; i<pointCloud.m_pointsCount; i++)
	//    {
	//        if (Vector3.Distance(pointCloud.m_points[i], camera.transform.position) > 0.7f)
	//            continue;
	//        points newPoint = new points(pointCloud.m_points[i] , 0);
	//        closePoints.Add(newPoint);
	//        indices.Add(idx);
	//        idx++;
	//    }
	//    int currId = 1;
	//    int numPoints = cluster(closePoints, closePoints[0], 0, currId);
	//    Debug.Log(numPoints);

	//    List<Vector3> temp = new List<Vector3>();
	//    indices.Clear();
	//    idx = 0;
	//    foreach (points point in closePoints)
	//    {
	//        //renderer.material.SetColor("_Color", Color.red);
	//        if (point.classId == 1)
	//        {
	//            temp.Add(point.position);
	//            indices.Add(idx);
	//            idx++;
	//        }
	//    }
	//    mesh.Clear();
	//    mesh.vertices = temp.ToArray();
	//    mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        
	//}

	public void RenderHand(TangoPointCloud pointCloud)
	{
		if (firstTime) {
			firstTime = false;
			//StartCoroutine(SpawnBlock());
		}
		List<int> closePoints = new List<int>();
		List<Vector3> closePointPos = new List<Vector3>();
		List<int> indices = new List<int>();
		int idx = 0;
		for (int i = 0; i < pointCloud.m_pointsCount; i++) {
			if (Vector3.Distance(pointCloud.m_points [i], camera.transform.position) > 1f) {
				continue;
			}
			closePoints.Add(i);

			indices.Add(idx);
			idx++;
			closePointPos.Add(pointCloud.m_points [i]);
		}
		numPoints = idx;
		mesh.Clear();
		if (numPoints < HAND_POINT_THRESH) {
			isHandVisible = false;
			handObject.SetActive(false);
			return;
		} else {
			isHandVisible = true;
			handObject.SetActive(true);
		}
        
		Vector3 centroid = pointCloud.GetAverageFromFilteredPoints(closePoints);
		//mesh.vertices = closePointPos.ToArray();
		//mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
		if (handQueue.Count > 3) {
			handQueue.Dequeue();
		}
		handQueue.Enqueue(centroid);
		Vector3 acc = new Vector3(0, 0, 0);
		Vector3[] handArray = handQueue.ToArray();
		for (int i=0; i<handQueue.Count; i++) {
			acc += handArray [i];
		}
		acc /= handQueue.Count;
		initialHandObjPos = finalHandObjPos;
		Vector3 snappedPos = VoxelExtractionPointCloud.Instance.FromGrid(VoxelExtractionPointCloud.Instance.ToGrid(acc));
		//for (int i = 0; i < VoxelConsts.PT_THRES; i++ )
		//    VoxelExtractionPointCloud.Instance.grid.setVoxel(VoxelExtractionPointCloud.Instance.ToGrid(acc));
		//Debug.Log(snappedPos.x + " " + snappedPos.y + " " + snappedPos.z);
		finalHandObjPos = acc + new Vector3(0.5f, 0.5f, 0.5f);
		if (initialHandObjPos != finalHandObjPos) {
			StartCoroutine("MoveHand");
		}
		//handObject.transform.position = finalHandObjPos;
	}

	IEnumerator MoveHand()
	{
		for (int i=0; i<4; i++) {
			handObject.transform.position = Vector3.Lerp(initialHandObjPos, finalHandObjPos, (float)i / 2.0f);
			yield return null;
		}
	}
}
