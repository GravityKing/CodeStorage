/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 공유하기
 */ 
using UnityEngine;
using Metalive;
public class FeedShare : MonoBehaviour
{
    public void Share()
    {
        var feedInfo = transform.root.GetComponentInChildren<FeedDetailInfo>();
        new NativeShare()
            .SetSubject("").SetText("").SetUrl($"https://{Metalive.Setting.Server.api}/auth/metalive-link?type=feed&no={feedInfo.feedNo}")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

    }

    public void Share(Feed _feed)
    {
        new NativeShare()
            .SetSubject("").SetText("").SetUrl($"https://{Metalive.Setting.Server.api}/auth/metalive-link?type=feed&no={_feed.contentNo}")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }
}
