using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 상태머신을 이용해서 제어하고 싶다
// 대기 , 이동 , 공격 
public class Enemy : MonoBehaviour
{

    // enum을 쓸 때는 어떤 자료형도 선언할 필요 없다.
    // enum을 선언하고 따로 변수를 만들어야 한다.
    enum State
    {
        Idle,
        Walk,
        Attack,
        Damage,
        Die
    }
    [SerializeField] State state;

    NavMeshAgent agent;
    GameObject target;
    Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        agent.Warp(transform.position);
        state = State.Idle;

    }


    void Update()
    {
        if (state == State.Idle)
        {
            UpdateIdle();
        }

        if (state == State.Walk)
        {
            UpdateWalk();
        }

        if (state == State.Attack)
        {
            UpdateAttack();
        }
    }



    private void UpdateAttack()
    {
    }

    private void UpdateWalk()
    {
        // 1. agent에게 목적지를 알려주고 싶다.
        agent.SetDestination(target.transform.position);
        float distance = Vector3.Distance(transform.position,
                                          target.transform.position);
        // 2. 만약 목적지에 도착했다면
        // 3. Attack 상태로 전이하고 싶다.
        if (distance <= agent.stoppingDistance)
        {
            state = State.Attack;
            anim.SetTrigger("Attack");
        }
    }


    private void UpdateIdle()
    {    // 1. 플레이어를 찾고 싶다.
        target = GameObject.Find("Player");

        // 2. 만약 플레이어를 찾았다면
        // 3. Walk 상태로 전이하고 싶다.
        if (target != null)
        {
            state = State.Walk;
            anim.SetTrigger("Walk");
        }
    }

    internal void OnDamaged(int damage)
    {
        agent.isStopped = true;


        // EnemyHP가 가진 HP를 1 감소하고 싶다
        EnemyHP enemyHP = GetComponent<EnemyHP>();
        enemyHP.HP -= damage;
        // 만약 HP가 0이라면
        // 죽고 싶다.
        if (enemyHP.HP <= 0)
        {
            // 컴포넌트를 끄고 킬 수 있다
            GetComponent<Collider>().enabled = false;
            
            state = State.Die;
            anim.SetTrigger("Die");
            Destroy(gameObject, 2);
        }

        else
        {
            state = State.Damage;
            anim.SetTrigger("Damage");
        }

    }

    internal void OnDamagedFin()
    {
        state = State.Walk;
        anim.SetTrigger("Walk");
        agent.isStopped = false;
    }

    // 플레이어를 hit하는 순간이다.
    internal void OnAttackHit()
    {
        // 만약 플레이어가 공격 사정거리 안에 있다면
        // hit하고 싶다.
        float distance = Vector3.Distance(transform.position,
                                  target.transform.position);
        if (distance <= agent.stoppingDistance)
        {
            StopCoroutine(HitManager.instance.IEHit());
            StartCoroutine(HitManager.instance.IEHit());
        }
    }

    internal void OnAttackFin()
    {
        // 플레이어와의 거리를 측정한 후에
        // 만약 그 거리가 공격 사정거리보다 크면
        // Walk 상태로 전이하고 싶다.

        float distance = Vector3.Distance(transform.position,
                          target.transform.position);
        if (distance >= agent.stoppingDistance)
        {
            state = State.Walk;
            anim.SetTrigger("Walk");
            //anim.Play("Walk",0,0);
        }

        else
        {
            state = State.Attack;
            anim.SetTrigger("Attack");
        }
    }

    private void OnDestroy()
    {
        // 플레이어가 적을 죽이면 enemyKillCount를 1 증가시키고 싶다
        // 만약 enemyKillCount가 maxEnemyCount 이상이면 레벨업 하고 
        LevelManager.instance.AddEnemyKillCount();
    }

}
