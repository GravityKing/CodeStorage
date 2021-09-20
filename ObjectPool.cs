using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ho_ObjectPool : MonoBehaviour
{
    public static Ho_ObjectPool instance;

    Dictionary<string, List<GameObject>> dic;
    Dictionary<string, List<GameObject>> deActiveDic;
    private void Awake()
    {
        instance = this;

        dic = new Dictionary<string, List<GameObject>>();
        deActiveDic = new Dictionary<string, List<GameObject>>();
    }

    public void CreateInstance(string key, int count)
    {
        if (true == dic.ContainsKey(key))
            return;

        GameObject prefab = Resources.Load<GameObject>(key);
        List<GameObject> list = new List<GameObject>();

        dic.Add(key,list);
        deActiveDic.Add(key,list);
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab);
            go.name = key;
            go.AddComponent<Ho_ObjectPoolObj>();
            go.SetActive(false);
            list.Add(go);
        }
    }

    int listNum;
    public GameObject GetDeactiveInstance(string key)
    {
        // 만약 key가 deActiveDic에 없거나 deActiveDic.count가 0이라면
        // null을 반환하고 싶다
        if (false == deActiveDic.ContainsKey(key) || deActiveDic[key].Count == 0)
            return null;

        // 그렇지 않다면
        // deActiveDic의 첫번째 값을 반환하고 싶다.
        // 반환하기 전에 deActiveDic의 목록에서 제외하고 싶다.
        List<GameObject> list = deActiveDic[key];
        GameObject temp = list[listNum]; // temp를 만들어야 삭제하고 리턴값을 반환할 수 있다.
        list.Remove(temp);
        
        listNum++;
        if (listNum == deActiveDic[key].Count)
        {
            print("오브젝트풀에서 오브젝트를 더 만들어주세요.");
            return null;
        }

        return temp;

    }

    public void SetDeactiveInstance(Ho_ObjectPoolObj ho_ObjectPoolObj)
    {
        // 사용된 오브젝트가 Disable 됐을 때 다시 DeActiveDic에 추가시킨다.
        deActiveDic[ho_ObjectPoolObj.name].Add(ho_ObjectPoolObj.gameObject); 
    }

}
