using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

/// <summary>
/// TO Store some keys and having data statically with static fucntions
/// </summary>
public class StaticData : MonoBehaviour
{
    public static StaticData instance;
    public static string GameID = "supercat_gamesphere";
    public static int ChainID = 80001;

    #region Asset ID's

    public const int MagnetID = 4512;
    public const int ExtralifeId = 4513;
    public const int InvincibltyId = 4514;
    public const int ScoreMultipierId = 4515;

    #endregion

  

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    
    #region Extension helper

    public static void DestroyHelper(GameObject go)
    {
        Destroy(go);
    }

    public static void ClearContainerHelper(Transform trans)
    {
        foreach (Transform s in trans)
        {
            s.gameObject.CustomDestroy();
        }
    }

    #endregion

    #region Corutine Wait

    private static readonly Dictionary<float, WaitForSeconds> WaitDict = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDict.TryGetValue(time, out var wait)) return wait;
        WaitDict[time] = new WaitForSeconds(time);
        return WaitDict[time];
    }

    #endregion

    #region UTF Serilization

    public static string UTFSerializeObject<T>(T classData)
    {
        // byte[] jsonBytes = Utf8Json.JsonSerializer.Serialize(classData);
        // return System.Text.Encoding.UTF8.GetString(jsonBytes);
        Newtonsoft.Json.JsonSerializerSettings Settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Ignore,
            DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
            Converters =
            {
                new Newtonsoft.Json.Converters.IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
        return Newtonsoft.Json.JsonConvert.SerializeObject(classData, Settings);
    }

    public static T UTFDeserializeObject<T>(string mainJson)
    {
        // byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(mainJson);
        //
        // // Deserialize JSON to data
        // return Utf8Json.JsonSerializer.Deserialize<T>(jsonBytes);


        Newtonsoft.Json.JsonSerializerSettings Settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Ignore,
            DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
            Converters =
            {
                new Newtonsoft.Json.Converters.IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(mainJson, Settings);
    }

    #endregion

    #region Email Validation

    public static bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        return Regex.IsMatch(email, pattern);
    }

    #endregion


}