using UnityEngine;
using System.Collections;

public class WarningScreenInputScript : MonoBehaviour {

    public GameObject screen1;
    public GameObject screen2;
    public GameObject screen3;
    GameObject screen;
    private int screenCount = 0;

	// Use this for initialization
	void Start () {
	
	}

    IEnumerator ChangeScene(GameObject screen)
    {
        while (screen.transform.position.x < 2.8f)
        {
            screen.transform.Translate(0.1f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ResetScene(GameObject screen)
    {
        while (screen.transform.position.x < -0.1f)
        {
            screen.transform.Translate(0.1f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        screen.transform.position = new Vector3(0, screen.transform.position.y, screen.transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        
        switch (screenCount)
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
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
            if (touchDelta.x < 0 && screen.transform.position.x <= 0f)
                screen.transform.position = new Vector3(0, screen.transform.position.y, screen.transform.position.z);
            else
                screen.transform.Translate(touchDelta.x * 0.01f, 0, 0);
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (screen.transform.position.x > 0)
            {
                StartCoroutine("ChangeScene", screen);
                screenCount++;
            }
            else
                StartCoroutine("ResetScene", screen);
            
        }
	}

    public void Skip()
    {
        Debug.Log("Skipped");
    }

    public void Next()
    {
        screenCount++;
        if (screenCount > 2)
        {
            Skip();
        }
        StartCoroutine("ChangeScene", screen);
    }
}
