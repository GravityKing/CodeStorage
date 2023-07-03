/*
작성자: 최재호(cjh0798@gmail.com)
기능: 최근검색 Save Load
 */
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class RecentSearchBase : MonoBehaviour
{
    [System.Serializable]
    public class SearchInfo
    {
        public List<string> searchList;

        public SearchInfo(List<string> _searchList)
        { searchList = _searchList; }
    }

    public TMP_InputField input;
    protected string path;
    protected List<string> searchList;

    // 현재 SearchList Save
    public virtual void SaveSearch()
    {
        SearchInfo info = new SearchInfo(searchList);
        string json = JsonUtility.ToJson(info);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        string code = System.Convert.ToBase64String(bytes);
        File.WriteAllText(path, code);

    }

    // Input 값을 포함한 현재 SearchList Save
    public virtual void SaveSearch(TMP_InputField input)
    {
        if (string.IsNullOrWhiteSpace(input.text))
            return;

        else if (IsSameKeyword(input.text) == true)
        {
            searchList.Remove(input.text);
            searchList.Add(input.text);
            SaveSearch();
        }

        else
        {
            searchList.Add(input.text);
            SaveSearch();
        }
    }

    // 검색 패널 값을 포함한 현재 SearchList Save
    public virtual void SaveSearch(TMP_Text _text)
    {
        if (string.IsNullOrWhiteSpace(_text.text))
            return;

        else if (IsSameKeyword(_text.text) == true)
        {
            searchList.Remove(_text.text);
            searchList.Add(_text.text);
            SaveSearch();
        }

        else
        {
            searchList.Add(_text.text);
            SaveSearch();
        }
    }

    // SearchList Load
    public virtual async UniTask LoadSearch()
    {
        await UniTask.RunOnThreadPool(() =>
        {
            try
            {
                string code = File.ReadAllText(path);
                byte[] bytes = System.Convert.FromBase64String(code);
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                SearchInfo info = JsonUtility.FromJson<SearchInfo>(json);

                if (info.searchList.Count == 0)
                    return;

                else
                {
                    searchList = info.searchList;
                }
            }
            catch (System.Exception)
            {
                searchList = new List<string>();
            }
        });

    }

    // 같은 키워드 검사
    bool IsSameKeyword(string _text)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (searchList[i] == _text)
            {
                return true;
            }
        }
        return false;
    }

}
