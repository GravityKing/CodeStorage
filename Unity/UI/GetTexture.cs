/*
작성자: 최재호(cjh0798@gamil.com)
기능: 갤러리 오픈 및 사진 선택
 */
using CompactExifLib;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetTexture : MonoBehaviour
{
    public async UniTask<Texture2D> GetTexture(string _path)
    {
        string uri = $@"file://{_path}";
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
        {
            try
            {
                Debug.Log("uri: " + uri);
                return await GetTextureAction(uri);

            }
            // file://으로 불러오지 못했을 때 file:///으로 재시도
            catch (UnityWebRequestException e)
            {
                Debug.Log(e.Result);
                uri = $@"file:///{_path}";
                Debug.Log("uri: " + uri);
                return await GetTextureAction(uri);
            }
        }
    }

    private async UniTask<Texture2D> GetTextureAction(string _uri)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(_uri))
        {

            await request.SendWebRequest();

            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            textureDisposer.AddTexture(pageName, tex);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("텍스쳐 데이터 result: " + request.result);
                return null;
            }
            else
            {
                return tex;
            }
        }
    }

}
