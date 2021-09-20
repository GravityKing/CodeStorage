using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRotate : MonoBehaviour
{
    public GameObject gun;
    public GameObject gunBody;
    public GameObject target;
    public float speed = 3;
    public float turnSpeed = 2;

    void Start()
    {



    }
    void Update()
    {
        TankRot();
    }
    void TankRot()
    {
        Quaternion t_lookRotation = Quaternion.LookRotation(target.transform.position);
        Vector3 t_euler = Quaternion.RotateTowards(gunBody.transform.rotation,
                                                   t_lookRotation,
                                                   turnSpeed * Time.deltaTime).eulerAngles;

        gunBody.transform.rotation = Quaternion.Euler(0, t_euler.y, 0);

    }

}
