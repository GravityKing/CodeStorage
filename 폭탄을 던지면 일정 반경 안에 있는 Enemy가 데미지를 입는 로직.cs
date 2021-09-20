using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 태어날 때 앞 방향으로 계속 이동하게 하고 싶다(단, 물리엔진(Rigidbody)를 이용해서)
// 
public class Bomb : MonoBehaviour
{
    public float force = 10;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }
    private void OnEnable()
    {
        Vector3 dir = transform.forward + transform.up;
        dir = Camera.main.transform.TransformDirection(dir);

        rigid.velocity = dir * force; // 등속도 운동 AddForce를 사용하면 가속도가 됨

    }

    void Update()
    {

    }


    private void OnCollisionEnter(Collision other)
    {
        int layer = 1 << LayerMask.NameToLayer("Enemy");
        // 부딪힌 시점에 반경 3M 내의 Enemy 충돌체들을 검출하고 싶다
        // 그리고 그 충돌체가 Enemy라면
        Collider[] cols = Physics.OverlapSphere(other.contacts[0].point, 3f, layer);
        if (cols.Length > 0)
        {
            // Enemy의 Com에게 너 폭탄에 맞았어라고 알려주고 싶다.
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].gameObject.GetComponent<Enemy>().OnDamaged(3);
            }

        }
        GameObject explosion = Instantiate(expFactory);
        explosion.transform.position = other.contacts[0].point;
        gameObject.SetActive(false);

        //  PlayerFire의 deActiveBombList에 추가하고 싶다.
        PlayerFire.instance.deActiveBombList.Add(gameObject);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero; // 회전하는 것 방지
        
    }
    public GameObject expFactory;
}
