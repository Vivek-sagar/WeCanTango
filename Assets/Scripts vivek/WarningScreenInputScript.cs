using UnityEngine;
using System.Collections;

public class WarningScreenInputScript : MonoBehaviour {

    public GameObject screen1;
    public GameObject screen2;
    public GameObject screen3;
    public GameObject screen4;
    public GameObject startButton;
    public GameObject skipButton;
    GameObject screen;
    private int screenCount = 0;
    private Vector2 touchStartPos;

	// Use this for initialization
	void Start () {
        touchStartPos = new Vector2(0, 0);
	}

    IEnumerator ChangeScene(GameObject screen)
    {
        //startButton.SetActive(false);
        //skipButton.SetActive(true);
        while (screen.transform.position.x < 2.8f)
        {
            screen.transform.Translate(0.1f, 0, 0);
            yield return new WaitForSeconds(0.005f);
        }
        if (screenCount > 2)
        {
            startButton.SetActive(true);
            skipButton.SetActive(false);
        }
        screen.transform.position = new Vector3(2.8f, screen.transform.position.y, screen.transform.position.z);
    }

    IEnumerator ResetScene(GameObject screen)
    {
        if (screen.transform.position.x < -0.1f)
        {
            while (screen.transform.position.x < -0.1f)
            {
                screen.transform.Translate(0.1f, 0, 0);
                yield return new WaitForSeconds(0.005f);
            }
        }
        else if (screen.transform.position.x > 0.1f)
        {
            while (screen.transform.position.x > 0.1f)
            {
                screen.transform.Translate(-0.1f, 0, 0);
                yield return new WaitForSeconds(0.005f);
            }
        }
        if (screenCount <= 2)
        {
            startButton.SetActive(false);
            skipButton.SetActive(true);
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
            case 3:
                screen = screen4;
                break;
            default:
                screen = screen1;
                break;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchStartPos = Input.GetTouch(0).position;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
            if (touchDelta.x < 0 && screen.transform.position.x <= 0f)
            {
                screen.transform.position = new Vector3(0, screen.transform.position.y, screen.transform.position.z);
                if (screenCount > 0)
                {
                    screenCount--;
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
                        case 3:
                            screen = screen4;
                            break;
                        default:
                            screen = screen1;
                            break;
                    }
                    screen.transform.Translate(touchDelta.x * 0.01f, 0, 0);
                }
                
            }
            else
            {
                if (screenCount < 3)
                    screen.transform.Translate(touchDelta.x * 0.01f, 0, 0);
            }
                
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (screenCount < 3)
            {
                if (Input.GetTouch(0).position.x > touchStartPos.x)
                {
                    StartCoroutine("ChangeScene", screen);
                    screenCount++;
                }
                else
                    StartCoroutine("ResetScene", screen);
            }
            
        }
	}

    public void Skip()
    {
        Debug.Log("Skipped");
        Application.LoadLevel(1);
    }

    public void Next()
    {
        screenCount++;
        if (screenCount > 3)
        {
            Skip();
        }
        StartCoroutine("ChangeScene", screen);
    }
}
