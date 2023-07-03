/*
 * brunch : changho
 * day : 2023-01-31
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PortalController : MonoBehaviour
{

    #region Variable

    public Canvas portal;
    public Canvas portalProgress;
    public TMP_Text progressStatus;
    public TMP_Text progressPercentage;
    public Image progressGauge;

    #endregion



    #region Check

    public bool IsProgress { get; set; }

    #endregion



    #region Lifecycle

    private void OnDestroy()
    {
        if(progressStatus)
        {
            progressStatus = null;
        }

        if(progressPercentage)
        {
            progressPercentage = null;
        }

        if(progressGauge)
        {
            progressGauge = null;
        }
    }

    #endregion



    #region Method

    // ==================================================
    // [ Portal ]
    // ==================================================
    public void PortalOpen()
    {
        if(!portal.enabled)
        {
            portal.enabled = true;
        }
    }

    public void PortalClose()
    {
        if (portal.enabled)
        {
            portal.enabled = false;
        }
    }


    // ==================================================
    // [ Progress ]
    // ==================================================
    public void ProgressOpen()
    {
        if(!portalProgress.enabled)
        {
            portalProgress.enabled = true;
        }
    }

    public void ProgressClose()
    {
        if (portalProgress.enabled)
        {
            portalProgress.enabled = false;
        }
    }


    public void ProgressInit()
    {
        IsProgress = false;
        progressStatus.text = "";
        progressPercentage.text = "";        
        progressGauge.fillAmount = 0;
    }

    public void ProgressBreak()
    {
        IsProgress = false;
    }
    
    public void ProgressDownload(float percente)
    {
        float timer = 1 / percente;
        float amount = Mathf.Lerp(progressGauge.fillAmount, percente, timer);
        progressStatus.text = $"리소스 다운로드 중...({(progressGauge.fillAmount * 100).ToString("F1")}%)";
        progressPercentage.text = $"{(progressGauge.fillAmount * 100).ToString("F1")}%";
        progressGauge.fillAmount = amount;
    }

    public async void ProgressEnviorment()
    {
        IsProgress = true;
        while (IsProgress)
        {
            float amount = Mathf.Lerp(progressGauge.fillAmount, 1, Time.deltaTime);
            progressStatus.text = "가상월드 입장 중...";
            progressPercentage.text = $"{(progressGauge.fillAmount * 100).ToString("F1")}%";
            progressGauge.fillAmount = amount;
            await UniTask.Yield();
        }

        progressStatus.text = "";
        progressPercentage.text = "100%";
        progressGauge.fillAmount = 1f;
    }

    public void ProgressPercentage(float percentage)
    {
        progressGauge.fillAmount = percentage;
    }

    
    

    #endregion

}
