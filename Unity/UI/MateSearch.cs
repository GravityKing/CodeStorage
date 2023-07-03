/*
작성자: 최재호 (cjh0798@gmail.com)
기능: 
1. 메이트 검색
2. 검색된 메이트 패널 생성
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using Metalive;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

#region MateSearchDataRoot Class

public class MateSearchDataRoot
{
    public string result { get; set; }
    public List<MateSearchData> data { get; set; }
    public string message { get; set; }
}
public class MateSearchData
{
    public int no { get; set; }
    public int userCodeNo { get; set; }
    public string nickname { get; set; }
    public string userId { get; set; }
    public string followingYn { get; set; }
    public MateSearchProfileImgPath profileImgPath { get; set; }
}

public class MateSearchProfileImgPath
{
    public string SD { get; set; }
    public string ORIGIN { get; set; }
}


#endregion

public class MateSearch : MonoBehaviour
{
    [SerializeField] private MainTextureDisposer textureDisposer;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private MainSceneLoading loading;
    [SerializeField] private GameObject noResultIcon;
    public ScrollRect scrollRect;
    private RectTransform matePrefab;
    private List<MateSearchPanel> matePanelList;

    private List<string> BanList = new List<string>();
    private bool isAdding;
    private int lastNo;
    private string searchKeyword;

    private void Start()
    {
        matePrefab = scrollRect.content.GetChild(0).GetComponent<RectTransform>();
        matePanelList = new List<MateSearchPanel>();
    }

    public void DoReset()
    {
        input.text = "";
        isAdding = false;
        lastNo = 0;
        searchKeyword = "";
        noResultIcon.gameObject.SetActive(false);
        GetComponentInParent<ProfileMateSearchPage>().MoveToRecentSearch();
    }

    // InputFiled로 메이트 검색
    public void Search()
    {
        if (string.IsNullOrEmpty(input.text))
            return;


        else if (Regex.IsMatch(input.text, @"[^a-zA-Z0-9가-힣]") && !input.text.Contains("@"))
        {
            noResultIcon.SetActive(true);
            DeleteMatePanel();
            GetComponentInParent<ProfileMateSearchPage>().MoveToSearchResult();
            return;
        }

        DeleteMatePanel();
        isAdding = false;
        lastNo = 0;
        searchKeyword = input.text;
        GetComponentInParent<ProfileMateSearchPage>().MoveToSearchResult();
        GetSearchResult(input.text).Forget();
    }

    // 최근 검색을 눌러서 검색
    public void Search(TMP_Text _text)
    {
        if (string.IsNullOrEmpty(_text.text))
            return;

        else if (Regex.IsMatch(_text.text, @"[^a-zA-Z0-9가-힣]") && !_text.text.Contains("@"))
        {
            noResultIcon.SetActive(true);
            DeleteMatePanel();
            GetComponentInParent<ProfileMateSearchPage>().MoveToSearchResult();
            return;
        }

        DeleteMatePanel();
        isAdding = false;
        lastNo = 0;
        searchKeyword = _text.text;
        GetComponentInParent<ProfileMateSearchPage>().MoveToSearchResult();
        GetSearchResult(_text.text).Forget();
    }

    private async UniTask GetSearchResult(string _text)
    {

        loading.SetActiveMidLoading(true, async () => await GetSearchResult(_text));


        string uri = $"https://{Metalive.Setting.Server.api}/api/mate/search?findMate={_text}&lastNo={lastNo}&range=20";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            try
            {
                request.SetRequestHeader("Authorization", "Bearer " + Setting.User.token);
                await request.SendWebRequest();
            }
            catch (System.Exception)
            {
                loading.SetActiveMidLoading(false);
                DeleteMatePanel();
                noResultIcon.SetActive(true);
            }


            GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSceneManager>().CheckDuplicateLogin(request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("메이트 데이터 result: " + request.result);
            }
            else
            {
                JObject requestData = JObject.Parse(request.downloadHandler.text);
                string requestResult = requestData["result"].ToString();
                if (requestResult == "false")
                {
                    if (!isAdding)
                    {
                        noResultIcon.SetActive(true);
                    }
                    loading.SetActiveMidLoading(false);
                    return;
                }
                else
                {
                    noResultIcon.SetActive(false);
                    MateSearchDataRoot mateData = JsonConvert.DeserializeObject<MateSearchDataRoot>(request.downloadHandler.text);
                    AddMatePanel(mateData.data);
                }

            }
        }
        isAdding = false;
        loading.SetActiveMidLoading(false);
    }

    // 검색된 메이트 패널 생성
    private async void AddMatePanel(List<MateSearchData> _mateList)
    {
        if (_mateList == null || _mateList.Count == 0)
        {
            noResultIcon.SetActive(true);
            return;
        }
        noResultIcon.SetActive(false);

        BanList = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>().subDic["Ban_Page"].GetComponentInChildren<BanPageManager>().BanUserCodeList; ;
        for (int i = 0; i < _mateList.Count; i++)
        {
            bool bancheck = false;
            if (_mateList[i].userId == null)
                continue;

            for (int j = 0; j < BanList.Count; j++)
            {
                if (BanList[j] == _mateList[i].userCodeNo.ToString())
                {
                    bancheck = true;
                }
            }

            if (!bancheck)
            {
                // 메이트 패널 생성
                MateSearchPanel matePanel = InstantiatePanel();
                matePanelList.Add(matePanel);
                matePanel.nickname.text = _mateList[i].nickname;
                matePanel.userID.text = _mateList[i].userId;
                matePanel.userCodeNo = _mateList[i].userCodeNo;
                matePanel.profileImgPath = _mateList[i]?.profileImgPath?.SD;
                matePanel.isSearched = true;
                // 메이트 패널 데이터 뿌려주기
                UpdatePanel(_mateList[i], matePanel);
                // 프로필 이미지 가져오기
                GetMateImage(_mateList[i]?.profileImgPath?.SD, matePanel).Forget();
                // 이미 팔로우한 상태면 팔로잉 버튼 On
                CheckIsFollowing(matePanel, _mateList[i]);
            }
            if (i == _mateList.Count - 1)
                lastNo = _mateList[i].no;

        }

        // 레이아웃 업데이트
        UpdateLayout();
    }

    private void AddMatePanel(MateSearchData _mateData)
    {
        List<MateSearchData> list = new List<MateSearchData>() { _mateData };
        AddMatePanel(list);
    }

    // 메이트 패널 전부 삭제
    private void DeleteMatePanel()
    {
        for (int i = 0; i < matePanelList.Count; i++)
        {
            Destroy(matePanelList[i].gameObject);
        }
        matePanelList.Clear();
    }

    // 메이트 패널 오브젝트 생성
    private MateSearchPanel InstantiatePanel()
    {
        RectTransform obj = Instantiate(matePrefab, scrollRect.content.transform);
        obj.gameObject.SetActive(true);
        MateSearchPanel matePanel = obj.GetComponent<MateSearchPanel>();
        return matePanel;
    }

    // 이미 팔로우한 상태면 팔로잉 버튼 On
    private void CheckIsFollowing(MateSearchPanel _matePanel, MateSearchData _followingData)
    {
        // 자기자신일 경우 return
        if (_matePanel.userCodeNo.ToString() == MetaliveManager.Instance.user.userCodeNo)
            return;

        // 팔로잉 메이트가 없으면 return
        if (_followingData == null)
            return;

        if (_followingData.followingYn == "Y")
        {
            _matePanel.followButton.gameObject.SetActive(false);
            _matePanel.followingButton.gameObject.SetActive(true);
        }
    }

    // 메이트 패널 데이터 뿌려주기
    private void UpdatePanel(MateSearchData _mateSearchData, MatePanel _matePanel)
    {
        _matePanel.nickname.text = _mateSearchData.nickname;
        _matePanel.userID.text = string.IsNullOrEmpty(_mateSearchData.userId) ? "" : "@" + _mateSearchData.userId;
        _matePanel.userCodeNo = _mateSearchData.userCodeNo;
        _matePanel.isSearched = true;

    }

    // 레이아웃 업데이트
    private void UpdateLayout()
    {
        float height = matePrefab.rect.height;
        float spacing = 43.8f;
        float mateYPos = 0;

        for (int i = 0; i < matePanelList.Count; i++)
        {
            //mateYPos = i * (height + spacing);
            RectTransform panel = matePanelList[i].GetComponent<RectTransform>();
            panel.anchoredPosition = new Vector2(0, -mateYPos);
            mateYPos += (height + spacing);
        }

        scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (height + spacing) * matePanelList.Count);
    }

    // 메이트 프로필 이미지 뿌려주기
    private async UniTask GetMateImage(string _path, MatePanel _matePanel)
    {
        if (string.IsNullOrEmpty(_path))
            return;

        UnityWebRequest request;

        request = UnityWebRequestTexture.GetTexture(_path);
        await request.SendWebRequest();

        try
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            texture.name = $"MateSearch: Texture";
            textureDisposer.AddTexture(MainPage.ProfileMateSearch, texture);
            _matePanel.profileImage.texture = texture;
        }
        catch (System.Exception)
        {

        }
    }

    // 메이트가 20개 초과 시 스크롤을 내리면 더 불러오기
    public async void ShowMoreMate(Vector2 _value)
    {
        if (_value.y >= 0 || isAdding)
            return;

        isAdding = true;
        scrollRect.enabled = false;
        await GetSearchResult(searchKeyword);
        scrollRect.enabled = true;
    }
}
