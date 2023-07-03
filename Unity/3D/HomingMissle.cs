using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour
{
    public enum Type
    {
        one,
        two,
        three,
        four,
        five,
        six,
        seven,
        eight
    }

    public Type type;
    public GameObject missileEffectObj;
    public GameObject target;
    public GameObject damageText;
    public float vSpeed = 5f;
    [SerializeField] float speed = 0f;

    Rigidbody rigid;
    float currentSpeed = 0f;
    public int dmg = 3;
    void Start()
    {

        rigid = GetComponent<Rigidbody>();

    }

    private void Update()
    {


        // 타입에 따라 각각 다른 방향으로 미사일 발사
        if (type == Type.one)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = Vector3.left * 3;

            // 가속도 증가
            currentSpeed += 6 * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;



        }

        else if (type == Type.two)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = Vector3.right * vSpeed;

            // 가속도 증가
            currentSpeed += speed * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.three)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = Vector3.up * vSpeed;

            // 가속도 증가
            currentSpeed += 8f * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.four)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = Vector3.down * vSpeed;

            // 가속도 증가
            currentSpeed += speed * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.five)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = (Vector3.left + Vector3.up) * vSpeed;

            // 가속도 증가
            currentSpeed += speed * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.six)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = (Vector3.right + Vector3.up) * vSpeed;

            // 가속도 증가
            currentSpeed += speed * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.seven)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = (Vector3.down + Vector3.left) * vSpeed;

            // 가속도 증가
            currentSpeed += speed * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.eight)
        {
            transform.LookAt(target.transform);
            // 미사일을 휘게하기 위해 힘을 추가시킴
            rigid.velocity = (Vector3.down + Vector3.right) * vSpeed;

            // 가속도 증가
            currentSpeed += speed * Time.deltaTime;

            // 표적 거리 및 미사일 위치
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

    }
}
