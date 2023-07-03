/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 댓글 생성 및 삭제 기능
 */
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using Metalive;
using DG.Tweening;

#region SlangDataRoot Class
public class SlangDataRoot
{
    public string result;
    public List<string> data;
    public string message;
}
#endregion 

#region CommentSlangDataRoot class

public class CommentSlangDataRoot
{
    public string result;
    public string data;
    public string message;
}

#endregion

#region FeedCommentDataRoot Class
public class FeedNestedCommentData
{
    public string userId;
    public string nickname;
    public int userCodeNo;
    public int feedCommentNo;
    public int feedNo;
    public string delYn;
    public string feedCommentText;
    public int feedParent;
    public int feedDepth;
    public string profileImgPath;
    public string insertTs;
    public string updateTs;
}

public class FeedCommentData
{
    public string userId;
    public int userCodeNo;
    public string delYn;
    public string nickname;
    public int feedCommentNo;
    public int feedNo;
    public string feedCommentText;
    public int feedParent;
    public int feedDepth;
    public string profileImgPath;
    public string insertTs;
    public string updateTs;
    public List<FeedNestedCommentData> getFeedCommentSs;
}

public class FeedCommentDataRoot
{
    public string result;
    public List<FeedCommentData> data;
    public string message;
}
#endregion

#region  CommentPostData Class

[System.Serializable]
public class CommentPostData
{
    public int feedNo;
    public string feedCommentText;
    public int feedParent;
    public int feedDepth;
    public List<string> feedTag;

    public CommentPostData(int _feedNo, string _feedCommentText, int _feedParent, int _feedDepth, List<string> _feedTag)
    { feedNo = _feedNo; feedCommentText = _feedCommentText; feedParent = _feedParent; feedDepth = _feedDepth; feedTag = _feedTag; }
}
#endregion

public class FeedCommentFactory : CommentFactory
{
    [SerializeField] private FeedDetailInfo detailInfo;
    [SerializeField] private RectTransform pageContent;
    [SerializeField] MainSceneLoading loading;
    [SerializeField] CommentInputAnimation inputAnim;
    [HideInInspector] public int feedNo;
    private TMP_InputField commentInput;
    private TMP_InputField nestedCommentInput;


    // 피드 댓글 생성
    public async void CreateFeedComment(int _feedNo)
    {

        feedNo = _feedNo;
        FeedCommentDataRoot feedCommentData = await GetFeedCommentData(_feedNo);
        UpdateFeedCommnetData(feedCommentData);
        await CreateComment();
        SetPullupInfo();
        UpdateLayout();
    }

    // 댓글을 만들 클래스 데이터(CommentDataRoot)에 정보 할당
    private void UpdateFeedCommnetData(FeedCommentDataRoot _feedCommentData)
    {
        if (_feedCommentData.data.Count == 0)
            UpdateLayout();

        List<CommentData> commentDataList = new List<CommentData>();
        for (int i = 0; i < _feedCommentData.data.Count; i++)
        {
            List<NestedCommentData> nestedCommentDataList = new List<NestedCommentData>();
            CommentData commentData = new CommentData();
            int count = _feedCommentData.data[i].getFeedCommentSs.Count;
            for (int j = 0; j < count; j++)
            {
                NestedCommentData nestedCommentData = new NestedCommentData();

                nestedCommentData.userCodeNo = _feedCommentData.data[i].getFeedCommentSs[j].userCodeNo;
                nestedCommentData.contentText = _feedCommentData.data[i].getFeedCommentSs[j].feedCommentText;
                nestedCommentData.nickname = _feedCommentData.data[i].getFeedCommentSs[j].nickname;
                nestedCommentData.userID = _feedCommentData.data[i].getFeedCommentSs[j].userId;
                nestedCommentData.date = _feedCommentData.data[i].getFeedCommentSs[j].insertTs;
                nestedCommentData.parentNo = _feedCommentData.data[i].getFeedCommentSs[j].feedParent;
                nestedCommentData.postingNo = _feedCommentData.data[i].getFeedCommentSs[j].feedNo;
                nestedCommentData.delYn = _feedCommentData.data[i].getFeedCommentSs[j].delYn;
                nestedCommentData.commentNo = _feedCommentData.data[i].getFeedCommentSs[j].feedCommentNo;
                nestedCommentData.profileImgPath = _feedCommentData.data[i].getFeedCommentSs[j].profileImgPath;
                nestedCommentDataList.Add(nestedCommentData);
            }

            commentData.userCodeNo = _feedCommentData.data[i].userCodeNo;
            commentData.contentText = _feedCommentData.data[i].feedCommentText;
            commentData.nickname = _feedCommentData.data[i].nickname;
            commentData.delYn = _feedCommentData.data[i].delYn;
            commentData.userID = _feedCommentData.data[i].userId;
            commentData.userCodeNo = _feedCommentData.data[i].userCodeNo;
            commentData.date = _feedCommentData.data[i].insertTs;
            commentData.profileImgPath = _feedCommentData.data[i].profileImgPath;
            commentData.nestedCount = _feedCommentData.data[i].getFeedCommentSs.Count;
            commentData.commentNo = _feedCommentData.data[i].feedCommentNo;
            commentData.postingNo = _feedCommentData.data[i].feedNo;
            commentDataList.Add(commentData);
            commentData.nestedCommentList = nestedCommentDataList;
        }
        commentData.commnetList = commentDataList;
    }

    // 댓글 생성
    public override async void AddComment(TMP_InputField _input)
    {
        if (string.IsNullOrEmpty(_input.text) || string.IsNullOrWhiteSpace(_input.text))
            return;

        commentInput = _input;
        loading.SetActiveMidLoading(true, () => AddComment(_input));
        string inputTxt = commentInput.text;
        CommentPostData postData = new CommentPostData(feedNo, inputTxt, 0, 0, HashTagSplit(inputTxt));
        await PostComment(postData, false);

        loading.SetActiveMidLoading(false);

        base.AddComment(commentInput);
        commentInput.text = "";

        // 댓글 넘버 업데이트
        FeedCommentDataRoot feedCommentData = await GetFeedCommentData(feedNo);
        List<FeedNestedCommentData> nestedCommentList = null;

        if (feedCommentData.data.Count == 0)
        {
            int index = commentList.Count - 1;
            commentList[index].commentNo = feedCommentData.data[0].feedCommentNo;
        }
        else
        {
            if (feedCommentData.data[0].getFeedCommentSs != null)
            {
                nestedCommentList = feedCommentData.data[0].getFeedCommentSs;
            }

            if (nestedCommentList == null || nestedCommentList.Count == 0)
            {
                int index = 0;
                commentList[index].commentNo = feedCommentData.data[0].feedCommentNo;
            }
            else
            {
                int index = 0;
                commentList[index].commentNo = commentNo = nestedCommentList[0].feedCommentNo;
            }
        }

        UpdateLayout();
        UpdateCommentCount(1);
        //DoCommentAnimation();
    }

    // 대댓글 생성
    public override async void AddNestedComment(Comment _comment)
    {
        if (string.IsNullOrEmpty(_comment.input.text) || string.IsNullOrWhiteSpace(_comment.input.text))
            return;

        nestedCommentInput = _comment.input;
        loading.SetActiveMidLoading(true, () => AddNestedComment(_comment));
        string inputTxt = nestedCommentInput.text;
        Comment selectedComment = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Comment>();
        int parentNo = selectedComment.commentNo;
        CommentPostData postData = new CommentPostData(feedNo, inputTxt, parentNo, 1, HashTagSplit(inputTxt));
        await PostComment(postData, true);

        loading.SetActiveMidLoading(false);
        base.AddNestedComment(_comment);
        nestedCommentInput.text = "";

        // 레이아웃 업데이트
        await UniTask.Yield();
        commentContent.GetComponent<VerticalLayoutGroup>().enabled = false;
        commentContent.GetComponent<VerticalLayoutGroup>().enabled = true;
        UpdateLayout();
        UpdateCommentCount(1);
        DoNestedCommentAnimation(selectedComment);
    }

    // 피드 댓글 가져오기
    private async UniTask<FeedCommentDataRoot> GetFeedCommentData(int _feedNo)
    {
        string uri = $"https://{Metalive.Setting.Server.api}/api/feed/comment?feedNo={_feedNo}";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            request.SetRequestHeader("Authorization", "Bearer " + Setting.User.token);
            await request.SendWebRequest();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSceneManager>().CheckDuplicateLogin(request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("피드 댓글 데이터 result: " + request.result);
                return null;
            }
            else
            {
                Debug.Log("feedComment\n" + request.downloadHandler.text);
                FeedCommentDataRoot commentData = JsonConvert.DeserializeObject<FeedCommentDataRoot>(request.downloadHandler.text);
                return commentData;
            }
        }
    }

    // Set 댓글 타입
    private void SetPullupInfo()
    {
        ReportManager report = GameObject.FindGameObjectWithTag("GameController").GetComponent<ReportManager>();
        for (int i = 0; i < commentList.Count; i++)
        {
            commentList[i].type = CommentEditType.feed;

            int targetNo = commentList[i].commentNo;
            commentList[i].pullupButton.onClick.AddListener(() =>
            {
                report.GetTargetInfo(targetNo, ReportType.feedComment);
            });
        }
    }

    // 댓글 수 업데이트
    public async void UpdateCommentCount(int _value)
    {
        int commentCount = detailInfo.commentCount;
        detailInfo.commentCountText.text = "댓글 " + (commentCount + _value);
        detailInfo.commentCount += _value;

        var canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        var feedDic = canvasNav.subDic["FeedMain"].GetComponent<FeedFactory>().feedDic;
        await UniTask.WaitUntil(() => feedDic.Count != 0);
        if (_value > 0)
        {
            feedDic[feedNo].commentCount += _value;
        }
        else
        {
            //Debug.Log(feedDic[feedNo].commentCount);
            feedDic[feedNo].commentCount += _value;
            //Debug.Log(feedDic[feedNo].commentCount);
        }

        feedDic[feedNo].commentCountText.text = "댓글 " + (feedDic[feedNo].commentCount);
    }


    // 댓글 POST
    async UniTask<bool> PostComment(CommentPostData postData, bool isNestedComment)
    {
        string json = JsonConvert.SerializeObject(postData);
        Debug.Log(json);
        string url = $"https://{Metalive.Setting.Server.api}/api/feed/comment";
        using (UnityWebRequest request = UnityWebRequest.Post(url, json))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + Setting.User.token);

            await request.SendWebRequest();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSceneManager>().CheckDuplicateLogin(request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)

            {
                Debug.Log(request.error);
                return false;
            }

            else
            {
                Debug.Log($"댓글의 값 POST\n" + request.downloadHandler.text);

                if (request.downloadHandler.text.Contains("비속어") || request.downloadHandler.text.Contains("*"))
                {
                    CommentSlangDataRoot slangData = JsonConvert.DeserializeObject<CommentSlangDataRoot>(request.downloadHandler.text);
                    ChangeSlang(slangData, isNestedComment);

                    return false;
                }

                else
                {
                    return true;
                }
            }

        }
    }

    // 비속어 *로 표시하기
    private void ChangeSlang(CommentSlangDataRoot _slangData, bool isNestedComment)
    {
        string changedText = _slangData.data;

        if (isNestedComment)
        {
            nestedCommentInput.text = changedText;
        }
        else
        {
            commentInput.text = changedText;
        }
    }

    // 댓글 페이지 이동 애니메이션
    private async void DoCommentAnimation()
    {
        RectTransform canvasRt = transform.root.GetComponent<RectTransform>();
        if (commentContent.rect.height < 1800)
        {
            await UniTask.Delay(100);
            pageContent.DOPause();
            pageContent.DOAnchorPosY(0, 0.5f).onComplete = () => inputAnim.DoReset();
        }
        else
        {
            await UniTask.Delay(100);
            pageContent.DOPause();
            inputAnim.DoReset();
            pageContent.DOAnchorPosY(pageContent.rect.height - canvasRt.rect.height, 0.5f);
        }

    }

    // 대댓글 페이지 이동 애니메이션
    private async void DoNestedCommentAnimation(Comment _comment)
    {
        RectTransform canvasRt = transform.root.GetComponent<RectTransform>();
        if (commentContent.rect.height < 1800)
        {
            await UniTask.Delay(100);
            pageContent.DOPause();
            pageContent.DOAnchorPosY(0, 0.5f).onComplete = () => inputAnim.DoReset();
        }
        else
        {
            await UniTask.Delay(100);
            pageContent.DOPause();
            inputAnim.DoReset();

            float animAmount = GetNestedAnimAmount(_comment, pageContent.rect.height);
            pageContent.DOAnchorPosY(animAmount, 0.5f).onComplete = () => inputAnim.DoReset();
        }
    }

}
