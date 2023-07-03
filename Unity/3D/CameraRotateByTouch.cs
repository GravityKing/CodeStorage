using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateByTouch : MonoBehaviour
{
    [SerializeField]
    private Transform videoCamera;
    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private Vector3 velocity;

    void Start()
    {
        if (Setting.Video.option)
        {
            videoCamera.localPosition = Setting.Video.cameraPosition;
            videoCamera.localRotation = Setting.Video.cameraRotation;
            videoCamera.localScale = Vector3.one;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (videoCamera)
            {
                velocity.y += 2.0f * Input.GetAxis(MouseXInput);
                velocity.x -= 2.0f * Input.GetAxis(MouseYInput);

                velocity.x = Mathf.Clamp(velocity.x, -50.0f, 65.0f);
                videoCamera.eulerAngles = velocity;
            }
        }
    }
}
