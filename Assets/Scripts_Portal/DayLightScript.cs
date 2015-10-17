using UnityEngine;
using System.Collections;

public class DayLightScript : MonoBehaviour
{

	public Vector3 dayVector, nighVector;
	public float dayTime = 1000;
	Vector3 currentEular;
	Transform myTrans;
	public float rate, totalDayTime;
	bool isDay;
	// Use this for initialization
	void Start ()
	{
		myTrans = GetComponent<Transform> ();
		rate = 1 / dayTime;
		dayVector = myTrans.eulerAngles;
		currentEular = nighVector;
	}
	
	// Update is called once per frame
	void Update ()
	{
		isDay = totalDayTime < dayTime;
		totalDayTime += rate;
		//if (isDay)
		myTrans.eulerAngles = Vector3.Slerp (myTrans.eulerAngles, currentEular, totalDayTime);
		/*else {
			myTrans.eulerAngles = Vector3.Slerp (myTrans.eulerAngles, dayVector, totalDayTime);
		}*/

		if (totalDayTime > dayTime) {
			totalDayTime = 0f;
			currentEular *= -1f;
		}
	}
}
