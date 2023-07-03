/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 상세페이지 정보 업데이트
 */
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Metalive;

#region FeedRoot Class

public class FeedRoot
{
    public string result { get; set; }
    public FeedData data { get; set; }
    public string message { get; set; }
}

public class FeedData
{
    public int userCodeNo { get; set; }
    public string userId { get; set; }
    public string nickname { get; set; }
    public int feedNo { get; set; }
    public string feedText { get; set; }
    public string feedLocName { get; set; }
    public string feedLocLongitude { get; set; }
    public string feedLocLatitude { get; set; }
    public string profileImgPath { get; set; }
    public List<FeedImg> feedImg { get; set; }
    public int commentCount { get; set; }
    public int feedLikeCount { get; set; }
    public int feedLikeStatus { get; set; }
    public string insertTs { get; set; }
    public string updateTs { get; set; }
}

public class FeedComment
{
    public int commentCount;
    public int userCodeNo;
    public string nickname;
    public string feedCommentText;
    public string insertTs;
    public string updateTs;
}

public class FeedImg
{
    public int feedImgNo { get; set; }
    public int feedNo { get; set; }
    public string imgPath { get; set; }
    public string imgLatitude { get; set; }
    public string imgLongitude { get; set; }
}
#endregion

public class FeedDetailPage : ContentUtil
{
    public HorizontalScrollSnap scrollSnap;
    public ScrollRect imageScrollRect;
    public MainSceneLoading loading;
    public RectTransform emptySet;
    public Image backBtnImg;
    [HideInInspector] public FeedDetailInfo pageInfo;
    [HideInInspector] public List<RawImage> imageList;
    [HideInInspector] public List<Texture> feedImageTextures;
    private RectTransform imagePrefab;
    private FeedRoot feedData;
    private int preFeedNo;
    public List<int> FeedList = new List<int>();
    [SerializeField] private MainTextureDisposer texDisposer;
    private void OnEnable()
    {
        AlarmManager am = GameObject.FindGameObjectWithTag("GameController").GetComponent<AlarmManager>();
        am.feedAction = GoToFeedPageFromAlarm;
    }

    private void Start()
    {
        pageInfo = GetComponent<FeedDetailInfo>();
        imagePrefab = imageScrollRect.transform.GetChild(0).GetComponent<RectTransform>();
        imageList = new List<RawImage>();
    }

    // 초기화
    public async void Init(bool _isLoading)
    {
        // 로딩 On
        texDisposer.DisposePageTexture(MainPage.FeedDetail);
        loading.SetActiveMidLoading(_isLoading, () => UpdatePage(pageInfo.feedNo));

        pageInfo.placeName.text = "";
        pageInfo.contentText.text = "";
        pageInfo.locName.text = "";
        pageInfo.commentCountText.text = "";
        pageInfo.nicknameTxt.text = "";
        pageInfo.maxImageCount.text = "";
        pageInfo.userIDTxt.text = "";
        pageInfo.likeCount.text = "";
        pageInfo.date.text = "";
        pageInfo.profileImage.texture = null;
        pageInfo.commentInput.GetComponent<Image>().color = new Color(0.4392f, 0.4392f, 0.4392f, 0.2980f);
        pageInfo.slangNoticeTxt.gameObject.SetActive(false);
        emptySet.gameObject.SetActive(false);
        pageInfo.pageScrollRect.gameObject.SetActive(false);
        backBtnImg.color = Color.white;

        for (int i = 0; i < imageList.Count; i++)
        {
            imageList[i].gameObject.SetActive(false);
            Destroy(imageList[i].gameObject);
        }
        imageList.Clear();

        await UniTask.WaitUntil(() => scrollSnap.enabled);
        scrollSnap.ChangePage(0);
        texDisposer.DisposePageTexture(MainPage.FeedDetail);
    }

    // 페이지 업데이트(피드 클릭 시)
    public void UpdatePage(Feed _feed)
    {
        pageInfo.feedNo = _feed.contentNo;
        UpdatePage(_feed.contentNo);
        SetPullupInfo();
    }

    // 내 프로필의 피드를 클릭했을 때
    public void OnclickProfilefeed(MyFeedThumbnail mfeed)
    {
        UpdatePage(mfeed.feedNo);
        pageInfo.feedNo = mfeed.feedNo;
    }

    public void OnclickTargetProfilefeed(TargetThumbnail tfeed)
    {
        UpdatePage(tfeed.feedNo);
        pageInfo.feedNo = tfeed.feedNo;
    }

    // 알람에서 피드 상세페이지로 이동
    private async void GoToFeedPageFromAlarm(int _feedNo)
    {
        MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        if (canvasNav.subUIStack.Peek().name == "FeedDetail_Page")
            canvasNav.pop();

        loading.SetActiveMidLoadingWithBG(true, () => GoToFeedPageFromAlarm(_feedNo));
        pageInfo.feedNo = _feedNo;

        // 초기화
        Init(true);
        // 지워지거나 데이터가 없을 경우
        if (await GetFeedData(_feedNo) == false)
        {
            canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
            canvasNav.Push("FeedDetail_Page");
            loading.SetActiveMidLoadingWithBG(false);
            emptySet.gameObject.SetActive(true);
            backBtnImg.color = MetaLiveColor.portage;
        }
        // 데이터가 있을 경우
        else
        {
            canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
            canvasNav.Push("FeedDetail_Page");
            // 페이지 정보 업데이트
            UpdateFeedInfo();
            // 댓글 업데이트
            UpdateComment(_feedNo);
            // 이미지 업데이트
            UpdateFeedImage();
            // 레이아웃 업데이트
            UpdateLayout();
            loading.SetActiveMidLoadingWithBG(false);
        }
    }

    // 페이지 업데이트(검색된 피드 썸네일 시)
    public async void UpdatePage(int _feedNo)
    {
        preFeedNo = FeedList.Count == 0 ? _feedNo : FeedList[FeedList.Count - 1];
        FeedList.Add(_feedNo);
        // 초기화
        Init(true);
        // 페이지 정보 DB에서 가져오기
        if (await GetFeedData(_feedNo) == false)
        {
            loading.SetActiveMidLoadingWithBG(false);
            emptySet.gameObject.SetActive(true);
            backBtnImg.color = MetaLiveColor.portage;

            return;
        }

        // 페이지 정보 업데이트
        UpdateFeedInfo();
        // 댓글 업데이트
        UpdateComment(_feedNo);
        // 이미지 업데이트
        UpdateFeedImage();
        // 레이아웃 업데이트
        UpdateLayout();
    }

    public async void ListUpdatePage()
    {
        if (FeedList.Count >= 2)
        {
            int _feedNo = FeedList[FeedList.Count - 2];
            // 초기화
            Init(true);
            // 페이지 정보 DB에서 가져오기
            if (await GetFeedData(_feedNo) == false)
            {
                loading.SetActiveMidLoadingWithBG(false);
                emptySet.gameObject.SetActive(true);
                backBtnImg.color = MetaLiveColor.portage;

                return;
            }
            // 페이지 정보 업데이트
            UpdateFeedInfo();
            // 댓글 업데이트
            UpdateComment(_feedNo);
            // 이미지 업데이트
            UpdateFeedImage();
            // 레이아웃 업데이트
            UpdateLayout();
            FeedList.Remove(FeedList[FeedList.Count - 1]);
        }
    }

    public async UniTask RecoveryCurFeedImage()
    {
        if (FeedList.Count == 0)
            return;

        int curFeedNo = FeedList[FeedList.Count - 1];
        if (preFeedNo == curFeedNo)
        {
            for (int i = 0; i < feedData.data.feedImg.Count; i++)
            {
                imageList[i].texture = await UpdateFeedImageAction(feedData.data.feedImg[i].imgPath);
            }
        }
        pageInfo.profileImage.texture = string.IsNullOrEmpty(feedData.data.profileImgPath) ? pageInfo.defaultProfileTex : await GetImage(feedData.data.profileImgPath);

    }

    // 피드 데이터 가져오기
    public async UniTask<bool> GetFeedData(int _feedNo)
    {
        string uri = $"https://{Metalive.Setting.Server.api}/api/feed/by-feed-no?feedNo={_feedNo}";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            request.SetRequestHeader("Authorization", "Bearer " + Setting.User.token);
            await request.SendWebRequest();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSceneManager>().CheckDuplicateLogin(request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("피드 데이터 result: " + request.result);
                return false;
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                feedData = JsonConvert.DeserializeObject<FeedRoot>(request.downloadHandler.text);
                if (feedData.data == null)
                    return false;
                else
                    return true;
            }
        }
    }

    // 피드 정보 업데이트
    private async void UpdateFeedInfo()
    {
        pageInfo.pageScrollRect.gameObject.SetActive(true); // 최상위 페이지 ScrollRect
        FeedData data = feedData.data;
        pageInfo.likeToggle.feedNo = data.feedNo;
        pageInfo.profileImage.texture = string.IsNullOrEmpty(data.profileImgPath) ? pageInfo.defaultProfileTex : await GetImage(data.profileImgPath);
        pageInfo.profileImage.GetComponentInParent<DM_info>().UsercodeNum = data.userCodeNo;
        pageInfo.nicknameTxt.text = data.nickname;
        pageInfo.nickname = data.nickname;
        pageInfo.userIDTxt.text = "@" + data.userId;
        pageInfo.userId = data.userId;
        pageInfo.userCodeNo = data.userCodeNo;
        pageInfo.date.text = ChangeTimeList(data.insertTs);
        pageInfo.contentNo = data.feedNo;
        pageInfo.contentText.text = data.feedText;
        pageInfo.locName.text = data.feedLocName == null ? "   " : data.feedLocName;
        pageInfo.maxImageCount.text = data.feedImg.Count.ToString();
        pageInfo.commentCountText.text = $"댓글 {ConvertNumberToKM(data.commentCount)}";
        pageInfo.commentCount = data.commentCount;
        pageInfo.likeCount.text = ConvertNumberToKM(data.feedLikeCount);
        pageInfo.likeToggle.detailLikeToggle.isOn = data.feedLikeStatus == 1 ? true : false;
        pageInfo.likeToggle.GetComponentInChildren<TMP_Text>().text = data.feedLikeCount.ToString();
    }

    // 댓글 업데이트
    private void UpdateComment(int _feedNo)
    {
        FeedCommentFactory commentManager = GetComponent<FeedCommentFactory>();
        // 댓글 초기화 및 로딩 Off
        commentManager.Init();
        loading.SetActiveMidLoading(false);

        commentManager.CreateFeedComment(_feedNo);
    }

    // 이미지 업데이트
    private async void UpdateFeedImage()
    {
        int imageCount = feedData.data.feedImg.Count;
        for (int i = 0; i < imageCount; i++)
        {
            FeedImg imageData = feedData.data.feedImg[i];
            RawImage obj = Instantiate(imagePrefab, imageScrollRect.content.transform).GetComponent<RawImage>();
            obj.gameObject.SetActive(true);
            imageList.Add(obj);
            string path = imageData.imgPath;
            obj.texture = path == null ? null : await UpdateFeedImageAction(path);
            obj.color = Color.white;

            // 길찾기 버튼 On
            if (!string.IsNullOrEmpty(imageData.imgLatitude) && !string.IsNullOrEmpty(imageData.imgLongitude) && imageData.imgLatitude != "0.0" && imageData.imgLongitude != "0.0")
            {
                obj.transform.GetChild(0).gameObject.SetActive(true);
                GeoData geoData = obj.GetComponentInChildren<GeoData>();
                geoData.latitude = imageData.imgLatitude;
                geoData.longitude = imageData.imgLongitude;
            }
        }

        // 레이아웃 업데이트
        await UniTask.Yield();
        imageScrollRect.GetComponent<HorizontalScrollSnap>().DistributePages();
    }

    // 레이아웃 업데이트
    private async void UpdateLayout()
    {
        await UniTask.WaitUntil(() => !string.IsNullOrEmpty(pageInfo.contentText.text));
        pageInfo.contentLayout.enabled = false;
        await UniTask.Yield();
        pageInfo.contentLayout.enabled = true;
    }

    // 풀업 정보 셋팅
    private void SetPullupInfo()
    {
        ReportManager report = GameObject.FindGameObjectWithTag("GameController").GetComponent<ReportManager>();
        pageInfo.pullupButton.onClick.AddListener(() =>
        {
            report.GetTargetInfo(pageInfo.feedNo, ReportType.feed);
            //FindObjectOfType<FeedMessageBtn>().targetNo = pageInfo.userCodeNo;
        });
    }

    // 텍스쳐 불러오기
    private async UniTask<Texture2D> UpdateFeedImageAction(string _url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(_url))
        {
            await request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            texture.name = $"FeedDetailPage1: Texture";
            feedImageTextures.Add(texture);

            return texture;
        }
    }

    // 피드를 올린 사람의 이미지 가져오기
    private async UniTask<Texture> GetImage(string _path)
    {
        string url = _path;
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            await request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            texture.name = $"FeedDetailPage2: Texture";
            texDisposer.AddTexture(MainPage.FeedDetail, texture);

            return texture;
        }

    }

    public void DisposeFeedImageTexture()
    {
        for (int i = 0; i < feedImageTextures.Count; i++)
        {
            DestroyImmediate(feedImageTextures[i]);
        }
        feedImageTextures.Clear();
    }

    public void OnClickBackBtn()
    {
        if (FeedList.Count == 1)
        {
            FeedList.Clear();
        }
    }

    public void FeedListClear()
    {
        FeedList.Clear();
    }
}
