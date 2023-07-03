/*
작성자: 최재호(cjh0798@gmail.com)
기능: X축 Y축 멀티 스크롤 기능 포함
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiScrollBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect xScrollRect;
    public ScrollRect yScrollRect;

    private Vector3 preMousePos;
    private Vector3 mousePos;
    private Vector2 mousePosX;
    private Vector2 mousePosY;
    private float absX;
    private float absY;
    private void Update()
    {
        preMousePos = Input.mousePosition;
    }

    // 첫 드래그 시
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginMultiScroll(eventData);
    }

    // 드래그 시 Scroll
    public virtual void OnDrag(PointerEventData eventData)
    {
        OnMultiScroll(eventData);
    }

    // 드래그 끝날 때 X,Y Scroll Rect True
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        OnEndMultiScroll(eventData);
        yScrollRect.enabled = true;
        xScrollRect.enabled = true;
    }

    // 첫 1회 Scroll
    public void OnBeginMultiScroll(PointerEventData eventData)
    {
        GetMousePos(eventData);

        if (absX > absY)
        {
            yScrollRect.enabled = false;
            xScrollRect.enabled = true;
            xScrollRect.OnBeginDrag(eventData);

        }
        else if (absX < absY)
        {
            xScrollRect.enabled = false;
            yScrollRect.enabled = true;
            yScrollRect.OnBeginDrag(eventData);
        }
    }

    // 드래그 중 Scroll
    public void OnMultiScroll(PointerEventData eventData)
    {

        if (absX > absY)
        {
            xScrollRect.OnDrag(eventData);
        }
        else if (absX < absY)
        {
            yScrollRect.OnDrag(eventData);
        }

    }

    // 드래그가 끝날 시 Scroll(가속)
    public void OnEndMultiScroll(PointerEventData eventData)
    {
        if (absX > absY)
        {
            xScrollRect.OnEndDrag(eventData);
        }
        else if (absX < absY)
        {
            yScrollRect.OnEndDrag(eventData);
        }

    }

    // Mouse Vector 및 x,y의 절대값 계산
    void GetMousePos(PointerEventData eventData)
    {
        mousePos = eventData.position;
        mousePos -= preMousePos;
        mousePosX = new Vector2(mousePos.x, 0);
        mousePosY = new Vector2(0, mousePos.y);

        absX = Mathf.Abs(mousePosX.x);
        absY = Mathf.Abs(mousePosY.y);
    }
}
