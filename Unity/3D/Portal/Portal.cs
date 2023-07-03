/*
 * brunch : phantom
 * update : 2023-03-23
 * email : chho1365@gmail.com
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metalive
{
    public class Portal
    {

        /// <summary>
        /// 
        /// </summary>
        public static bool IsUse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static PortalStatusType status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string> catalogs { get; set; }



        // ==================================================
        // [ Portal ]
        // ==================================================

        /// <summary>
        /// 
        /// </summary>
        public static string activateScene { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable() => IsAvailable(activateScene);

        public static bool IsAvailable(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid();
        }


        /// <summary>
        /// 
        /// </summary>        
        public static void Open(string sceneName) => OpenAsync(sceneName).Forget();

        private static async UniTask OpenAsync(string sceneName)
        {
            if (IsAvailable(sceneName))
            {
                return;
            }
            else
            {
                if(IsAvailable())
                {
                    await CloseAsync(activateScene);
                }
            }

            var scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await scene;

            if (scene.isDone)
            {
                activateScene = sceneName;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        public static void Close(string sceneName) => CloseAsync(sceneName).Forget();

        public static async UniTask CloseAsync(string sceneName)
        {
            var scene = SceneManager.UnloadSceneAsync(sceneName);
            await scene;

            if (!scene.isDone)
            {
                activateScene = sceneName;
            }
        }

    }
}