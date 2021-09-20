using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyBase : MonoBehaviour
{
    NavMeshAgent agent;
    Transform target;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        AutoTarget();
    }
    public void AutoTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        if (towers.Length > 0)
        {
            // 1. ���� �� Ÿ������ �Ÿ��� �����ϰ� �ʹ�
            // 2. ���� ����� Ÿ���� Ÿ������ ��� �ʹ�
            int tempIndex = -1;
            float temp = 0;
            for (int i = 0; i < towers.Length; i++)
            {
                float tempDistance = Vector3.Distance(transform.position,towers[i].transform.position);
                if (i == 0)
                {
                    tempIndex = i;
                    temp = tempDistance;
                }

                else if (temp > tempDistance)
                {
                    tempIndex = i;
                    temp = tempDistance;
                }
            }
            if (tempIndex == -1)
                return;

            target = towers[tempIndex].transform;

        }
    }
    public virtual void Update()
    {
        agent.SetDestination(target.position);
    }

    public virtual void DoDamage(int damage)
    {
        GameObject explosion = ObjectPool.instance.GetDeactiveInstace("Explosion");
        if (explosion != null)
        {
            explosion.transform.position = transform.position;
            explosion.GetComponent<ObjectPoolObj>().SetDisable(2, () =>
            {
                Destroy(explosion);
            });
        }
        Destroy(gameObject);
    }
}
