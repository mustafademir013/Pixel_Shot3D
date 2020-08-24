using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Camera.main.aspect);
        if (Camera.main.aspect < 0.51f)
            Camera.main.fieldOfView = Camera.main.fieldOfView + 8;
    }
}
