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
        // ���� key�� deActiveDic�� ���ų� deActiveDic.count�� 0�̶��
        // null�� ��ȯ�ϰ� �ʹ�
        if (false == deActiveDic.ContainsKey(key) || deActiveDic[key].Count == 0)
            return null;

        // �׷��� �ʴٸ�
        // deActiveDic�� ù��° ���� ��ȯ�ϰ� �ʹ�.
        // ��ȯ�ϱ� ���� deActiveDic�� ��Ͽ��� �����ϰ� �ʹ�.
        List<GameObject> list = deActiveDic[key];
        GameObject temp = list[listNum]; // temp�� ������ �����ϰ� ���ϰ��� ��ȯ�� �� �ִ�.
        list.Remove(temp);
        
        listNum++;
        if (listNum == deActiveDic[key].Count)
        {
            print("������ƮǮ���� ������Ʈ�� �� ������ּ���.");
            return null;
        }

        return temp;

    }

    public void SetDeactiveInstance(Ho_ObjectPoolObj ho_ObjectPoolObj)
    {
        // ���� ������Ʈ�� Disable ���� �� �ٽ� DeActiveDic�� �߰���Ų��.
        deActiveDic[ho_ObjectPoolObj.name].Add(ho_ObjectPoolObj.gameObject); 
    }

}
