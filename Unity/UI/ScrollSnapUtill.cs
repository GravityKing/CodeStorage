using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSnapUtill : MonoBehavior
{
    public HorizontalScrollSnap scrollSnap;

    // 런타임 중 이미지를 생성하고 초기화 할 때 사용
    private void DistributePages()
    {
        scrollSnap.DistributePages();
        scrollSnap.UpdateLayout();
    }
}
