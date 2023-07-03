/*
작성자: 최재호(cjh0798@gmail.com)
기능: ScrollSnap과 함께 X축 Y축 멀티 스크롤 가능한 기능
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class SnapableMultiscroll : MultiScrollBase
{
    public HorizontalScrollSnap horizontalScrollSnap;

    // 첫 드래그 시
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        horizontalScrollSnap.OnBeginDrag(eventData);
    }

    // 드래그 시 Scroll
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        horizontalScrollSnap.OnDrag(eventData);
    }

    // 드래그 끝날 때 X,Y Scroll Rect True
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        horizontalScrollSnap.OnEndDrag(eventData);
    }
}
