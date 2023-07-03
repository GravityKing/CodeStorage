using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAnimationClipLength : MonoBehaviour
{
    public Animator animator;

    public async void Dance()
    {
        animator.Play("Ani_Emotion_Hi_01");

        await UniTask.Yield();

        AnimatorClipInfo[] clipsInfo = animator.GetCurrentAnimatorClipInfo(0);
        AnimatorClipInfo clipInfo = Array.Find(clipsInfo, x => x.clip.name == "Ani_Emotion_Hi_01");
        float clipLength = clipInfo.clip.length;

        await UniTask.Delay((int)clipLength * 1000);
    }
}
