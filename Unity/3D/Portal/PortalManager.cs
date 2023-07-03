/*
 * brunch : phantom
 * update : 2023-05-09
 * email : chho1365@gmail.com
 */

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Cysharp.Threading.Tasks;

namespace Metalive
{
    /*
     * Addressable add download
     */

    [RequireComponent(typeof(PortalController))]
    public class PortalManager : MonoBehaviour, IPopupCallback
    {

        #region Variable

        // Controller
        private PortalController controller;        

        // Task
        private List<AsyncOperationHandle<SceneInstance>> scenes;
        private AsyncOperationHandle<MetaliveLightingData> lighting;


        // ↓ 수정 필요
        private GameObject player;

        #endregion



        #region Lifecycle

        private void OnEnable()
        {
            Metalive.Hub += OnMessage;

            Popup.AddCallback(this);
        }

        private void Start()
        {
            controller = GetComponent<PortalController>();
            if(controller)
            {
                Portal.IsUse = true;
            }
        }

        private void OnDisable()
        {
            Metalive.Hub -= OnMessage;

            Popup.RemoveCallback(this);
        }

        /*
         * [ Enter ]
         * 1. Direct
         * 2. World To World
         */
        private void OnMessage(string label, string key, string value)
        {
            if (label.Equals("Portal"))
            {
                switch (key)
                {
                    case "Enter":
                        PortalEnter(value);
                        break;
                    case "Exit":
                        PortalExit(value);
                        break;
                    case "Error":
                        PortalError(value);
                        break;
                    case "Debug":
                        PortalDebug(value);
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            Portal.IsUse = false;

            if(scenes.Count > 0)
            {
                for(int i = 0; i < scenes.Count; i++)
                {
                    if (scenes[i].IsValid())
                    {
                        Addressables.Release(scenes[i]);
                    }
                }
            }

            if(lighting.IsValid())
            {
                Addressables.Release(lighting);
            }

            if (player)
            {
                DestroyImmediate(player);
            }
        }

        #endregion



        #region Method

        // ==================================================
        // Enter or Exit
        // ==================================================        
        private void PortalEnter(string code)
        {
            Portal.status = PortalStatusType.Enter;
            Enter();
        }
        
        private void PortalExit(string code)
        {
            Portal.status = PortalStatusType.Exit;
            Exit();
        }

        // ==================================================
        // Progress
        // ==================================================
        private void Enter()
        {
            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            controller.PortalOpen();

            // Chapter1
            controller.ProgressOpen();
            controller.ProgressInit();            

            var catalog = await CatalogInit();
            if (!catalog)
            {                
                return;
            }

            controller.ProgressBreak();
            controller.ProgressInit();

            // Chapter2            
            controller.ProgressEnviorment();

            var IsWorld = await WorldSceneInit();
            if (!IsWorld)
            {             
                return;
            }

            var IsPlayer = await PlayerSceneInit();
            if (IsPlayer)
            {
                PlayerInit().Forget();
            }
            else
            {
                return;
            }

            var IsNetwork = await NetworkInit();
            if (!IsNetwork)
            {
                return;
            }

            var IsUI = await UISceneInit();
            if (!IsUI)
            {                
                return;
            }            

            controller.ProgressBreak();
            controller.ProgressInit();

            await UniTask.Delay(2000);

            Portal.status = PortalStatusType.Enter;
            controller.ProgressClose();
            controller.PortalClose();            
        }
        
        private async void Exit()
        {                        
            try
            {                
                controller.PortalOpen();

                NetworkFina();
                WorldSceneFina();                
                UISceneFina();                
                PlayerSceneFina();                
                PlayerFina();                
                
                controller.PortalClose();
            }
            catch
            {
                Metalive.Message("Portal", "Error", "9999");
            }
            finally
            {                
                var location = "00_Hub";
                var scene = SceneManager.LoadSceneAsync(location);
                await scene;

                if(scene.isDone)
                {
                    Portal.status = PortalStatusType.Exit;
                    await Resources.UnloadUnusedAssets();
                }
            }
        }


        // ==================================================
        // Catelog
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

                if(data.Status == AsyncOperationStatus.Succeeded)
                {
                    var totalSize = (long)0;
                    var count = data.Result.scenes.Count;
                    for(int i = 0; i < count; i++)
                    {
                        var location = data.Result.scenes[i];
                        var size = Addressables.GetDownloadSizeAsync(location);
                        await size;
                        totalSize += size.Result;

                        if(size.IsValid())
                        {
                            Addressables.Release(size);
                        }                        
                    }

                    if(totalSize > 0)
                    {
                        for(int i = 0; i < count; i++)
                        {
                            var location = data.Result.scenes[i];
                            var download = Addressables.DownloadDependenciesAsync(location);
                            while(!download.IsDone)
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
                                if(download.IsValid())
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

        // ==================================================
        // Network
        // ==================================================
        private async UniTask<bool> NetworkInit()
        {
            try
            {
                if ((SettingNetworkType)Setting.Network.type == SettingNetworkType.MultiPlay)
                {
                    NetworkManager.Instance.ConnectToServer();                    

                    var cts = new CancellationTokenSource();
                    cts.CancelAfterSlim(TimeSpan.FromSeconds(30));
                    await UniTask.WaitUntil(() => NetworkManager.Instance.IsNetworkConnect, PlayerLoopTiming.Update, cts.Token);

                    if(NetworkManager.Instance.IsNetworkConnect)
                    {
                        NetworkChat.Instance.ConnectToServer();
                    }
                }

                return true;
            }
            catch
            {
                CatalogError();
                return false;
            }
        }

        private bool NetworkFina()
        {
            try
            {
                if ((SettingNetworkType)Setting.Network.type == SettingNetworkType.MultiPlay)
                {
                    NetworkManager.Instance.DisconnectToServer();
                }

                return true;
            }
            catch
            {
                CatalogCatch();
                return false;
            }
        }

        // ==================================================
        // WorldScene
        // ==================================================
        private async UniTask<bool> WorldSceneInit()
        {
            try
            {
                var address = $"{Setting.World.code}_0_0";
                var data = Addressables.LoadAssetAsync<MetaliveExportData>(address);
                await data;

                if (data.Status == AsyncOperationStatus.Succeeded)
                {
                    if (scenes == null)
                    {
                        scenes = new List<AsyncOperationHandle<SceneInstance>>();
                    }

                    var count = data.Result.scenes.Count;                    
                    for(int i = 0; i < count; i++)
                    {                        
                        var scene = Addressables.LoadSceneAsync(data.Result.scenes[i], LoadSceneMode.Additive);
                        await scene;
                        scenes.Add(scene);
                    }                    

                    if(data.IsValid())
                    {
                        Addressables.Release(data);
                    }
                    
                    if(Setting.World.code == 4055)
                    {
                        Quest.Send("32", Setting.World.code.ToString());
                    }                    

                    return true;
                }
                else
                {
                    CatalogError();
                    return false;
                }
            }
            catch
            {
                CatalogError();
                return false;
            }
            finally
            {
                WorldSetting().Forget();
            }
        }

        private async UniTaskVoid WorldSetting()
        {
            try
            {
                var address = $"{Setting.World.code}_0_1";
                var data = Addressables.LoadAssetAsync<MetaliveLightingData>(address);
                await data;

                if(data.Status == AsyncOperationStatus.Succeeded)
                {
                    if(data.Result.IsSkybox)
                    {
                        RenderSettings.skybox = data.Result.skybox;
                    }
                    else
                    {
                        RenderSettings.skybox = null;
                    }

                    if(data.Result.IsFog)
                    {
                        RenderSettings.fog = true;
                        RenderSettings.fogColor = data.Result.fogColor;
                        RenderSettings.fogMode = data.Result.fogMode;
                        RenderSettings.fogDensity = data.Result.fogDensity;
                    }
                    else
                    {
                        RenderSettings.fog = false;
                    }
                }
            }
            catch
            {                
                return;
            }
        }

        private bool WorldSceneFina()
        {
            try
            {
                if (scenes.Count > 0)
                {
                    for (int i = 0; i < scenes.Count; i++)
                    {
                        if (scenes[i].IsValid())
                        {
                            Addressables.Release(scenes[i]);
                        }
                    }
                }

                if(lighting.IsValid())
                {
                    Addressables.Release(lighting);
                }

                return true;
            }
            catch
            {
                CatalogCatch();
                return true;
            }
        }


        // ==================================================
        // UI
        // ==================================================
        private async UniTask<bool> UISceneInit()
        {
            try
            {
                var location = "02_UI";
                var scene = SceneManager.LoadSceneAsync(location, LoadSceneMode.Additive);
                await scene;

                if (scene.isDone)
                {
                    return true;
                }
                else
                {
                    CatalogError();
                    return false;
                }
            }
            catch
            {
                CatalogError();
                return false;
            }
        }

        private bool UISceneFina()
        {
            try
            {
                var location = "02_UI";
                if (SceneManager.GetSceneByName(location) != null)
                {
                    SceneManager.UnloadSceneAsync(location);
                }

                return true;
            }
            catch
            {
                CatalogCatch();
                return false;
            }
        }


        // ==================================================
        // Player
        // ==================================================
        private async UniTask<bool> PlayerSceneInit()
        {
            try
            {
                var location = "03_Player";
                var scene = SceneManager.LoadSceneAsync(location, LoadSceneMode.Additive);
                await scene;

                if (scene.isDone)
                {
                    return true;
                }
                else
                {
                    CatalogError();
                    return false;
                }
            }
            catch
            {
                CatalogError();
                return false;
            }
        }

        private bool PlayerSceneFina()
        {
            try
            {
                var location = "03_Player";
                if (SceneManager.GetSceneByName(location) != null)
                {
                    SceneManager.UnloadSceneAsync(location);
                }

                return true;
            }
            catch
            {
                CatalogCatch();
                return false;
            }
        }

        private async UniTaskVoid PlayerInit()
        {
            try
            {
                var address = $"{Setting.World.code}_0_3";
                var data = Addressables.LoadAssetAsync<MetalivePlayerData>(address);
                await data;

                if (data.Status == AsyncOperationStatus.Succeeded)
                {
                    CreatePlayer(data.Result.playerPosition, Quaternion.Euler(data.Result.playerRotation)).Forget();
                }
                else
                {
                    CreatePlayer(Vector3.zero, Quaternion.identity).Forget();
                }

                if (data.IsValid())
                {
                    Addressables.Release(data);
                }                
            }
            catch
            {
                CreatePlayer(Vector3.zero, Quaternion.identity).Forget();                
            }

        }

        private async UniTaskVoid CreatePlayer(Vector3 position, Quaternion rotation)
        {
            if ((SettingNetworkType)Setting.Network.type == SettingNetworkType.MultiPlay)
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfterSlim(TimeSpan.FromSeconds(60));
                await UniTask.WaitUntil(() => NetworkManager.Instance.IsNetworkConnect == true, PlayerLoopTiming.Update, cts.Token);
                player = NetworkManager.Instance.PlayerInstantiate(position, rotation);
                if (player)
                {
                    SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName("03_Player"));
                }
                else
                {
                    CatalogError();
                }
            }
            else
            {
                var obj = Resources.Load<GameObject>("Player");
                player = Instantiate(obj, position, rotation);
                if (player)
                {
                    SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName("03_Player"));
                }
                else
                {
                    CatalogError();
                }
            }
        }

        private bool PlayerFina()
        {
            try
            {
                if (player)
                {
                    DestroyImmediate(player);
                }

                return true;
            }
            catch
            {
                CatalogCatch();
                return false;
            }
        }


        // ==================================================
        // Portal Error
        // ==================================================
        private void PortalError(string code)
        {
            Portal.status = PortalStatusType.Error;            
        }


        // ==================================================
        // Portal Cancel
        // ==================================================
        public void PortalCancel()
        {
            Portal.status = PortalStatusType.Cancel;
        }


        // ==================================================
        // Portal Debug 
        // ==================================================
        private void PortalDebug(string debug)
        {

#if UNITY_EDITOR
            Debug.Log(debug);
#endif

        }

        public void OnPopupUpdate(PopupRedirect redirect)
        {
            if(redirect.code == "Error-out")
            {
                Debug.Log("Exit");
                PortalExit("");
            }
        }

        #endregion

    }

}

