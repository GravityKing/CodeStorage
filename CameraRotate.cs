using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ڰ� ���콺�� ȸ���ϸ� ī�޶� ȸ���ϰ� �ʹ�.
public class CameraRotate : MonoBehaviour
{
    public float rotSpeed = 200;
    float rx,ry;

    void Start()
    {
        
    }

    void Update()
    {
        // ����ڰ� ���콺�� ȸ���ϸ�
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        rx -= my * Time.deltaTime * rotSpeed;
        ry += mx * Time.deltaTime * rotSpeed;

        // rx�� ȸ������ �����ϰ� �ʹ�.
        rx = Mathf.Clamp(rx,-90,90);
        // rx = Clamp(rx,-90,90);

        // ī�޶� ȸ���ϰ� �ʹ�
        // x���� ������ ���� ���� ����
        transform.eulerAngles = new Vector3(rx, ry, 0);

    }

    // �̷��� �Լ��� ����� �� ���� �ִ�
    static float Clamp(float value, float min, float max)
    {
        // ���� value�� min ���� �۴ٸ� min�� ��ȯ�ϰ� �ʹ�.
        // ���� value�� max ���� ũ�ٸ� max�� ��ȯ�ϰ� �ʹ�.
        // �׷��� �ʴٸ� value�� ��ȯ�ϰ� �ʹ�.

        if (value < min)
            return min;

        else if (value > max)
            return max;

        else
            return value;
    }
}
