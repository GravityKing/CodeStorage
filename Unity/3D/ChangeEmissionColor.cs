using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEmissionColor : MonoBehaviour
{
    [SerializeField] private Material mercuryMat;
    [SerializeField] private float lerpDuration = 1.5f;
    private int layerIndex;
    private float lerpTimer;
    private Color curColor;
    private Color originColor;
    private Color brightColor;

    private Coroutine co;
    private void Start()
    {
        layerIndex = LayerMask.NameToLayer("Player");
        Debug.Log("layerIndex: " + layerIndex);
        curColor = new Color(1.498039f, 1.411765f, 0.9019608f);
        originColor = curColor;
        brightColor = new Color(2.976675f, 2.862835f, 2.190144f);
        mercuryMat.SetColor("_EmissionColor",originColor);
    }

    private void OnApplicationQuit()
    {
        mercuryMat.SetColor("_EmissionColor", originColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: {other.name}");
        if (other.gameObject.layer == layerIndex)
        {
            Debug.Log("Player Enter");
            if (co != null)
            {
                StopCoroutine(co);
                lerpTimer = 0;
            }

            co = StartCoroutine(SetEmission(curColor, brightColor));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit: {other.name}");
        if (other.gameObject.layer == layerIndex)
        {
            Debug.Log("Player Exit");
            if (co != null)
            {
                StopCoroutine(co);
                lerpTimer = 0;
            }

            co = StartCoroutine(SetEmission(curColor, originColor));
        }
    }


    private IEnumerator SetEmission(Color oldColor, Color newColor)
    {
        Debug.Log("SetEmission ¡¯¿‘");
        while (lerpTimer < lerpDuration)
        {
            lerpTimer += Time.deltaTime;
            float t = lerpTimer / lerpDuration;
            Color lerpedColor = Color.Lerp(oldColor, newColor, t);
            curColor = lerpedColor;
            mercuryMat.SetColor("_EmissionColor", lerpedColor);
            yield return new WaitForEndOfFrame();
        }
        lerpTimer = 0;
    }
}
