using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class CustomExtension
{
    //UI
    public static void AddCustomListener(this Button button, System.Action action)
    {
        if (button == null)
        {
            Debug.LogError("button reference missing");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { action?.Invoke(); });
    }

    public static void CustomOnValueChanged(this InputField inputField, System.Action action)
    {
        if (inputField == null)
        {
            Debug.LogError("inputField reference missing");
            return;
        }

        inputField.onValueChanged.RemoveAllListeners();
        inputField.onValueChanged.AddListener(delegate { action?.Invoke(); });
    }

    public static bool CheckIsNull(this InputField inputField)
    {
        return (string.IsNullOrWhiteSpace(inputField.text) || string.IsNullOrEmpty(inputField.text));
    }

    public static void EmptyField(this InputField inputField, System.Action action)
    {
        inputField.onValueChanged.RemoveAllListeners();
        inputField.text = string.Empty;
        inputField.CustomOnValueChanged(action);
    }

    public static void CustomDestroy(this GameObject go)
    {
        if (go != null)
        {
            StaticData.DestroyHelper(go);
        }
    }

    public static void ClearContainer(this Transform trans)
    {
        StaticData.ClearContainerHelper(trans);
    }

    public static void CallGetAPI(this MonoBehaviour behav, string url, System.Action<UnityWebRequest, string> output,
        System.Action<float> progress = null, string RawJson = null,
        Dictionary<string, string> customHeader = null, string headertoken = null, string payload = null)
    {
        GenericApi.CallGetAPI(behav, url, output, progress, RawJson, customHeader, headertoken, payload);
    }

    public static void CallPostAPI(this MonoBehaviour behav, string url, string RawJson,
        System.Action<UnityWebRequest, string> output, Dictionary<string, string> customHeader = null,
        string headertoken = null)
    {
        GenericApi.CallPostAPI(behav, url, RawJson, output, customHeader, headertoken);
    }

    public static void CallPutAPI(this MonoBehaviour behav, string url, string RawJson,
        System.Action<UnityWebRequest, string> output, Dictionary<string, string> customHeader = null,
        string headertoken = null)
    {
        GenericApi.CallPutAPI(behav, url, RawJson, output, customHeader, headertoken);
    }

    public static void CallPostAPI(this MonoBehaviour behav, string url, WWWForm RawJson,
        System.Action<UnityWebRequest, WWWForm> output, Dictionary<string, string> customHeader = null,
        string headertoken = null)
    {
        GenericApi.CallPostAPI(behav, url, RawJson, output, customHeader, headertoken);
    }
    public static void CallGetTextureAPI(this MonoBehaviour behav, string url, System.Action<UnityWebRequest> output,
        System.Action<float> progress = null, string RawJson = null,
        string headertoken = null, string payload = null)
    {
        //Debug.Log("Using GetAPi>>" + url);
        GenericApi.CallGetTextureAPI(behav, url, output, progress, RawJson, headertoken, payload);
    }

    public static void CallGetAudioAPI(this MonoBehaviour behav, string url, System.Action<UnityWebRequest> output,
        System.Action<float> progress = null, string RawJson = null,
        string headertoken = null, string payload = null)
    {
        // Debug.Log("Using GetAPi>>" + url);
        GenericApi.CallGetAudioAPI(behav, url, output, progress, headertoken, payload);
    }

    #region Loger

    internal static void Log(this MonoBehaviour mono, object msg)
    {
        Debug.Log(msg);
    }

    internal static void LogWarning(this MonoBehaviour mono, object msg)
    {
        Debug.LogWarning(msg);
    }

    internal static void LogError(this MonoBehaviour mono, object msg)
    {
        Debug.LogError(msg);
    }

    #endregion

    #region Serialization

    public static string CallUTFSerialization<T>(this T classData)
    {
        return StaticData.UTFSerializeObject(classData);
    }

    public static T CallUTFDeserialization<T>(this string jsonData)
    {
        return StaticData.UTFDeserializeObject<T>(jsonData);
    }


    #endregion

    #region Generic task


    public static void DelayDestroy(this MonoBehaviour mono, GameObject Go, float delay,
        System.Action onComplete = null)
    {
        GenericTasks.StaticDelayDestroy(mono, Go, delay, onComplete);
    }

    public static void GetGenericDelay(this MonoBehaviour mono, float delay, System.Action onComplete = null)
    {
        GenericTasks.StaticGetDelay(mono, delay, onComplete);
    }

    #endregion

}