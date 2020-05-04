using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Text DescriptionText;
    private bool isTouchScreen;
    private bool isSwipeScreen;
    private bool isDetachScreen;

    private Vector3 beganPos;

    private void Start()
    {
        if (GameManager.MapNum == 1)
        {
            DescriptionText.gameObject.SetActive(true);
            DescriptionText.text = descriptions[0];
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            // Input
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !isTouchScreen)
            {
                //Mouse Button Down
                beganPos = Input.mousePosition;
                isTouchScreen = true;
                DescriptionText.text = descriptions[1];
            }

            if ((beganPos - Input.mousePosition).magnitude > 60f && isTouchScreen && !isSwipeScreen)
            {
                isSwipeScreen = true;
                DescriptionText.text = descriptions[2];
            }

            if (Input.GetMouseButtonUp(0) && isTouchScreen && isSwipeScreen && !isDetachScreen)
            {
                isDetachScreen = true;
                DescriptionText.text = descriptions[3];
                StartCoroutine(InactivateDescriptionText());
            }
#elif UNITY_ANDROID
            
            if (Input.touchCount > 0)
            {
                var getTocuh = Input.GetTouch(0);
                var fingerPos = getTocuh.position;
                switch (getTocuh.phase)
                {
                    case TouchPhase.Began:
                        if (!isTouchScreen)
                        {
                            beganPos = fingerPos;
                            isTouchScreen = true;
                            DescriptionText.text = descriptions[1];
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        {
                            if (((Vector2)beganPos - fingerPos).magnitude > 60f && isTouchScreen && !isSwipeScreen)
                            {
                                isSwipeScreen = true;
                                DescriptionText.text = descriptions[2];
                            }
                            break;
                        }
                    case TouchPhase.Ended:
                        if (isTouchScreen && isSwipeScreen && !isDetachScreen)
                        {
                            isDetachScreen = true;
                            DescriptionText.text = descriptions[3];
                            StartCoroutine(InactivateDescriptionText());
                        }
                        break;
                    case TouchPhase.Canceled:
                        break;
                }
            }
#endif
        }
    }

    private List<string> descriptions = new List<string>()
    {
        "화면 아무 곳이나 눌러보세요. 손 떼지마시고!!",
        "그대로 왼쪽으로 끌어보세요.",
        "셀의 몸에서 이동 궤적이 보이나요? 손을 떼보세요.",
        "셀이 점프하였나요? 이제 모험을 떠나보세요!"
    };

    private IEnumerator InactivateDescriptionText()
    {
        yield return new WaitForSeconds(3f);
        DescriptionText.gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
