using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ���¸ӽ��� �̿��ؼ� �����ϰ� �ʹ�
// ��� , �̵� , ���� 
public class Enemy : MonoBehaviour
{

    // enum�� �� ���� � �ڷ����� ������ �ʿ� ����.
    // enum�� �����ϰ� ���� ������ ������ �Ѵ�.
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
        // 1. agent���� �������� �˷��ְ� �ʹ�.
        agent.SetDestination(target.transform.position);
        float distance = Vector3.Distance(transform.position,
                                          target.transform.position);
        // 2. ���� �������� �����ߴٸ�
        // 3. Attack ���·� �����ϰ� �ʹ�.
        if (distance <= agent.stoppingDistance)
        {
            state = State.Attack;
            anim.SetTrigger("Attack");
        }
    }


    private void UpdateIdle()
    {    // 1. �÷��̾ ã�� �ʹ�.
        target = GameObject.Find("Player");

        // 2. ���� �÷��̾ ã�Ҵٸ�
        // 3. Walk ���·� �����ϰ� �ʹ�.
        if (target != null)
        {
            state = State.Walk;
            anim.SetTrigger("Walk");
        }
    }

    internal void OnDamaged(int damage)
    {
        agent.isStopped = true;


        // EnemyHP�� ���� HP�� 1 �����ϰ� �ʹ�
        EnemyHP enemyHP = GetComponent<EnemyHP>();
        enemyHP.HP -= damage;
        // ���� HP�� 0�̶��
        // �װ� �ʹ�.
        if (enemyHP.HP <= 0)
        {
            // ������Ʈ�� ���� ų �� �ִ�
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

    // �÷��̾ hit�ϴ� �����̴�.
    internal void OnAttackHit()
    {
        // ���� �÷��̾ ���� �����Ÿ� �ȿ� �ִٸ�
        // hit�ϰ� �ʹ�.
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
        // �÷��̾���� �Ÿ��� ������ �Ŀ�
        // ���� �� �Ÿ��� ���� �����Ÿ����� ũ��
        // Walk ���·� �����ϰ� �ʹ�.

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
        // �÷��̾ ���� ���̸� enemyKillCount�� 1 ������Ű�� �ʹ�
        // ���� enemyKillCount�� maxEnemyCount �̻��̸� ������ �ϰ� 
        LevelManager.instance.AddEnemyKillCount();
    }

}
