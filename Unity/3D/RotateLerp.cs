using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateLerp : MonoBehaviour
{
    public Canvas clickCanvas;
    public GameObject messageBox;
    public float npcRotationSpeed;

    private Transform camTransform;

    private void Update()
    {
        if (clickCanvas.gameObject.activeSelf)
        {
            if (camTransform == null)
                camTransform = Camera.main.transform;

            Vector3 lookPos = new Vector3(camTransform.position.x, transform.position.y, camTransform.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(lookPos - transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, npcRotationSpeed * Time.deltaTime);
            messageBox.transform.rotation = camTransform.rotation;
        }
    }
}
