using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunZoom : MonoBehaviour
{
    public float fovTarget;

    void Start()
    {
        fovTarget = zoomOut;
        
        Cursor.lockState = CursorLockMode.Locked; // ESCŰ�� ������ ������
        Cursor.visible = false;
    }

    public float zoomIn = 10f;
    public float zoomOut = 60f;

    void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fovTarget, Time.deltaTime * 5);

        // ���콺 ������ ��ư�� ������ ������ ZoomIn�� �ǰ�
        if (Input.GetButton("Fire2"))
        {
            Camera.main.fieldOfView = zoomIn;
            fovTarget = zoomIn;
        }

        // ���콺 ������ ��ư�� ���� ZoomOut�� �ǰ� �ϰ� �ʹ�.
        else if (Input.GetButtonUp("Fire2"))
        {
            fovTarget = zoomOut;

        }
    }
}
