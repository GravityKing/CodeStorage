/*
작성자: 최재호(cjh0798@gmail.com)
기능: 댓글 Input 관련 기능
 */
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentInputController : CommentInputAnimation, IBeginDragHandler, IDragHandler, IPointerClickHandler, IEndDragHandler
{
    private bool isDragging;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging == false)
        {
            commentInput.enabled = true;
            commentInput.ActivateInputField();
        }
        else
        {
            isDragging = false;
        }
    }

    public async void OnBeginDrag(PointerEventData eventData)
    {
        commentInput.enabled = false;
        scrollRect.OnBeginDrag(eventData);

        await UniTask.WaitUntil(() => !Input.GetMouseButton(0), PlayerLoopTiming.Update);
        await UniTask.Delay(100);

        if (isDragging == true)
            isDragging = false;

    }

    public void OnEndEdit()
    {
        commentInput.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        scrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }
}
