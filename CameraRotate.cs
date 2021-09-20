using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자가 마우스를 회전하면 카메라를 회전하고 싶다.
public class CameraRotate : MonoBehaviour
{
    public float rotSpeed = 200;
    float rx,ry;

    void Start()
    {
        
    }

    void Update()
    {
        // 사용자가 마우스를 회전하면
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        rx -= my * Time.deltaTime * rotSpeed;
        ry += mx * Time.deltaTime * rotSpeed;

        // rx의 회전값을 제한하고 싶다.
        rx = Mathf.Clamp(rx,-90,90);
        // rx = Clamp(rx,-90,90);

        // 카메라를 회전하고 싶다
        // x축을 돌리면 고개를 흔드는 느낌
        transform.eulerAngles = new Vector3(rx, ry, 0);

    }

    // 이렇게 함수를 만들어 쓸 수도 있다
    static float Clamp(float value, float min, float max)
    {
        // 만약 value가 min 보다 작다면 min을 반환하고 싶다.
        // 만약 value가 max 보다 크다면 max를 반환하고 싶다.
        // 그렇지 않다면 value를 반환하고 싶다.

        if (value < min)
            return min;

        else if (value > max)
            return max;

        else
            return value;
    }
}
