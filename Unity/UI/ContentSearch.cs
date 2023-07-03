/*
작성자: 최재호(cjh0798@gmail.com)
기능: 검색
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class ContentSearch : RecentSearchBase
{
    [SerializeField] ProfileMain profileMain;
    [SerializeField] string txtName;
    public ScrollRect scrollRect;
    private RectTransform searchPanelPrefab;
    private List<RectTransform> searchPanelList;
    private async void Start()
    {
        await UniTask.WaitUntil(() => profileMain.profileData != null);
        path = $"{Application.persistentDataPath}/{profileMain.profileData.data.userId}{txtName}.txt";
        searchPanelPrefab = scrollRect.content.GetChild(0).GetComponent<RectTransform>();
        searchPanelList = new List<RectTransform>();
        searchList = new List<string>();

        Init();
    }

    // 초기화
    public async void Init()
    {
        DeleteAllSearchPanel();
        await LoadSearch();
        CreateSearchPanel();
    }

    // 검색 Panel 삭제(하나만 삭제)
    public void DeleteMatePanel(RectTransform _panel)
    {
        int targetIndex = (searchList.Count - searchPanelList.IndexOf(_panel)) - 1;
        searchList.RemoveAt(targetIndex);
        searchPanelList.Remove(_panel);
        Destroy(_panel.gameObject);


        SaveSearch();
        UpdateAllLayout();
    }

    // 모든 검색 Panel 삭제(오브젝트를 삭제)
    private void DeleteAllSearchPanel()
    {
        try
        {
            for (int i = 0; i < searchPanelList.Count; i++)
            {
                Destroy(searchPanelList[i].gameObject);
            }
        }
        catch (System.Exception)
        {

        }
        searchList.Clear();
        searchPanelList.Clear();

    }

    // 모든 검색 기록 삭제(검색 기록 자체를 삭제)
    public void DeleteAllSearch()
    {
        DeleteAllSearchPanel();
        SaveSearch();
    }

    // 검색 panel 생성(한 개 이상)
    private void CreateSearchPanel()
    {

        int count = searchList.Count;
        int panelIndex = 0;
        for (int i = count - 1; i >= 0; i--)
        {
            RectTransform obj = Instantiate(searchPanelPrefab, scrollRect.content.transform);
            obj.gameObject.SetActive(true);
            searchPanelList.Add(obj);
            searchPanelList[panelIndex].GetComponentInChildren<TMP_Text>().text = searchList[i];
            panelIndex++;
        }
        UpdateAllLayout();
    }

    // 단일 패널 레이아웃 업데이트
    private void UpdateLayout(RectTransform _obj)
    {
        float height = searchPanelPrefab.rect.height;
        float spacing = 44;
        float posY = 0;
        posY = (searchPanelList.Count - 1) * (height + spacing);
        _obj.anchoredPosition = new Vector2(0, -posY);
    }

    // 모든 패널 레이아웃 업데이트
    public void UpdateAllLayout()
    {
        float height = searchPanelPrefab.rect.height;
        float spacing = 44;
        float posY = 0;
        for (int i = 0; i < searchPanelList.Count; i++)
        {
            posY = (i) * (height + spacing);
            RectTransform obj = searchPanelList[i].GetComponent<RectTransform>();
            obj.anchoredPosition = new Vector2(0, -posY);
        }

        if (searchPanelList.Count >= 15)
        {
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15 * (height + spacing));
        }
        else
        {
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, searchPanelList.Count * (height + spacing));
        }
        
    }

}
