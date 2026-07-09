using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileAndShopManager : MonoBehaviour
{
    internal static ProfileAndShopManager instance;
    [SerializeField] internal Button startButton;
    public static Action AfterAssetPurchaseSuccess;

    #region Data Classes
    #region Profile Data Class

    [System.Serializable]
    public class Data
    {
        public string _id;
        public string email;
        public string userName;
        public long mobile;
        public string address;
        public DateTime createdAt;
        public DateTime updatedAt;
        public string ethBalance;
        public double gspBalance;
        public string tokenPrice;
    }

    [System.Serializable]
    public class ProfileData
    {
        public int code;
        public string message;
        public Data data = new Data();
        public bool error;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Header("------------------Profile Data------------------")] 
    [SerializeField] internal ProfileData _profileData = new ProfileData();
    #endregion

    #region User Assets Data
    [Serializable]
    public class Assetdetail
    {
        public string transactionHash;
        public string fromAddress;
        public string toAddress;
        public int blockNumber;
        public int gasUsed;
        public int chainId;
        public bool Status;
        public string _id;
        public string transactionTime;
    }
    [Serializable]
    public class Datum
    {
        public string _id;
        public string gameId;
        public string assetName;
        public string assetId;
        public int price;
        public int totalAssetsCount;
        public int remainingAssetsCount;
        public List<Assetdetail> assetdetails;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [Serializable]
    public class UserAssetData
    {
        public int code;
        public string message;
        public List<Datum> data = new List<Datum>();
        public bool error;
    }

    [SerializeField] internal UserAssetData userAssetData = new UserAssetData();
    #endregion

    #region Coin Data

    public class CoinData
    {
        public string gameId;
        public string assetName;
        public int totalCount;
        public bool Status;
    }
    internal CoinData coinData = new CoinData();
    #endregion

    #region GetCoinsData

    public class UserCoinsData
    {
        public string _id;
        public string gameId;
        public string assetName;
        public int totalCount;
        public DateTime createdAt;
        public DateTime updatedAt;
    }

    public class UserCoins
    {
        public int code;
        public string message;
        public List<UserCoinsData> data = new List<UserCoinsData>();
        public bool error;
    }

    internal  UserCoins _userCoins = new UserCoins();
    #endregion

    #region Buy Asset Data
    [Serializable]
    public class BuyAssetData
    {
        public string gameId;
        public string assetName;
        public string assetId;
        public string price;
        public int chainId;
    }
    [SerializeField] internal BuyAssetData buyAssetData = new BuyAssetData();

    #endregion

    #region BuyAssetResponse

    public class BuyAssetdetail
    {
        public string transactionHash;
        public string fromAddress;
        public string toAddress;
        public int blockNumber;
        public int gasUsed;
        public int chainId;
        public bool Status;
        public string _id;
    }

    public class BuyAssetDataRespose
    {
        public string _id;
        public string userId;
        public string gameId;
        public string assetName;
        public string assetId;
        public int price;
        public int totalAssetsCount;
        public int remainingAssetsCount;
        public List<BuyAssetdetail> assetdetails;
        public DateTime createdAt;
        public DateTime updatedAt;
        public int __v;
    }

    public class BuyassetResponse
    {
        public int code;
        public string message;
        public BuyAssetDataRespose data = new BuyAssetDataRespose();
        public bool error;
    }

    [SerializeField] internal BuyassetResponse buyassetResponse = new BuyassetResponse();
    #endregion

    #region Gsp Usd Conversion Data
    [Serializable]
    public class GspUsdConveriondata
    {
        public string _id;
        public double price;
        public DateTime createdAt;
        public DateTime updatedAt;
        public int __v;
    }
    [Serializable]
    public class GspUsdConversion
    {
        public int code;
        public string message;
        public GspUsdConveriondata data = new GspUsdConveriondata();
        public bool error;
    }

    [SerializeField] internal GspUsdConversion gspUsdConversion = new GspUsdConversion();
    #endregion

    #endregion


    #region Api Calls

    #region Profile Api Call

    internal void FetchProfileData()
    {
        if (startButton != null)
        {
            startButton.GetComponentInChildren<Text>().text = "Loading...";
        }
        string url = EndPoint.GetProfileUrl();
        string loginToken = LoginManager.instance.loginResponseData.data.jwtToken;
        Dictionary<string, string> customHeader = new Dictionary<string, string>();
        customHeader.Add("token", loginToken);

        this.CallGetAPI(url, delegate(UnityWebRequest www, string payload)
        {
            if (www.error != null)
            {
                this.Log($"Error Found in profile api {www.error}");
            }
            else
            {
                GetGameAssetsByUser();
                string resp = www.downloadHandler.text;
                this.Log($"Profile APi {resp}");
                _profileData = resp.CallUTFDeserialization<ProfileData>();
                GetUserCoins();
                PlayerData.instance.previousName = _profileData.data.userName;
                if (SceneManager.GetSceneByName("Shop").isLoaded)
                {
                    DebugLog.Log("Scene 2");
                    ShopUIManager.instance.FetchGspBalance();
                }
                
            }
        }, customHeader: customHeader);
    }
    #endregion
    
    #region In Game Coin Update Api Call
    internal void UpdateCoinsFromGame(int Coins)
    {
        string url = EndPoint.UpdateUserCoinsUrl();
        CoinData Coindata = new CoinData()
        {
            gameId = StaticData.GameID,
            assetName = "PlayerCoins",
            totalCount = Coins,
            Status = true
        };

        string json = Coindata.CallUTFSerialization();

        string loginToken = LoginManager.instance.loginResponseData.data.jwtToken;
        Dictionary<string, string> customHeader = new Dictionary<string, string>();
        customHeader.Add("token", loginToken);

        this.CallPutAPI(url, json, delegate (UnityWebRequest www, string payload)
        {
            if (www.error != null)
            {
                this.Log($"Error Found in update Coin api {www.error}");
            }
            else
            {
                string resp = www.downloadHandler.text;
                this.Log($"Update Coins {resp}");

            }
        }, customHeader: customHeader);

    }

    #endregion

    #region Getting Game Coins Api Call
    internal void GetUserCoins()
    {
        string url = EndPoint.GetUserCoinsUrl();
        string loginToken = LoginManager.instance.loginResponseData.data.jwtToken;
        Dictionary<string, string> customHeader = new Dictionary<string, string>();
        customHeader.Add("token", loginToken);
        this.CallGetAPI(url, delegate (UnityWebRequest www, string payload)
        {
            if (www.error != null)
            {
                this.Log($"Error Found in User coin api {www.error}");
            }
            else
            {
                string resp = www.downloadHandler.text;
                this.Log($"User Coin Api {resp}");
                _userCoins = resp.CallUTFDeserialization<UserCoins>();
                if (startButton != null)
                {
                    startButton.GetComponentInChildren<Text>().text = "Start";
                    startButton.interactable = true;
                }
            }
        }, customHeader: customHeader);
    }

    #endregion
    
    #region Getting Game Assets From user Api Call

    internal void GetGameAssetsByUser()
    {
        string url = EndPoint.GetGameAssets();
        string loginToken = LoginManager.instance.loginResponseData.data.jwtToken;
        Dictionary<string, string> customHeader = new Dictionary<string, string>();
        customHeader.Add("token", loginToken);
        this.CallGetAPI(url, delegate (UnityWebRequest www, string payload)
        {
            if (www.error != null)
            {
                this.Log($"Error Found in UserAssetData api {www.error}");
            }
            else
            {
                string resp = www.downloadHandler.text;
                this.Log($"UserAsset Api {resp}");
                userAssetData = resp.CallUTFDeserialization<UserAssetData>();
                AfterAssetPurchaseSuccess?.Invoke();
                
            }
        }, customHeader: customHeader);
    }
    #endregion

    #region Buy Assets From Game Api Call      
    internal void BuyAssetsFromGame(string aseetName, string assetId, Action onComplete)
    {
        string url = EndPoint.BuyGameAssets();
        buyAssetData.gameId = StaticData.GameID;
        buyAssetData.assetName = aseetName;
        buyAssetData.assetId = assetId;
        buyAssetData.price = "1";
        buyAssetData.chainId = StaticData.ChainID;

        string json = buyAssetData.CallUTFSerialization();

        string loginToken = LoginManager.instance.loginResponseData.data.jwtToken;
        Dictionary<string, string> customHeader = new Dictionary<string, string>();
        customHeader.Add("token", loginToken);

        this.CallPostAPI(url, json, delegate (UnityWebRequest www, string payload)
        {
            if (www.error != null)
            {
                this.Log($"Error Found in Buy Asset api {www.error}");
                StartCoroutine(ShopUIManager.instance.ShowWarnings(www.error));
            }
            else
            {
                string resp = www.downloadHandler.text;
                this.Log($"BuyAssetApi {resp}");
                buyassetResponse = resp.CallUTFDeserialization<BuyassetResponse>();
                StartCoroutine(ShopUIManager.instance.ShowWarnings(buyassetResponse.message));
                onComplete?.Invoke();

            }
        }, customHeader: customHeader);

    }

    #endregion

    #region Get gsp price in usd     

    internal void GetGspPriceInUsd()
    {
        string url = EndPoint.GetGspUsdUrl();
        string loginToken = LoginManager.instance.loginResponseData.data.jwtToken;
        Dictionary<string, string> customHeader = new Dictionary<string, string>();
        customHeader.Add("token", loginToken);
        this.CallGetAPI(url, delegate (UnityWebRequest www, string payload)
        {
            if (www.error != null)
            {
                this.Log($"Error Found in GspUsd api {www.error}");
            }
            else
            {
                string resp = www.downloadHandler.text;
                this.Log($"GSP Api {resp}");
                gspUsdConversion = resp.CallUTFDeserialization<GspUsdConversion>();
                ShopUIManager.instance.ConversionText.text = gspUsdConversion.data.price.ToString() + "$";
            }
        }, customHeader: customHeader);

    }
    #endregion

    #endregion
}