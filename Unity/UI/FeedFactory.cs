/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 생성 및 삭제 기능
 */
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Metalive;
using System.Linq;

#region FeedsDataRoot Class

public class FeedsDataRoot
{
    public string result { get; set; }
    public List<FeedsData> data { get; set; }
    public string message { get; set; }
}

public class FeedsData
{
    public int no { get; set; }
    public int userCodeNo { get; set; }
    public string userId { get; set; }
    public string nickname { get; set; }
    public int feedNo { get; set; }
    public string feedText { get; set; }
    public string feedLocName { get; set; }
    public string feedLocLongitude { get; set; }
    public string feedLocLatitude { get; set; }
    public string profileImgPath { get; set; }
    public List<FeedsImg> feedImg { get; set; }
    public int commentCount { get; set; }
    public int feedLikeCount { get; set; }
    public int feedLikeStatus { get; set; }
    public string insertTs { get; set; }
    public string updateTs { get; set; }
}

public class FeedsImg
{
    public int feedImgNo { get; set; }
    public int feedNo { get; set; }
    public string imgPath { get; set; }
    public string imgLatitude { get; set; }
    public string imgLongitude { get; set; }
}

#endregion

public class FeedFactory : ContentUtil
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private MainSceneLoading loading;
    [SerializeField] private Image emptyImage;
    [SerializeField] private Texture defaultProfileTex;
    [SerializeField] private MainTextureDisposer texDisposer;
    [HideInInspector] public Dictionary<int, Feed> feedDic;
    [HideInInspector] public List<Feed> feedList;
    [HideInInspector] public int userCodeNo;
    public FeedsDataRoot FeedData { get; private set; }
    private RectTransform feedPrefab;
    private int lastFeedNo;
    private bool isAddingFeed;

    // 피드 텍스쳐 관리에 사용되는 변수
    private bool isOptimizing;
    private int onOffCount = 12; // 이 값을 통해 OnOff할 이미지 숫자 제어
    private int downTargetIndex = 0;
    private int upTargetIndex = 0;
    private int disposeStartIndex = 0;
    private int disposeLastIndex = 0;
    private int recoveryStartIndex = 0;
    private int recoveryLastIndex = 0;
    private async void Start()
    {
        isAddingFeed = true;
        feedPrefab = scrollRect.content.GetChild(0).GetComponent<RectTransform>();
        feedDic = new Dictionary<int, Feed>();
        feedList = new List<Feed>();
        MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        await UniTask.WaitUntil(() => canvasNav.subDic.Count != 0);
        FeedPost feedPost = canvasNav.subDic["FeedWrite_Page"].GetComponent<FeedPost>();
        feedPost.init += () =>
            {
                InitFeed();
                canvasNav.Push("FeedMain");
            };
    }

    // 피드 초기화
    private void DoReset()
    {
        isAddingFeed = true;
        lastFeedNo = 0;
        foreach (Feed feed in feedDic.Values)
        {
            Destroy(feed.gameObject);
        }
        for (int i = 0; i < feedList.Count; i++)
        {
            Destroy(feedList[i].gameObject);
        }
        feedDic.Clear();
        feedList.Clear();
        texDisposer.DisposePageTexture(MainPage.FeedMain);
        texDisposer.DisposePageTexture(MainPage.FeedDetail);
        texDisposer.DisposePageTexture(MainPage.FeedSearch);
        texDisposer.DisposePageTexture(MainPage.FeedWrite);

        isOptimizing = false;
        downTargetIndex = 0;
        upTargetIndex = 0;
        disposeStartIndex = 0;
        disposeLastIndex = 0;
        recoveryStartIndex = 0;
        recoveryLastIndex = 0;
    }

    // 피드 첫 생성
    public async void InitFeed()
    {
        MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        if (canvasNav.subUIStack.Peek().name == "FeedMain")
        {
            loading.SetActiveMidLoading(true, InitFeed);

        }
        else
        {
            loading.SetActiveMidLoading(false);
        }

        DoReset();

        bool isFeed = await GetFeedData();
        loading.SetActiveMidLoading(false);
        if (isFeed)
        {
            await CreateFeed();
            UpdateLayout();
            isAddingFeed = false;
        }
        else
        {
            emptyImage.gameObject.SetActive(true);
        }
    }

    // 피드 가져오기
    private async UniTask<bool> GetFeedData()
    {
        string uri = null;
        if (lastFeedNo == 0)
        {
            uri = $"https://{Metalive.Setting.Server.api}/api/feed?range=4";
        }
        else
        {
            uri = $"https://{Metalive.Setting.Server.api}/api/feed?lastNo={lastFeedNo}&range=4";
        }

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
                Debug.Log("feed/\n" + request.downloadHandler.text);
                FeedData = JsonConvert.DeserializeObject<FeedsDataRoot>(request.downloadHandler.text);
                if (FeedData.data == null || FeedData.data.Count == 0)
                    return false;

                else
                {
                    lastFeedNo = FeedData.data[FeedData.data.Count - 1].no;
                    return true;
                }
            }
        }
    }

    public void UpdateFeedCommentCount(int feedNo, int delCount)
    {
        if (feedNo == 0)
            return;

        int commentCount = feedDic[feedNo].commentCount;
        feedDic[feedNo].commentCountText.text = "댓글 " + (commentCount - delCount);
    }


    // 4장 이상 업로드 시 추가 Asign
    public async void ShowMoreFeed(Vector2 _value)
    {
        if (_value.y >= 0 || isAddingFeed)
            return;

        isAddingFeed = true;
        loading.SetActiveBotLoading(true, InitFeed);
        bool isFeed = await GetFeedData();
        if (isFeed)
        {
            scrollRect.enabled = false;
            await CreateFeed();
            UpdateLayout();
            scrollRect.enabled = true;
            loading.SetActiveBotLoading(false);
        }
        else
        {
            loading.SetActiveBotLoading(false);
        }
    }


    // 피드 동적 OnOff
    public void OnOffTexture()
    {
        if (isOptimizing)
            return;

        if (downTargetIndex == 0)
        {
            downTargetIndex = (onOffCount * 2) - 5;
            disposeLastIndex = onOffCount;
            recoveryStartIndex = -onOffCount;
        }

        float feedHeight = feedPrefab.rect.height + 30; // Spacing = 30
        float contentYPos = scrollRect.content.anchoredPosition.y;
        float downTargetFeedYPos = feedHeight * downTargetIndex;
        float upTargetFeedYPos = feedHeight * upTargetIndex;

        // 아래로 내리기
        if (contentYPos > downTargetFeedYPos)
        {
            //Debug.Log("아래로 내리기");
            isOptimizing = true;

            // 위로 올렸다가 다시 아래로 내릴 때
            if (recoveryLastIndex + onOffCount == disposeStartIndex)
            {
                disposeStartIndex -= onOffCount;
                disposeLastIndex -= onOffCount;
            }

            // 위로 올렸다가 다시 아래로 내릴 때
            if (feedList.Count >= disposeLastIndex + (onOffCount * 2))
            {
                RecoveryTexture(disposeStartIndex + (onOffCount * 2), disposeLastIndex + (onOffCount * 2), true);
            }
            DisposeTexture(disposeStartIndex, disposeLastIndex);

            recoveryStartIndex += onOffCount;
            recoveryLastIndex += onOffCount;
            disposeStartIndex += onOffCount;
            disposeLastIndex += onOffCount;
            downTargetIndex += onOffCount;
            upTargetIndex += onOffCount;
        }
        // 위로 올리기
        else if (contentYPos > 0 && contentYPos < upTargetFeedYPos)
        {
            //Debug.Log("위로 올리기");
            isOptimizing = true;

            // 아래에 피드가 40개 이상 있을 때
            // 20개만 있을 경우 Dispose하지 않는다.
            if (disposeStartIndex != recoveryLastIndex)
            {
                DisposeTexture(disposeStartIndex, disposeLastIndex);
                disposeStartIndex -= onOffCount;
                disposeLastIndex -= onOffCount;
            }
            RecoveryTexture(recoveryStartIndex, recoveryLastIndex, false);

            recoveryStartIndex -= onOffCount;
            recoveryLastIndex -= onOffCount;
            downTargetIndex -= onOffCount;
            upTargetIndex -= onOffCount;
        }
    }

    // 텍스쳐 Dispose
    private void DisposeTexture(int _startIndex, int _lastIndex)
    {
        //Debug.Log("DisposeTexture");

        for (int i = _startIndex; i < _lastIndex; i++)
        {
            int imageLength = feedList[i].images.Where(x => x.texture != null).Count();
            for (int j = 0; j < imageLength; j++)
            {
                DestroyImmediate(feedList[i].images[j].texture);
                feedList[i].gameObject.SetActive(false);
            }
            DestroyImmediate(feedList[i].profileImage.texture);
        }
        isOptimizing = false;
    }

    // 텍스쳐 재로드
    private void RecoveryTexture(int _startIndex, int _lastIndex, bool isDown)
    {
        if (_startIndex < 0)
            return;

        //Debug.Log("RecoveryTexture");

        if (isDown)
        {
            for (int i = _startIndex; i < _lastIndex; i++)
            {
                GetFeedImage(feedList[i], feedList[i].imagePaths).Forget();
                GetProfileImage(feedList[i], feedList[i].profileImagePath).Forget();
                feedList[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = _lastIndex - 1; i >= _startIndex; i--)
            {
                GetFeedImage(feedList[i], feedList[i].imagePaths).Forget();
                GetProfileImage(feedList[i], feedList[i].profileImagePath).Forget();
                feedList[i].gameObject.SetActive(true);
            }
        }
        isOptimizing = false;
    }

    // 다른 페이지 이동 후 돌아왔을 때 텍스쳐 재로드
    public void RecoveryCurTexture()
    {
        if (feedDic.Count == 0 || feedList.Count == 0)
            return;

        Feed startFeed = feedList.First((x) => x.gameObject.activeSelf);
        Feed lastFeed = feedList.Last((x) => x.gameObject.activeSelf);
        int startIndex = feedList.IndexOf(startFeed);
        int lastIndex = feedList.IndexOf(lastFeed) + 1;

        RecoveryTexture(startIndex,lastIndex,false);
    }

    // 피드 새로고침
    public async void RefreshFeed(Vector2 _value)
    {
        if (isAddingFeed)
            return;

        else if (_value.y >= 1.1)
        {
            InitFeed();
            isAddingFeed = true;
            await UniTask.WaitUntil(() => loading.gameObject.activeSelf == false);
            isAddingFeed = false;
        }
    }

    // 피드 생성
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    private async UniTask CreateFeed()
    {
        if (FeedData.data == null || FeedData.data.Count == 0)
            return;

        else
            emptyImage.gameObject.SetActive(false);

        for (int i = 0; i < FeedData.data.Count; i++)
        {
            RectTransform obj = null;
            try
            {
                if (i == 4)
                    break;

                obj = Instantiate(feedPrefab, scrollRect.content.transform);
                Feed feed = obj.GetComponent<Feed>();
                obj.gameObject.SetActive(true);

                // 피드 데이터 뿌려주기
                UpdateFeedData(feed, FeedData.data[i]);
                if (feedDic.ContainsKey(feed.contentNo))
                {
                    Destroy(obj.gameObject);
                    continue;
                }

                // 피드 프로필 이미지
                GetProfileImage(feed, FeedData.data[i].profileImgPath).Forget();
                // 피드 이미지
                GetFeedImage(feed, FeedData.data[i].feedImg).Forget();

                feedDic.Add(feed.contentNo, feed);
                feedList.Add(feed);
            }
            catch
            {
                obj.gameObject.SetActive(false);
            }

        }
        isAddingFeed = false;
    }


    // 레이아웃 업데이트
    public void UpdateLayout()
    {
        float height = feedPrefab.rect.height;
        float spacing = 30;

        for (int j = 0; j < feedList.Count; j++)
        {
            float y = j * (height + spacing);
            if (feedList[j] == null)
                continue;

            feedList[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -y);
        }
        scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, feedDic.Count * (height + spacing));
    }

    private void UpdateFeedData(Feed _feed, FeedsData feedData)
    {
        _feed.profileImage.GetComponentInParent<DM_info>().UsercodeNum = feedData.userCodeNo;
        _feed.contentText.text = ConvertToLink(feedData.feedText);
        _feed.content = feedData.feedText;
        _feed.contentNo = feedData.feedNo;
        _feed.userCodeNo = feedData.userCodeNo;
        _feed.commentCountText.text = feedData.commentCount == 0 ? "댓글 0" : $"댓글 {ConvertNumberToKM(feedData.commentCount)}";
        _feed.nicknameTxt.text = feedData.nickname;
        _feed.nickname = feedData.nickname;
        _feed.userIdTxt.text = "@" + feedData.userId;
        _feed.userId = feedData.userId;
        _feed.likeCount = feedData.feedLikeCount == 0 ? 0 : feedData.feedLikeCount;
        _feed.date = feedData.insertTs;
        _feed.placeName = feedData.feedLocName;
        _feed.imageCount = feedData.feedImg.Count;
        _feed.imageCountText.text = feedData.feedImg.Count.ToString();
        _feed.likeToggle.isOn = feedData.feedLikeStatus == 0 ? false : true;
        _feed.addressText.text = feedData.feedLocName == null ? "" : feedData.feedLocName;
        _feed.commentCount = feedData.commentCount;
        _feed.imagePaths = feedData.feedImg;
        _feed.profileImagePath = feedData.profileImgPath;
    }

    // 피드 프로필 사진 가져오기
    private async UniTask GetProfileImage(Feed _feed, string _path)
    {

        // path가 null이면 return
        if (string.IsNullOrEmpty(_path))
        {
            _feed.profileImage.texture = defaultProfileTex;
            return;
        }

        UnityWebRequest request;

        request = UnityWebRequestTexture.GetTexture(_path);
        await request.SendWebRequest();

        try
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            _feed.profileImage.texture = texture;
            texDisposer.AddTexture(MainPage.FeedMain, texture);
            texture.name = $"FeedFactory1: Texture";
        }
        catch (System.Exception)
        {

        }
    }

    // 피드 사진 가져오기
    private async UniTask GetFeedImage(Feed _feed, List<FeedsImg> _feedImg)
    {
        for (int i = 0; i < _feedImg.Count; i++)
        {
            string path = _feedImg[i].imgPath;

            // path가 null이면 return
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            UnityWebRequest request;

            request = UnityWebRequestTexture.GetTexture(path);
            await request.SendWebRequest();
            try
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                texDisposer.AddTexture(MainPage.FeedMain, texture);
                texture.name = $"FeedFactory2: Texture";
                _feed.images[i].color = Color.white;
                _feed.images[i].texture = texture;

                Vector2 resizedImageSize = new Vector2(1284, 732);
                _feed.images[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resizedImageSize.x);
                _feed.images[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, resizedImageSize.y);

            }
            catch (System.Exception)
            {

            }
        }
        DestroyEmptyImage(_feed, _feedImg);
    }


    // 빈 이미지는 삭제 (총 10장 중 피드 이미지가 6장이라면 4장 삭제)
    private void DestroyEmptyImage(Feed _feed, List<FeedsImg> _feedImg)
    {
        if (_feed.images.Count == _feedImg.Count)
            return;

        int dbCount = _feedImg.Count;
        int imageCount = _feed.images.Count;
        for (int i = dbCount; i < imageCount; i++)
        {
            Destroy(_feed.images[i].gameObject);
        }
        for (int i = imageCount - 1; i >= dbCount; i--)
        {
            _feed.images.RemoveAt(i);
        }
    }
}
