using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �¾ �� �� �������� ��� �̵��ϰ� �ϰ� �ʹ�(��, ��������(Rigidbody)�� �̿��ؼ�)
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

        rigid.velocity = dir * force; // ��ӵ� � AddForce�� ����ϸ� ���ӵ��� ��

    }

    void Update()
    {

    }


    private void OnCollisionEnter(Collision other)
    {
        int layer = 1 << LayerMask.NameToLayer("Enemy");
        // �ε��� ������ �ݰ� 3M ���� Enemy �浹ü���� �����ϰ� �ʹ�
        // �׸��� �� �浹ü�� Enemy���
        Collider[] cols = Physics.OverlapSphere(other.contacts[0].point, 3f, layer);
        if (cols.Length > 0)
        {
            // Enemy�� Com���� �� ��ź�� �¾Ҿ��� �˷��ְ� �ʹ�.
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].gameObject.GetComponent<Enemy>().OnDamaged(3);
            }

        }
        GameObject explosion = Instantiate(expFactory);
        explosion.transform.position = other.contacts[0].point;
        gameObject.SetActive(false);

        //  PlayerFire�� deActiveBombList�� �߰��ϰ� �ʹ�.
        PlayerFire.instance.deActiveBombList.Add(gameObject);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero; // ȸ���ϴ� �� ����
        
    }
    public GameObject expFactory;
}
