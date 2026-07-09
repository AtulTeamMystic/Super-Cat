using System;
using UnityEngine;
/// <summary>
/// Server Endpoint and APIs are listed here and picked by using fucntion
/// </summary>
public class EndPoint : MonoBehaviour
{
    internal static EndPoint instance;
    [SerializeField]static string baseUrl = "https://api.gameshphere.flexsin.org";
    static string LoginUrl = "/user/userLogin";
    static string RegisterUrl = "/user/userRegister";
    static string ProfileUrl = "/user/userDetail";
    static string GspUsdUrl = "/user/getGspUsdPrice";
    static string BuyGameAssetsUrl = "/user/buyAsset";
    static string GetGameAssetsByUser = "/user/assetList?gameId=supercat_gamesphere";
    static string UpdateUserAsset = "/user/updateAsset";
    static string UpdateUserCoins = "/user/updateAssetItem";
    static string GetUserCoins = "/user/assetItem?gameId=supercat_gamesphere";


    public void OnValidate()
    {
        instance = this;
    }

    internal static string GetLoginAPI()
    {
        return baseUrl + LoginUrl;
    }

    internal static string GetRegisterAPI()
    {
        return baseUrl + RegisterUrl;
    } 
    internal static string GetProfileUrl()
    {
        return baseUrl + ProfileUrl;
    }
    internal static string GetGspUsdUrl()
    {
        return baseUrl + GspUsdUrl;
    }
    internal static string BuyGameAssets()
    {
        return baseUrl + BuyGameAssetsUrl;
    }

    internal static string GetGameAssets()
    {
        return baseUrl + GetGameAssetsByUser;
    }
    
    internal static string UpdateUserAssets()
    {
        return baseUrl + UpdateUserAsset;
    } 
    
    internal static string UpdateUserCoinsUrl()
    {
        return baseUrl + UpdateUserCoins;
    }
    
    internal static string GetUserCoinsUrl()
    {
        return baseUrl + GetUserCoins;
    }
}