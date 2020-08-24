using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //touch began or end value 
    bool touchActive = false;

    // began touch position value 
    Vector3 mouseDownPos;

    //direction of the ball value  
    Vector3 direction;

    //Began and end touch position difference value 
    Vector3 distance;

    //Ball speed 
    float speed;

    //spawner object 
    public GameObject ballSpawnArea;

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance.GetLevelMode() == LevelMode.LOAD)
        {

#if UNITY_EDITOR
            if (!touchActive)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    BeganTouch(Input.mousePosition);
            }
            if (touchActive)
            {
                DragBall(Input.mousePosition);

                if (Input.GetKeyUp(KeyCode.Mouse0))
                    UpTouch();
            }

#elif UNITY_ANDROID || UNITY_IOS

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (!touchActive)
                        {
                            BeganTouch(touch.position);
                        }
                        break;
                    case TouchPhase.Moved:
                        if (touchActive)
                        {
                            DragBall(touch.position);
                        }
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        if (touchActive)
                            UpTouch();
                        break;
                    case TouchPhase.Canceled:

                        break;
                    default:
                        break;
                }
            }
#endif
        }
    }
    //Work when touch began 
    void BeganTouch(Vector3 beganPosition)
    {
        touchActive = true;
        mouseDownPos = beganPosition;
    }
    // Move Ball When Dragging
    void DragBall(Vector3 touchPosition)
    {
        distance = touchPosition - mouseDownPos;
        direction = distance.normalized;
        speed = distance.magnitude / 10;
        if (direction.y > 0)
            direction.y = 0;
        if (speed < 30)
        {
            Vector3 pos = ballSpawnArea.transform.position + (direction * speed) / 30;
            BallController.SetPosition(pos);
            PredictionManager.instance.Predict(BallController.currentBall, -direction * speed);
        }

        else
        {
            Vector3 pos = ballSpawnArea.transform.position + direction;
            BallController.SetPosition(pos);
            PredictionManager.instance.Predict(BallController.currentBall, -direction * speed);
        }

    }
    //Work when touch up
    void UpTouch()
    {
        if (speed < 10)
            BallController.SetPosition(ballSpawnArea.transform.position);
        else if (speed > 30)
        {
            speed = 30;
            BallController.SetActiveCollider(true);
            BallController.AddForce(-direction * speed);
            LevelManager.instance.CreateBall();
        }
        else
        {
            BallController.SetActiveCollider(true);
            BallController.AddForce(-direction * speed);
            LevelManager.instance.CreateBall();
        }
        PredictionManager.instance.DeletePredict();

        touchActive = false;
    }

}