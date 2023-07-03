/*
작성자: 최재호(cjh0798@gmail.com)
기능: 댓글 생성 기능
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using Metalive;
using DG.Tweening;
using System.Linq;
using System;
using System.Threading;

public class CommentFactory : ContentUtil
{
    #region CommentDataRoot Class
    public class CommentDataRoot
    {
        public List<CommentData> commnetList;
    }
    public class CommentData
    {
        public List<NestedCommentData> nestedCommentList;
        public string nickname;
        public string userID;
        public string delYn;
        public int userCodeNo;
        public string contentText;
        public string date;
        public int commentNo;
        public int postingNo;
        public string profileImgPath;
        public int nestedCount;
    }

    public class NestedCommentData
    {
        public string nickname;
        public string userID;
        public int userCodeNo;
        public string delYn;
        public string contentText;
        public string date;
        public int parentNo;
        public int postingNo;
        public int commentNo;
        public string profileImgPath;
    }
    #endregion

    protected CommentDataRoot commentData;
    protected int commentNo;
    public MainPage mainPage;
    public RectTransform commentContent;
    private RectTransform commentPrefab;
    protected RectTransform nestedCommentPrefab;
    [HideInInspector]
    public List<Comment> commentList;
    protected List<Texture> profileImageTextures = new List<Texture>();
    [SerializeField] private MainTextureDisposer texDisposer;
    private void OnEnable()
    {
        commentPrefab = commentContent.GetChild(0).GetComponent<RectTransform>();
        nestedCommentPrefab = commentContent.GetChild(1).GetComponent<RectTransform>();
        commentData = new CommentDataRoot();
        commentList = new List<Comment>();
    }

    // 초기화
    public void Init()
    {
        for (int i = 0; i < commentList.Count; i++)
        {
            commentList[i].gameObject.SetActive(false);
            Destroy(commentList[i].gameObject);
        }

        commentList.Clear();
        commentNo = 0;
    }

    // 첫 댓글 생성
    public virtual async UniTask CreateComment()
    {
        Init();
        if (commentData.commnetList == null)
            return;

        List<CommentData> commentDataList = commentData.commnetList;
        for (int i = 0; i < commentDataList.Count; i++)
        {
            RectTransform obj = Instantiate(commentPrefab, commentContent.transform);
            Comment comment = obj.GetComponent<Comment>();
            this.commentList.Add(comment);
            obj.gameObject.SetActive(true);
            // 삭제된 댓글
            if (commentDataList[i].delYn == "y" || commentDataList[i].delYn == "Y")
            {
                comment.transform.GetChild(0).gameObject.SetActive(false);
                comment.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = "삭제된 댓글입니다.";
                comment.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                // 댓글 정보 뿌려주기
                await UpdateCommentPanel(commentDataList[i], comment);
                LayoutRebuilder.ForceRebuildLayoutImmediate(commentContent);

            }

            // 대댓글 생성
            await CreateNestedComment(commentDataList[i].nestedCount, i);
            LayoutRebuilder.ForceRebuildLayoutImmediate(commentContent);
        }


        CancellationTokenSource cts = new CancellationTokenSource();
        cts.RegisterRaiseCancelOnDestroy(commentContent);
        await UniTask.WaitUntil(() => commentContent.rect.height != 0, PlayerLoopTiming.Update, cts.Token);
    }



    // 첫 대댓글 추가
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    public virtual async UniTask CreateNestedComment(int _commentCount, int _commentIndex)
    {
        int Listcount = _commentCount;
        int nestedCommentIndex = 0;
        for (int i = Listcount; i < Listcount + _commentCount; i++)
        {
            RectTransform obj = Instantiate(nestedCommentPrefab, commentContent.transform);
            Comment comment = obj.GetComponent<Comment>();
            commentList.Add(comment);
            obj.gameObject.SetActive(true);

            // 대댓글 정보 뿌려주기
            UpdateNestedCommentPanel(_commentIndex, comment, nestedCommentIndex).Forget();
            nestedCommentIndex++;
        }
    }

    // 댓글 추가
    public virtual async void AddComment(TMP_InputField _input)
    {
        
        if (string.IsNullOrEmpty(_input.text) || _input.text == "" || string.IsNullOrWhiteSpace(_input.text))
            return;

        string inputTxt = _input.text;

        // 댓글 생성 및 레이아웃 Update
        RectTransform obj = Instantiate(commentPrefab, commentContent.transform);
        obj.SetAsFirstSibling();
        Comment comment = obj.GetComponent<Comment>();
        obj.gameObject.SetActive(true);
        //commentList.Add(comment);
        commentList.Insert(0, comment);

        // 유저 정보 가져오기
        ProfileDataRoot userData = await GetUserData();

        // 댓글 정보 뿌려주기
        comment.nicknameTxt.text = userData.data.nickname;
        comment.userIDTxt.text = "@" + userData.data.userId;
        comment.date.text = "방금";
        comment.userCodeNo = userData.data.userCodeNo;
        comment.nickname = userData.data.nickname;
        comment.userId = userData.data.userId;
        comment.contentText.text = ConvertToLink(inputTxt);
        comment.content = inputTxt;
        comment.parentNo = 0;
        comment.isNestedComment = false;

        if (string.IsNullOrEmpty(userData.data.profileImgPath.SD))
            Debug.Log("댓글 프로필 이미지 파일 없음");

        else
            comment.profileImage.sprite = await GetProfileImage(userData.data.profileImgPath.SD);

        UpdateLayout();

        // 댓글 추가 시 이펙트
        DoAddedEffect(comment.addedBG);
    }


    // 대댓글 추가
    public virtual async void AddNestedComment(Comment _comment)
    {
        if (string.IsNullOrEmpty(_comment.input.text) || _comment.input.text == "")
            return;

        string inputTxt = _comment.input.text;

        // 댓글 생성
        RectTransform obj = Instantiate(nestedCommentPrefab, commentContent.transform);
        Comment comment = obj.GetComponent<Comment>();
        obj.gameObject.SetActive(true);

        commentNo++;

        // 유저 정보 가져오기
        ProfileDataRoot userData = await GetUserData();

        // 댓글 정보 뿌려주기
        comment.nicknameTxt.text = userData.data.nickname;
        comment.userIDTxt.text = "@" + userData.data.userId;
        comment.date.text = "방금";
        comment.nickname = userData.data.nickname;
        comment.userId = userData.data.userId;
        comment.contentText.text = ConvertToLink(inputTxt);
        comment.content = inputTxt;
        comment.parentNo = _comment.commentNo;
        comment.contentNo = commentNo;
        comment.isNestedComment = true;

        int nextCommentIndex = GetNextCommentIndex(_comment);

        commentList.Insert(nextCommentIndex, comment);
        comment.transform.SetSiblingIndex(nextCommentIndex + 2); // content에 prefab이 0,1번째 Index에 있기 때문에 + 2

        // 댓글 추가 시 이펙트
        DoAddedEffect(comment.addedBG);

        if (string.IsNullOrEmpty(userData.data.profileImgPath.SD))
            return;

        else
            comment.profileImage.sprite = await GetProfileImage(userData.data.profileImgPath.SD);

        UpdateLayout();
    }

    // 댓글 정보 뿌려주기
    private async UniTask UpdateCommentPanel(CommentData _data, Comment _comment)
    {
        Comment comment = _comment;
        comment.nicknameTxt.text = _data.nickname;
        comment.userIDTxt.text = "@" + _data.userID;
        comment.userCodeNo = _data.userCodeNo;
        comment.nickname = _data.nickname;
        comment.userId = _data.userID;
        comment.contentText.text = ConvertToLink(_data.contentText);
        comment.content = _comment.contentText.text;
        comment.commentNo = _data.commentNo;
        comment.contentNo = commentNo;
        comment.postingNo = _data.postingNo;
        comment.isNestedComment = false;

        try
        {
            if (string.IsNullOrEmpty(_data.date))
                comment.date.text = "";
            else
                comment.date.text = ChangeTimeList(_data.date);

            if (string.IsNullOrEmpty(_data.profileImgPath))
                return;
            else
            {
                comment.profileImage.sprite = await GetProfileImage(_data.profileImgPath);
            }
        }
        catch
        {
            if (comment == null)
                return;
        }

    }

    // 대댓글 정보 뿌려주기
    private async UniTask UpdateNestedCommentPanel(int _Commentindex, Comment _comment, int _nestedCommentIndex)
    {
        List<CommentData> _commentList = commentData.commnetList;
        List<NestedCommentData> _nestedCommentList = _commentList[_Commentindex].nestedCommentList;
        _nestedCommentList = _nestedCommentList.OrderBy(x => DateTime.Parse(x.date)).ToList();
        Comment comment = _comment;
        NestedCommentData nestedData = _nestedCommentList[_nestedCommentIndex];

        comment.isNestedComment = true;
        if (nestedData.delYn == "y" || nestedData.delYn == "Y")
        {
            comment.contentText.text = "삭제된 댓글입니다.";
            comment.parentNo = _Commentindex;
            comment.userCodeNo = nestedData.userCodeNo;
            comment.commentNo = nestedData.commentNo;
            comment.contentNo = commentNo;
            comment.nickname = nestedData.nickname;
            comment.content = _comment.contentText.text;
            comment.userId = nestedData.userID;
            comment.transform.GetChild(0).gameObject.SetActive(false);
            comment.transform.GetChild(2).gameObject.SetActive(false);
            return;
        }
        else
        {
            comment.parentNo = _Commentindex;
            comment.userCodeNo = nestedData.userCodeNo;
            comment.commentNo = nestedData.commentNo;
            comment.contentNo = commentNo;
            comment.nickname = nestedData.nickname;
            comment.userId = nestedData.userID;
            comment.content = _comment.contentText.text;
            comment.nicknameTxt.text = nestedData.nickname;
            comment.userIDTxt.text = "@" + nestedData.userID;
            comment.date.text = string.IsNullOrEmpty(nestedData.date) ? "" : ChangeTimeList(nestedData.date);
            comment.contentText.text = ConvertToLink(nestedData.contentText);
        }

        try
        {
            if (string.IsNullOrEmpty(nestedData.profileImgPath))
                return;
            else
                comment.profileImage.sprite = await GetProfileImage(nestedData.profileImgPath);
        }
        catch
        {
            if (comment == null)
                return;
        }


    }

    // 프로필 사진 가져오기
    protected async UniTask<Sprite> GetProfileImage(string _path)
    {
        // path가 null이면 return
        if (string.IsNullOrEmpty(_path))
            return null;

        UnityWebRequest request;

        request = UnityWebRequestTexture.GetTexture(_path);
        await request.SendWebRequest();

        try
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            texture.name = $"CommentFactory: Texture";
            texDisposer.AddTexture(mainPage, texture);
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            profileImageTextures.Add(texture);
            return sprite;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    // 레이아웃 업데이트
    public async void UpdateLayout()
    {
        await UniTask.Yield();
        commentContent.GetComponent<VerticalLayoutGroup>().enabled = false;
        commentContent.GetComponent<VerticalLayoutGroup>().enabled = true;
        commentContent.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
        commentContent.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    // 댓글 추가 시 배경 이펙트
    private async void DoAddedEffect(Image _image)
    {
        _image.enabled = true;
        _image.DOFade(0, 1.5f).onComplete += () => _image.enabled = false;
    }

    // 유저 데이터 가져오기
    protected async UniTask<ProfileDataRoot> GetUserData()
    {
        string uri = $"https://{Metalive.Setting.Server.api}/api/user/info";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            request.SetRequestHeader("Authorization", "Bearer " + Setting.User.token);
            await request.SendWebRequest();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSceneManager>().CheckDuplicateLogin(request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("유저 데이터 result: " + request.result);
                return null;
            }
            else
            {
                ProfileDataRoot userData = JsonConvert.DeserializeObject<ProfileDataRoot>(request.downloadHandler.text);
                return userData;
            }
        }
    }

    // 대댓글 작성 시, 다음 댓글 Index 가져오기
    protected int GetNextCommentIndex(Comment _selectedComment)
    {
        int parentIndex = commentList.IndexOf(_selectedComment);
        int result = parentIndex;
        bool isNextComment = false;

        if (commentList.Count == parentIndex + 1)
            return result + 1;

        for (int i = parentIndex + 1; i < commentList.Count; i++)
        {
            result++;
            if (commentList[i].isNestedComment)
            {

            }
            else
            {
                isNextComment = true;
                break;
            }
        }

        if (isNextComment == false)
            result++;

        return result;
    }

    // 대댓글 작성 시, 애니메이션 이동 Amount 가져오기
    protected float GetNestedAnimAmount(Comment _selectedComment, float _pageContentHeight)
    {
        int targetCount = GetNextCommentIndex(_selectedComment) - 1;
        int allCount = commentList.Count;
        int restCount = allCount - targetCount;

        float canvasHeight = transform.root.GetComponent<RectTransform>().rect.height;
        float result = _pageContentHeight - canvasHeight - (restCount * 358) + 400;

        return result;
    }

    public void DisposeTextures()
    {
        for (int i = 0; i < profileImageTextures.Count; i++)
        {
            DestroyImmediate(profileImageTextures[i]);
        }
    }
}
