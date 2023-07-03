using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Metalive;

public enum MultipartUploadResult
{
    Success,
    Slang,
    Fail
}

[System.Serializable]
public class ImageInfo
{
    public string x = "";
    public string y = "";
    public string width = "";
    public string height = "";
    public string imgExtension = "";
    public string imgSize = "";
    public string imgWidth = "";
    public string imgHeight = "";
    public string imgMaker = "";
    public string imgModel = "";
    public string imgTakenDate = "";
    public string imgLatitude = "";
    public string imgLongitude = "";
    public int index;
}

public enum ImagePOSTType
{
    Feed,
    Trip,
    Profile,
}

public class CropRequestHelper
{
    private SlangDataRoot slangData;
    public List<string> Slangs => slangData.data;
    private byte[] FileToByteArray(string fileName)
    {
        byte[] buffer = null;

        try
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            BinaryReader binaryReader = new BinaryReader(fileStream);
            long totalBytes = new FileInfo(fileName).Length;

            buffer = binaryReader.ReadBytes((Int32)totalBytes);

            fileStream.Close();
            fileStream.Dispose();
            binaryReader.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return buffer;
    }

    public async UniTask<MultipartUploadResult> SendHttpRequest(ImagePOSTType type, string url, string body, string path, Dictionary<string, ImageInfo> imagesInfo)
    {
        string bodyKey = null;
        string lastPath = null;
        switch (type)
        {
            case ImagePOSTType.Trip:
                bodyKey = "tripInfo";
                lastPath = "PlaceRegist";
                break;

            case ImagePOSTType.Feed:
                bodyKey = "feedInfo";
                lastPath = "Feed";
                break;

            case ImagePOSTType.Profile:
                lastPath = "Profile";
                break;
        }

        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Setting.User.token}");

        MultipartFormDataContent form = new MultipartFormDataContent();

        Dictionary<int, string> sortedFiles = GetSortedFiles(path, lastPath);
        int fileIndex = 0;
        foreach (var info in imagesInfo)
        {
            form.Add(new ByteArrayContent(FileToByteArray(@$"{sortedFiles[fileIndex]}")), "image", $"{info.Key}");
            //Debug.Log("files[fileIndex]: " + sortedFiles[fileIndex]);
            fileIndex++;
        }

        Dictionary<string, ImageInfo> subDic = new Dictionary<string, ImageInfo>();

        foreach (var info in imagesInfo)
        {
            subDic.Add(info.Key, info.Value);
            //Debug.Log(info.Key);
            //Debug.Log("x: " + info.Value.x);
            //Debug.Log("y: " + info.Value.y);
            //Debug.Log("width: " + info.Value.width);
            //Debug.Log("height: " + info.Value.height);
        }

        StringContent subInfoSc = new StringContent(JsonConvert.SerializeObject(subDic), Encoding.UTF8, "application/json");
        form.Add(subInfoSc, "imageInfo");

        if (type != ImagePOSTType.Profile)
        {
            StringContent jsonStringSc = new StringContent(body, Encoding.UTF8, "application/json");
            form.Add(jsonStringSc, bodyKey);
        }

        ConfiguredTaskAwaitable<HttpResponseMessage> response;
        string result = null;

        Task feedPost = new Task(() => response = httpClient.PostAsync(url, form).ConfigureAwait(false));
        feedPost.Start();

        // Wait Feed Post
        await UniTask.WaitUntil(() => feedPost.IsCompleted == true);
        Task getResult = new Task(() =>
        {
            result = response.GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;
            Debug.Log(response.GetAwaiter().GetResult());
            Debug.Log(result);
        });

        getResult.Start();

        // Wait Response Data
        await UniTask.WaitUntil(() => getResult.IsCompleted == true);

        // Dispose
        feedPost.Dispose();
        getResult.Dispose();

        // Return
        if (response.GetAwaiter().GetResult().StatusCode == System.Net.HttpStatusCode.OK && !result.Contains("비속어"))
        {
            return MultipartUploadResult.Success;
        }
        else if (result.Contains("비속어"))
        {
            Debug.Log("resultLog.Contains(\"비속어\")");
            slangData = JsonConvert.DeserializeObject<SlangDataRoot>(result);
            return MultipartUploadResult.Slang;
        }
        else
        {
            return MultipartUploadResult.Fail;
        }

    }

    private Dictionary<int, string> GetSortedFiles(string path, string lastPath)
    {
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        Dictionary<int, string> sortedFiles = new Dictionary<int, string>();
        for (int i = 0; i < files.Length; i++)
        {
            string[] letters = files[i].Split('/');

            string lastLetter = letters[letters.Length - 1];
            string fileName = null;
            //Debug.Log("lastLetter: " + lastLetter);
            if (lastLetter.Contains("\\"))
            {
                fileName = lastLetter.Split(new string[] { lastPath + "\\" }, StringSplitOptions.None)[1];
            }
            else
            {
                fileName = lastLetter.Split(new string[] { ".jpg" }, StringSplitOptions.None)[0];
            }
            fileName = fileName.Replace(".jpg", "");
            //Debug.Log("fileName: " + fileName);

            if (lastLetter.Contains("_"))
            {
                string[] secondLetters = fileName.Split('_');
                int key = int.Parse(secondLetters[0]);
                sortedFiles.Add(key, files[i]);
                //Debug.Log("secondLetters[0]: " + secondLetters[0]);
            }
            else
            {
                int key = int.Parse(fileName);
                //Debug.Log("key: " + key);
                sortedFiles.Add(key, files[i]);
            }
        }

        return sortedFiles;
    }
}

