using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ho_ObjectPoolObj : MonoBehaviour
{
    public static Ho_ObjectPoolObj instance;
    private void Awake()
    {
        instance = this;
    }
    private void OnDisable()
    {
        Ho_ObjectPool.instance.SetDeactiveInstance(this);
    }

    public void GetDisable()
    {
        StopCoroutine(IESetActiveFalse(0.1f));
        StartCoroutine(IESetActiveFalse(0.1f));
    }

    public void SetDisable(float time)
    {
        StopCoroutine(IESetActiveFalse(time));
        StartCoroutine(IESetActiveFalse(time));
    }

    IEnumerator IESetActiveFalse(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 0 ZombieGuy
    /// 1 ZombieGirl
    /// 2 ZombieSuicide
    /// </summary>
    public string GetKeyValue(int valueNum)
    {
        switch (valueNum)
        {
            case 0:
                return "Ho_NormalZombieGuy";

            case 1:
                return "Ho_NormalZombieGirl";

            case 2:
                return "Ho_SuicideZombie";

            case 3:
                return "Ho_SwingZombie";
        }

        return null;
    }
}
