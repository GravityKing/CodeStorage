using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자의 입력에 따라 앞뒤좌우로 이동하고 싶다.
public class PlayerMove : MonoBehaviour
{

    public float speed = 5;
    public int maxJump = 2;
    int curJump = 0;

    // Y속도
    float yVelocity = 0;
    // - 점프뛰는 힘
    [SerializeField]
    float JPower = 10;
    // - 중력
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
        dir = Camera.main.transform.TransformDirection(dir); // 카메라 방향대로 움직여야 하기 때문

        Vector3 velocity = dir * speed;
        velocity.y = yVelocity;
        cc.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        // 1. yVelocity가 중력의 영향을 받고 싶다.
        // 2. 만약 점프키를 누르면
        // 3. yVelocity를 JummpPower의 값으로 대입하고 싶다.
        // 4. yVelocity를 실제 방향에 대입하고 싶다.
        yVelocity += gravity * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && maxJump > curJump) //cc.collisionFlags == CollisionFlags.Below 한 번만 점프할 때 사용
        {
            yVelocity = JPower;
            curJump++;
        }

       
    }
    // 디버깅용
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position + new Vector3(0,yVelocity,0));
    }
}
