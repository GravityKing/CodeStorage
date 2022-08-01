/*
작성자: 최재호(cjh0798@gmail.com)
기능: 자기 피드 댓글 풀업메뉴 컨트롤러
 */
using UnityEngine;
using UnityEngine.UI;

public class CommentPullupController : PullupController
{
    public Button editBtn;
    public Button deleteBtn;

    // Edit 페이지 업데이트
    public void EditPageInit(ContentInfo _content, CommentEditType _type)
    {
        editBtn.onClick.RemoveAllListeners();
        editBtn.onClick.AddListener(() => OffPullupMenu());
        editBtn.onClick.AddListener(() =>
        {
            MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
            canvasNav.Push("CommentEdit_Page");
            CommentEditor commentEditor = canvasNav.subDic["CommentEdit_Page"].GetComponentInChildren<CommentEditor>();
            commentEditor.Init(_content, _type);
        });
    }

    // 삭제 함수 AddListner
    public void AddDeleteListener(ContentInfo _content)
    {
        deleteBtn.onClick.RemoveAllListeners();
        deleteBtn.onClick.AddListener(() => OffPullupMenu());
        deleteBtn.onClick.AddListener(() =>
        {
            MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
            CommentEditor commentEditor = canvasNav.subDic["CommentEdit_Page"].GetComponentInChildren<CommentEditor>();
            commentEditor.DeleteCommnet(_content);
        });
    }
}
