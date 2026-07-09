using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class GenericApi : MonoBehaviour
{
    public static GenericApi instance;

    void Awake()
    {
        instance = this;
    }


    public static void CallGetAPI(MonoBehaviour mono, string url, System.Action<UnityWebRequest, string> Output,
        System.Action<float> progress = null, string RawJson = null,
        Dictionary<string, string> customHeader = null, string headertoken = null, string payload = null)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();

        mono.StartCoroutine(api.CoreGetAPI(url, Output, progress, RawJson, customHeader, headertoken, payload));
    }

    IEnumerator CoreGetAPI(string url, System.Action<UnityWebRequest, string> Output, System.Action<float> progress,
        string RawJson = null, Dictionary<string, string> customHeader = null, string headertoken = null,
        string payload = null)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        if (headertoken != null)
        {
            www.SetRequestHeader("Authorization", "Bearer " + headertoken);
        }

        if (customHeader != null)
        {
            foreach (var s in customHeader)
            {
                www.SetRequestHeader(s.Key, s.Value);
            }
        }

        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate, sdch");
        www.SetRequestHeader("Cache-Control", "no-cache");
        www.SetRequestHeader("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
        if (RawJson != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(RawJson);

            UploadHandler uH = new UploadHandlerRaw(bytes);

            uH.contentType = "application/json";
            www.uploadHandler = uH;
        }

        var operation = www.SendWebRequest();


        while (!operation.isDone)
        {
            progress?.Invoke(www.downloadProgress);
            yield return null;
        }

        Output?.Invoke(www, payload);
        Destroy(this);
    }

    public static void CallPostAPI(MonoBehaviour mono, string url, string RawJson,
        System.Action<UnityWebRequest, string> output,Dictionary<string, string> customHeader = null,
        string headerToken = null)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();

        mono.StartCoroutine(api.CoreCallPostAPI(url, RawJson, output,customHeader,headerToken));
    }
    
    public static void CallPutAPI(MonoBehaviour mono, string url, string RawJson,
        System.Action<UnityWebRequest, string> output,Dictionary<string, string> customHeader = null,
        string headerToken = null)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();

        mono.StartCoroutine(api.CoreCallPutAPI(url, RawJson, output,customHeader,headerToken));
    }

    IEnumerator CoreCallPostAPI(string url, string RawJson, System.Action<UnityWebRequest, string> output,Dictionary<string, string> customHeader = null,
        string headerToken = null)
    {
        using UnityWebRequest www = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST);
        
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate, sdch");
        www.SetRequestHeader("Cache-Control", "no-cache");
        www.SetRequestHeader("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(RawJson);
// Do not bypass SSL validation
        www.certificateHandler = new BypassCertificate();
        UploadHandler uH = new UploadHandlerRaw(bytes);

        uH.contentType = "application/json";
        www.uploadHandler = uH;
        if (headerToken != null)
        {
            www.SetRequestHeader("Authorization", "Bearer " + headerToken);
        }
        if (customHeader != null)
        {
            foreach (var s in customHeader)
            {
                www.SetRequestHeader(s.Key, s.Value);
            }
        }

        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        output?.Invoke(www, RawJson);
        //output?.Invoke(www, www.downloadHandler.text);

        Destroy(this);
    } 
    
    IEnumerator CoreCallPutAPI(string url, string RawJson, System.Action<UnityWebRequest, string> output,Dictionary<string, string> customHeader = null,
        string headerToken = null)
    {
        using UnityWebRequest www = UnityWebRequest.Put(url, UnityWebRequest.kHttpVerbPUT);
        
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate, sdch");
        www.SetRequestHeader("Cache-Control", "no-cache");
        www.SetRequestHeader("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(RawJson);
// Do not bypass SSL validation
        www.certificateHandler = new BypassCertificate();
        UploadHandler uH = new UploadHandlerRaw(bytes);

        uH.contentType = "application/json";
        www.uploadHandler = uH;
        if (headerToken != null)
        {
            www.SetRequestHeader("Authorization", "Bearer " + headerToken);
        }
        if (customHeader != null)
        {
            foreach (var s in customHeader)
            {
                www.SetRequestHeader(s.Key, s.Value);
            }
        }

        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        output?.Invoke(www, RawJson);
        //output?.Invoke(www, www.downloadHandler.text);

        Destroy(this);
    }

    public static void CallPostAPI(MonoBehaviour mono, string url, WWWForm FormData,
        System.Action<UnityWebRequest, WWWForm> output,Dictionary<string, string> customHeader = null,
        string headerToken = null)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();

        mono.StartCoroutine(api.CoreCallPostAPI(url, FormData, output,customHeader, headerToken));
    }

    IEnumerator CoreCallPostAPI(string url, WWWForm FormData, System.Action<UnityWebRequest, WWWForm> output,Dictionary<string, string> customHeader = null,
        string headerToken = null)
    {
        using UnityWebRequest www = UnityWebRequest.Post(url, FormData);
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate, sdch");
        www.SetRequestHeader("Cache-Control", "no-cache");
        www.SetRequestHeader("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");

        www.certificateHandler = new BypassCertificate();
        if (headerToken != null)
        {
            www.SetRequestHeader("Authorization", "Bearer " + headerToken);
        }
        if (customHeader != null)
        {
            foreach (var s in customHeader)
            {
                www.SetRequestHeader(s.Key, s.Value);
            }
        }

        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        output?.Invoke(www, FormData);

        Destroy(this);
    }

    public static void CallAssetBundle(MonoBehaviour mono, string url, System.Action<UnityWebRequest> outPut)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();

        mono.StartCoroutine(api.GetAssetBundle(url, outPut));
    }

    IEnumerator GetAssetBundle(string url, System.Action<UnityWebRequest> outPut)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate, sdch");
        www.SetRequestHeader("Cache-Control", "no-cache");
        www.SetRequestHeader("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
        yield return www.SendWebRequest();

        outPut?.Invoke(www);
        Destroy(this);
        //if (www.result != UnityWebRequest.Result.Success) {
        //    Debug.Log(www.error);
        //}
        //else {
        //    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
        //}
    }


    #region Texture API

    public static void CallGetTextureAPI(MonoBehaviour mono, string url, System.Action<UnityWebRequest> Output,
        System.Action<float> progress = null, string RawJson = null,
        string headertoken = null, string payload = null)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();
        Debug.Log("Using GetAPi>>" + url);

        mono.StartCoroutine(api.CoreGetTextureAPI(url, Output, progress, RawJson, headertoken, payload));
        //StaticData.instance.AddToQueue(CoreStaticGetApi(mono, url, Output, progress, RawJson, headertoken, payload));
    }

    IEnumerator CoreGetTextureAPI(string url, System.Action<UnityWebRequest> Output,
        System.Action<float> progress,
        string RawJson = null, string headertoken = null, string payload = null)
    {
        this.Log("Using GetAPi>>" + url);

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            if (headertoken != null)
            {
                www.SetRequestHeader("Authorization", "Bearer " + headertoken);
            }

            yield return www.Send();
            Output?.Invoke(www);

            www.Dispose();
        }


        Destroy(this);
    }

    #endregion

    #region Audio

    public static void CallGetAudioAPI(MonoBehaviour mono, string url, System.Action<UnityWebRequest> Output,
        System.Action<float> progress = null,
        string headertoken = null, string payload = null)
    {
        GenericApi api = mono.gameObject.AddComponent<GenericApi>();
        Debug.Log("Using GetAPi>>" + url);

        mono.StartCoroutine(api.DownloadAudio(url, Output, progress, headertoken, payload));
        //StaticData.instance.AddToQueue(CoreStaticGetApi(mono, url, Output, progress, RawJson, headertoken, payload));
    }

    IEnumerator DownloadAudio(string url, System.Action<UnityWebRequest> Output, System.Action<float> progress = null,
        string headertoken = null, string payload = null)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            Output?.Invoke(www);
            www.Dispose();
        }


        Destroy(this);
    }

    #endregion
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always return true to ignore certificate errors
        return true;
    }
}