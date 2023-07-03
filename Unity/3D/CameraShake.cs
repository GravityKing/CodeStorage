using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static public CameraShake cs;

    Vector3 originPos;

    void Start()
    {
        cs = this;
        
    }

    public IEnumerator Shake(float _amount, float _duration)
    {
        originPos = transform.localPosition;
        float timer = 0;
        while (timer <= _duration)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * _amount + originPos;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originPos;

    }
}
