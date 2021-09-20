using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ho_SpawnManager : MonoBehaviour
{
    public List<Transform> list;
    void Start()
    {
        same = new List<int>();
    }

    void Update()
    {

    }
    // 활성화 시킬 때 배열을 만들어서
    // 활성화 된 좀비 배열을 만든다
    // 활성된 좀비들 중 가까운 위치에 있는 좀비를 뽑는다.
    public void SpawnZombieGuy(int pos)
    {
        Ho_ObjectPool.instance.CreateInstance("Ho_NormalZombieGuy", 105);
        GameObject zombieGuy = Ho_ObjectPool.instance.GetDeactiveInstance(Ho_ObjectPoolObj.instance.GetKeyValue(0));
        //GameObject zombieGuy = Ho_ObjectPool.instance.GetDeactiveInstance("Ho_NormalZombieGuy");
        if (zombieGuy != null)
        {
            zombieGuy.SetActive(true);
            zombieGuy.transform.position = GetSpawnPos(pos);

            // 각자 다른 방향에서 때려야 하는데 Transform으로 이동하기 때문에
            // 이동 패턴을 3가지로 나눈다.
            if (pos % 3 == 1)
            {
                zombieGuy.GetComponent<Ho_NormalZombie>().targetNum = 0;
            }

            else if (pos % 3 == 2)
            {
                zombieGuy.GetComponent<Ho_NormalZombie>().targetNum = 1;
            }

            else if (pos % 3 == 0)
            {
                zombieGuy.GetComponent<Ho_NormalZombie>().targetNum = 2;
            }
        }
    }

    public void SpawnZombieGirl(int pos)
    {
        Ho_ObjectPool.instance.CreateInstance("Ho_NormalZombieGirl", 105);
        GameObject zombieGirl = Ho_ObjectPool.instance.GetDeactiveInstance(Ho_ObjectPoolObj.instance.GetKeyValue(1));

        if (zombieGirl != null)
        {
            zombieGirl.SetActive(true);
            zombieGirl.transform.position = GetSpawnPos(pos);

            // 각자 다른 방향에서 때려야 하는데 Transform으로 이동하기 때문에
            // 이동 패턴을 3가지로 나눈다.
            if (pos % 3 == 1)
            {
                zombieGirl.GetComponent<Ho_NormalZombie>().targetNum = 0;
            }

            else if (pos % 3 == 2)
            {
                zombieGirl.GetComponent<Ho_NormalZombie>().targetNum = 1;
            }

            else if (pos % 3 == 0)
            {
                zombieGirl.GetComponent<Ho_NormalZombie>().targetNum = 2;
            }

        }
    }

    public void SpawnZombieSuicide(int pos)
    {
        Ho_ObjectPool.instance.CreateInstance("Ho_SuicideZombie", 100);
        GameObject zombieSuicide = Ho_ObjectPool.instance.GetDeactiveInstance(Ho_ObjectPoolObj.instance.GetKeyValue(2));

        if (zombieSuicide != null)
        {
            zombieSuicide.SetActive(true);
            zombieSuicide.transform.position = GetSpawnPos(pos);

            // 각자 다른 방향에서 때려야 하는데 Transform으로 이동하기 때문에
            // 이동 패턴을 3가지로 나눈다.
            if (pos % 3 == 1)
            {
                zombieSuicide.GetComponent<Ho_SuicideZombie>().targetNum = 0;
            }

            else if (pos % 3 == 2)
            {
                zombieSuicide.GetComponent<Ho_SuicideZombie>().targetNum = 1;
            }

            else if (pos % 3 == 0)
            {
                zombieSuicide.GetComponent<Ho_SuicideZombie>().targetNum = 2;
            }

        }
    }

    public void SpawnSwingZombie(int pos)
    {
        Ho_ObjectPool.instance.CreateInstance("Ho_SwingZombie", 105);
        GameObject swingZombie = Ho_ObjectPool.instance.GetDeactiveInstance(Ho_ObjectPoolObj.instance.GetKeyValue(3));

        if (swingZombie != null)
        {
            swingZombie.SetActive(true);
            swingZombie.transform.position = GetSpawnPos(pos);

            // 각자 다른 방향에서 때려야 하는데 Transform으로 이동하기 때문에
            // 이동 패턴을 3가지로 나눈다.
            if (pos % 3 == 1)
            {
                swingZombie.GetComponent<Ho_NormalZombie>().targetNum = 0;
            }

            else if (pos % 3 == 2)
            {
                swingZombie.GetComponent<Ho_NormalZombie>().targetNum = 1;
            }

            else if (pos % 3 == 0)
            {
                swingZombie.GetComponent<Ho_NormalZombie>().targetNum = 2;
            }

        }
    }

    public void SpawnFake()
    {
        Ho_ObjectPool.instance.CreateInstance("Ho_SwingZombie", 100);
        GameObject fake = Ho_ObjectPool.instance.GetDeactiveInstance(Ho_ObjectPoolObj.instance.GetKeyValue(3));
        //fake.GetComponent<Ho_ObjectPoolObj>().SetDisable(0.01f);
    }

    int Index;
    List<int> same;
    // mix,max 값을 SpawnManager에서 가져다 쓴다
    //Vector3 GetRandomSpawnPos(int min, int max)
    // {
    //     Index = Random.Range(min, max); // count 개

    //     for (int i = 0; i < same.Count; i++)
    //     {
    //         if (same.Contains(Index))
    //             return GetRandomSpawnPos(min, max);

    //     }
    //     same.Add(Index);

    //     return list[Index].position;

    // }

    Vector3 GetSpawnPos(int pos)
    {


        return list[pos].position;

    }

}
