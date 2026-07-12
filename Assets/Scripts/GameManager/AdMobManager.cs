using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AdMobManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AdMobManager");
                    _instance = go.AddComponent<AdMobManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    private static AdMobManager _instance;

    [Header("Settings")]
    [Tooltip("If true, Google Mobile Ads test ad unit IDs will be used instead of production IDs.")]
    public bool useTestAds = true;

    [Header("Production (Real) Ad Unit IDs")]
    public string realAndroidAdUnitId = "YOUR_REAL_ANDROID_AD_UNIT_ID";
    public string realIosAdUnitId = "YOUR_REAL_IOS_AD_UNIT_ID";

    [Header("Test Ad Unit IDs")]
    [SerializeField]
    private string testAndroidAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField]
    private string testIosAdUnitId = "ca-app-pub-3940256099942544/1712485313";

    private RewardedAd _rewardedAd;
    private bool _isInitializing = false;
    private bool _isInitialized = false;

    public bool IsShowingAd { get; private set; } = false;

    private Action _onRewardEarnedCallback;
    private Action _onAdClosedCallback;
    private bool _rewardEarned;

    public string GetRewardedAdUnitId()
    {
        if (useTestAds)
        {
#if UNITY_ANDROID
            return testAndroidAdUnitId;
#elif UNITY_IPHONE
            return testIosAdUnitId;
#else
            return "unused";
#endif
        }
        else
        {
#if UNITY_ANDROID
            return realAndroidAdUnitId;
#elif UNITY_IPHONE
            return realIosAdUnitId;
#else
            return "unused";
#endif
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAdMob();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAdMob()
    {
        if (_isInitializing || _isInitialized) return;
        _isInitializing = true;

        Debug.Log("AdMobManager: Initializing Google Mobile Ads SDK...");

        // Set to true to marshal events to main thread
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            _isInitialized = true;
            _isInitializing = false;
            Debug.Log("AdMobManager: Google Mobile Ads SDK Initialized.");

            // Load the first ad
            LoadRewardedAd();
        });
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("AdMobManager: Loading rewarded ad...");
        var adRequest = new AdRequest();

        string adUnitId = GetRewardedAdUnitId();
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("AdMobManager: Rewarded ad failed to load. Error: " + error);
                return;
            }

            Debug.Log("AdMobManager: Rewarded ad loaded successfully. AdUnitId: " + adUnitId);
            _rewardedAd = ad;
            RegisterEventHandlers(ad);
        });
    }

    public bool IsRewardedAdReady()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }

    public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed)
    {
        if (IsRewardedAdReady())
        {
            IsShowingAd = true;
            _rewardEarned = false;
            _onRewardEarnedCallback = onRewardEarned;
            _onAdClosedCallback = onAdClosed;

            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("AdMobManager: User earned reward: " + reward.Amount + " " + reward.Type);
                _rewardEarned = true;
                _onRewardEarnedCallback?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("AdMobManager: ShowRewardedAd called but ad is not ready. Invoking close callback.");
            onAdClosed?.Invoke();
            LoadRewardedAd();
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("AdMobManager: Rewarded ad closed.");
            StartCoroutine(DelayClosedCallback());
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("AdMobManager: Rewarded ad failed to show: " + error);
            StartCoroutine(DelayFailedCallback());
        };
    }

    private System.Collections.IEnumerator DelayClosedCallback()
    {
        yield return null; // Wait 1 frame for any pending reward callbacks to execute
        IsShowingAd = false;
        if (!_rewardEarned)
        {
            _onAdClosedCallback?.Invoke();
        }
        LoadRewardedAd();
    }

    private System.Collections.IEnumerator DelayFailedCallback()
    {
        yield return null; // Wait 1 frame for any pending reward callbacks to execute
        IsShowingAd = false;
        if (!_rewardEarned)
        {
            _onAdClosedCallback?.Invoke();
        }
        LoadRewardedAd();
    }
}
