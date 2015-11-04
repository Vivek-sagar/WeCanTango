using UnityEngine;
using System.Collections;

public class WarningScreenInputScript : MonoBehaviour {

    public GameObject screen1;
    public GameObject screen2;
    public GameObject screen3;
    private int screenCount = 0;

	// Use this for initialization
	void Start () {
	
	}

    IEnumerator ChangeScene(int count)
    {
        GameObject screen;
        switch (count)
        {
            case 0:
                screen = screen1;
                break;
            case 1:
                screen = screen2;
                break;
            case 2:
                screen = screen3;
                break;
            default:
                screen = screen1;
                break;
        }
        while (screen.transform.position.x < 2.8f)
        {
            screen.transform.Translate(0.1f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
            if (touchDelta.x > 0)
                screen1.transform.Translate(touchDelta.x*0.01f, 0, 0);
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            StartCoroutine("ChangeScene", screenCount);
            screenCount++;
        }
	}
}
