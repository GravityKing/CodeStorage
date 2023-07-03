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


        // Ÿ�Կ� ���� ���� �ٸ� �������� �̻��� �߻�
        if (type == Type.one)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = Vector3.left * 3;

            // ���ӵ� ����
            currentSpeed += 6 * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;



        }

        else if (type == Type.two)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = Vector3.right * vSpeed;

            // ���ӵ� ����
            currentSpeed += speed * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.three)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = Vector3.up * vSpeed;

            // ���ӵ� ����
            currentSpeed += 8f * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.four)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = Vector3.down * vSpeed;

            // ���ӵ� ����
            currentSpeed += speed * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.five)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = (Vector3.left + Vector3.up) * vSpeed;

            // ���ӵ� ����
            currentSpeed += speed * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.six)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = (Vector3.right + Vector3.up) * vSpeed;

            // ���ӵ� ����
            currentSpeed += speed * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.seven)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = (Vector3.down + Vector3.left) * vSpeed;

            // ���ӵ� ����
            currentSpeed += speed * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

        else if (type == Type.eight)
        {
            transform.LookAt(target.transform);
            // �̻����� �ְ��ϱ� ���� ���� �߰���Ŵ
            rigid.velocity = (Vector3.down + Vector3.right) * vSpeed;

            // ���ӵ� ����
            currentSpeed += speed * Time.deltaTime;

            // ǥ�� �Ÿ� �� �̻��� ��ġ
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;
        }

    }
}
