using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunZoom : MonoBehaviour
{
    public float fovTarget;

    void Start()
    {
        fovTarget = zoomOut;
        
        Cursor.lockState = CursorLockMode.Locked; // ESC키를 누르면 해제됨
        Cursor.visible = false;
    }

    public float zoomIn = 10f;
    public float zoomOut = 60f;

    void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fovTarget, Time.deltaTime * 5);

        // 마우스 오른쪽 버튼을 누르고 있으면 ZoomIn이 되고
        if (Input.GetButton("Fire2"))
        {
            Camera.main.fieldOfView = zoomIn;
            fovTarget = zoomIn;
        }

        // 마우스 오른쪽 버튼을 떼면 ZoomOut이 되게 하고 싶다.
        else if (Input.GetButtonUp("Fire2"))
        {
            fovTarget = zoomOut;

        }
    }
}
