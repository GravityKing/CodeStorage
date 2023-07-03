using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GetAddressableCatalogWithLoading : MonoBehaviour
{

    public Canvas portal;
    public Canvas portalProgress;
    public TMP_Text progressStatus;
    public TMP_Text progressPercentage;
    public Image progressGauge;
    public bool IsProgress { get; set; }

    // ==================================================
    // Catalog
    // ==================================================
    private async UniTask<bool> CatalogInit()
    {
        try
        {
            if (Portal.catalogs == null)
            {
                Portal.catalogs = new Dictionary<string, string>();
            }

            var identification = $"{Setting.World.identification}";
            var code = $"{Setting.World.code}";
            if (!Portal.catalogs.ContainsKey(code))
            {
                var url = $"https://metalive-asset-resouse.s3.ap-northeast-2.amazonaws.com/admin/asset-resouse/WORLD/{identification}/{code}/v{Setting.World.version}/{Setting.Server.platform}/catalog_metalive.json";
                var catalogHandle = Addressables.LoadContentCatalogAsync(url.Trim(), true);
                await catalogHandle;

                if (catalogHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Addressables.AddResourceLocator(catalogHandle.Result);
                    Portal.catalogs.Add(code, "1");
                }
                else
                {
                    CatalogError();
                    return false;
                }
            }
            else
            {
                Portal.catalogs[code] = (int.Parse(Portal.catalogs[code]) + 1).ToString();
            }

            var address = $"{Setting.World.code}_0_0";
            var data = Addressables.LoadAssetAsync<MetaliveExportData>(address);
            await data;

            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                var totalSize = (long)0;
                var count = data.Result.scenes.Count;
                for (int i = 0; i < count; i++)
                {
                    var location = data.Result.scenes[i];
                    var size = Addressables.GetDownloadSizeAsync(location);
                    await size;
                    totalSize += size.Result;

                    if (size.IsValid())
                    {
                        Addressables.Release(size);
                    }
                }

                if (totalSize > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var location = data.Result.scenes[i];
                        var download = Addressables.DownloadDependenciesAsync(location);
                        while (!download.IsDone)
                        {
                            float percentage = download.GetDownloadStatus().Percent;
                            controller.ProgressDownload(percentage);

                            if (Portal.status == PortalStatusType.Cancel)
                            {
                                CatalogCancel();
                                return false;
                            }

                            await UniTask.Yield();
                        }

                        if (download.Status == AsyncOperationStatus.Succeeded)
                        {
                            if (download.IsValid())
                            {
                                Addressables.Release(download);
                            }
                        }
                        else
                        {
                            // Download fail → check download system
                            CatalogError();
                            return false;
                        }
                    }
                }

                Addressables.Release(data);
            }
            else
            {
                CatalogException();
                return false;
            }

            return true;
        }
        catch
        {
            CatalogException();
            return false;
        }
    }

    private void CatalogError()
    {
        Portal.status = PortalStatusType.Error;

        var hash = new PopupHash
        {
            title = "다운로드 오류",
            message = "서비스 접속이 원활하지 안습니다.\n잠시 후, 다시 시도해 주세요.",
            confirm = "확인",
            yes = "",
            no = "",
        };

        Popup.Message(PopupType.General, PopupCallbackType.Callback, "Error-out", hash);
        return;
    }

    private void CatalogCatch()
    {
        var hash = new PopupHash
        {
            title = "",
            message = "일시적인 오류가 발생하였습니다.",
            confirm = "",
            yes = "",
            no = "",
        };

        Popup.Message(PopupType.Floating, PopupStatusType.Caution, hash);
        return;
    }

    private void CatalogException()
    {
        Portal.status = PortalStatusType.Error;

        var hash = new PopupHash
        {
            title = "다운로드 오류",
            message = "네트워크 연결 상태를 확인하신 후,\n다시 시도해 주세요.",
            confirm = "확인",
            yes = "",
            no = "",
        };

        Popup.Message(PopupType.General, PopupCallbackType.Callback, "Error-out", hash);
        return;
    }

    private void CatalogCancel()
    {
        Portal.status = PortalStatusType.Cancel;

        var hash = new PopupHash
        {
            title = "다운로드 취소",
            message = "다운로드를 취소하였습니다.",
            confirm = "확인",
            yes = "",
            no = "",
        };

        Popup.Message(PopupType.General, PopupCallbackType.Callback, "Error-out", hash);
        return;
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
}
