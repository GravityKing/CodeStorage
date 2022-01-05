using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public Item(string _name, int _num)
    { name = _name; num = _num; }

    public string name;
    public int num;
}

public class JH_Sorting : MonoBehaviour
{
    List<Item> foodList;
    Dictionary<string, List<Item>> inventoryDic;

    private void Start()
    {
        inventoryDic = new Dictionary<string, List<Item>>();
        foodList = new List<Item>();
        List<string> list = new List<string>();

        list.Add("Beer");
        list.Add("Apple");
        list.Add("Direction");
        list.Add("Chair");

        for (int i = 0; i < list.Count; i++)
        {
            foodList.Add(new Item($"{list[i]}", i)); // i 대신 _inventory["wear"][] 에서 number 가져오기
        }

        foodList.Sort((p1, p2) => p1.num.CompareTo(p2.num));

        inventoryDic.Add("food", foodList);

    }
}
