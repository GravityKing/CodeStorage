/*
작성자: 최재호(cjh0798@gmail.com)
기능: 메인 씬 UI에 사용되는 Player 회전
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlayerRot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [HideInInspector]
    public GameObject player;
    public ScrollRect scrollRect;
    public float speed;

    private float preX;
    private Vector3 input;
    private float onPointerDownInputY;
    private bool isVerticalDrag;
    public void OnDrag(PointerEventData eventData)
    {
        float onDragInputY = 0;
        float deltaY = 0;

        // X 드래그
        if (player != null && Input.touchCount == 1)
        {
            input = Input.mousePosition;
            float inputX = input.x;
            onDragInputY = Mathf.Abs(input.y);
            if (preX < inputX) // 오른쪽
            {
                player.transform.Rotate(new Vector2(0, -speed));
            }
            else if (preX > inputX) // 왼쪽
            {
                player.transform.Rotate(new Vector2(0, speed));
            }
            preX = inputX;
        }
        else if (player == null)
        {
            Debug.LogError("UIPlayerRot 스크립트의 Player가 Null입니다.");
        }

        // Y 드래그
        if (onPointerDownInputY > onDragInputY)
        {
            deltaY = onPointerDownInputY - onDragInputY;
        }
        else if (onPointerDownInputY < onDragInputY)
        {
            deltaY = onDragInputY - onPointerDownInputY;
        }
        else
        {
            isVerticalDrag = false;
            return;
        }
        isVerticalDrag = deltaY > 7 ? true : false;

        if (isVerticalDrag)
            scrollRect.OnDrag(eventData);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isVerticalDrag)
            scrollRect.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isVerticalDrag)
            scrollRect.OnEndDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownInputY = Mathf.Abs(input.y);
    }
}
