using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothDamp : MonoBehaviour
{
public Camera player1Camera;

//누가 공격하고 있는지에 대한 플레그
public bool isCamera1 = true;
public bool isCamera2 = false;

//----------------------------------------------------------
public float cameraMoveValue = 0.05f;
public GameObject player1CameraPos;
public GameObject player2CameraPos;
void Update()
{

    if (isCamera1 == false)
    {
        Vector3 velo = Vector3.zero;
        player1Camera.transform.position = Vector3.SmoothDamp(player1Camera.transform.position, player2CameraPos.transform.position, ref velo, cameraMoveValue);
    }
    else if (isCamera2 == false)
    {
        Vector3 velo = Vector3.zero;
        player1Camera.transform.position = Vector3.SmoothDamp(player1Camera.transform.position, player1CameraPos.transform.position, ref velo, cameraMoveValue);
    }
}
}
