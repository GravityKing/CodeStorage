using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� �Է¿� ���� �յ��¿�� �̵��ϰ� �ʹ�.
public class PlayerMove : MonoBehaviour
{

    public float speed = 5;
    public int maxJump = 2;
    int curJump = 0;

    // Y�ӵ�
    float yVelocity = 0;
    // - �����ٴ� ��
    [SerializeField]
    float JPower = 10;
    // - �߷�
    [SerializeField]
    float gravity = -9.81f;

    CharacterController cc;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (cc.isGrounded)
        {
            curJump = 0;
            yVelocity = 0;
        }

        Jump();
        Move();

    }

    private void FixedUpdate()
    {
        
    }

    void Move()
    {
        Vector3 dir = 
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir); // ī�޶� ������ �������� �ϱ� ����

        Vector3 velocity = dir * speed;
        velocity.y = yVelocity;
        cc.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        // 1. yVelocity�� �߷��� ������ �ް� �ʹ�.
        // 2. ���� ����Ű�� ������
        // 3. yVelocity�� JummpPower�� ������ �����ϰ� �ʹ�.
        // 4. yVelocity�� ���� ���⿡ �����ϰ� �ʹ�.
        yVelocity += gravity * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && maxJump > curJump) //cc.collisionFlags == CollisionFlags.Below �� ���� ������ �� ���
        {
            yVelocity = JPower;
            curJump++;
        }

       
    }
    // ������
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position + new Vector3(0,yVelocity,0));
    }
}
