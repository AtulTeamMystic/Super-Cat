using System;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

/// <summary>
/// The Game manager is a state machine, that will switch between state according to current gamestate.
/// </summary>
public class GameManager : MonoBehaviour
{
    static public GameManager instance { get { return s_Instance; } }
    static protected GameManager s_Instance;
    [SerializeField] internal Button LogOutButton;

    [Header("Transaction Area")]
    [SerializeField] internal GameObject transactionPrefab;
    [SerializeField] internal Transform transactionPrefabParent;
    [SerializeField] internal GameObject TransactionPanel;
    [SerializeField] internal Button closeTransactionPanel;
    [SerializeField] internal Button openTransactionPanel;
    [SerializeField] internal TMP_Text NoTransactionFoundText;

    public AState[] states;
    public AState topState { get { if (m_StateStack.Count == 0) return null; return m_StateStack[m_StateStack.Count - 1]; } }

    public ConsumableDatabase m_ConsumableDatabase;

    protected List<AState> m_StateStack = new List<AState>();
    protected Dictionary<string, AState> m_StateDict = new Dictionary<string, AState>();

    protected void OnEnable()
    {
        PlayerData.Create();

        s_Instance = this;

        m_ConsumableDatabase.Load();

        // We build a dictionnary from state for easy switching using their name.
        m_StateDict.Clear();

        if (states.Length == 0)
            return;

        for (int i = 0; i < states.Length; ++i)
        {
            states[i].manager = this;
            m_StateDict.Add(states[i].GetName(), states[i]);
        }

        m_StateStack.Clear();

        PushState(states[0].GetName());
    }

    protected void Update()
    {
        if (m_StateStack.Count > 0)
        {
            m_StateStack[m_StateStack.Count - 1].Tick();
        }
    }

    protected void OnApplicationQuit()
    {
#if UNITY_ANALYTICS
        // We are exiting during game, so this make this invalid, send an event to log it
        // NOTE : this is only called on standalone build, as on mobile this function isn't called
        bool inGameExit = m_StateStack[m_StateStack.Count - 1].GetType() == typeof(GameState);

        Analytics.CustomEvent("user_end_session", new Dictionary<string, object>
        {
            { "force_exit", inGameExit },
            { "timer", Time.realtimeSinceStartup }
        });
#endif
    }

    // State management
    public void SwitchState(string newState)
    {
        if (newState == "Loadout")
        {
            Camera.main.transform.position = new Vector3(0, 4.3f, -3);
        }

        AState state = FindState(newState);
        if (state == null)
        {
            Debug.LogError("Can't find the state named " + newState);
            return;
        }

        m_StateStack[m_StateStack.Count - 1].Exit(state);
        state.Enter(m_StateStack[m_StateStack.Count - 1]);
        m_StateStack.RemoveAt(m_StateStack.Count - 1);
        m_StateStack.Add(state);
    }

    public AState FindState(string stateName)
    {
        AState state;
        if (!m_StateDict.TryGetValue(stateName, out state))
        {
            return null;
        }

        return state;
    }

    public void PopState()
    {
        if (m_StateStack.Count < 2)
        {
            Debug.LogError("Can't pop states, only one in stack.");
            return;
        }

        m_StateStack[m_StateStack.Count - 1].Exit(m_StateStack[m_StateStack.Count - 2]);
        m_StateStack[m_StateStack.Count - 2].Enter(m_StateStack[m_StateStack.Count - 2]);
        m_StateStack.RemoveAt(m_StateStack.Count - 1);
    }

    public void PushState(string name)
    {
        AState state;
        if (!m_StateDict.TryGetValue(name, out state))
        {
            Debug.LogError("Can't find the state named " + name);
            return;
        }

        if (m_StateStack.Count > 0)
        {
            m_StateStack[m_StateStack.Count - 1].Exit(state);
            state.Enter(m_StateStack[m_StateStack.Count - 1]);
        }
        else
        {
            state.Enter(null);
        }
        m_StateStack.Add(state);
    }



    #region ProfileManagement

    [System.Serializable]
    public class ProfileUI
    {
        public TMP_Text coinCount;
        public TMP_Text gspCount;
        public TMP_Text playerName;
    }

    [Header("Ui Data")][SerializeField] internal ProfileUI _profileUi;


    private void Start()
    {
        GetPlayerEssentials();
        openTransactionPanel.onClick.AddListener(OpenTransacton);
        closeTransactionPanel.onClick.AddListener(CloseTransacton);

        foreach (var s in ProfileAndShopManager.instance._userCoins.data)
        {
            PlayerData.instance.coins = s.totalCount;
            PlayerData.instance.Save();
        }

        LogOutButton.onClick.AddListener(delegate
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("start");
        });
    }

    internal void GetPlayerEssentials()
    {
        double gspBalance = ProfileAndShopManager.instance._profileData.data.gspBalance;
        gspBalance = Math.Floor(gspBalance * 100) / 100;
        string formattedGspBalance = gspBalance.ToString("R");

        _profileUi.coinCount.text = PlayerData.instance.coins.ToString();
        _profileUi.playerName.text = ProfileAndShopManager.instance._profileData.data.userName;
        _profileUi.gspCount.text = formattedGspBalance;
        AddTransactions();
    }


    internal void AddTransactions()
    {

        if (ProfileAndShopManager.instance.userAssetData.data.Count == 0)
        {
            NoTransactionFoundText.gameObject.SetActive(true);
        }

        if (ProfileAndShopManager.instance.userAssetData.data.Count != 0)
        {
            NoTransactionFoundText.gameObject.SetActive(false);
            foreach (Transform item in transactionPrefabParent)
            {
                Destroy(item.gameObject);
            }
        }
        if (ProfileAndShopManager.instance.userAssetData.data.Count != 0)
        {
            var sortedTransactions = ProfileAndShopManager.instance.userAssetData.data
                .SelectMany(s => s.assetdetails.Select(t => new { AssetName = s.assetName, Price = s.price, TransactionTime = DateTime.Parse(t.transactionTime, null, DateTimeStyles.RoundtripKind).ToLocalTime() }))
                .OrderByDescending(t => t.TransactionTime);

            foreach (var t in sortedTransactions)
            {
                GameObject transactionHistoryData = Instantiate(transactionPrefab, transactionPrefabParent);
                transactionHistoryData.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = t.AssetName;
                transactionHistoryData.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = t.Price.ToString();
                transactionHistoryData.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = t.TransactionTime.ToString();
            }
        }

    }

    internal void OpenTransacton()
    {
        LoadoutState ld = FindObjectOfType<LoadoutState>();
        ld.charPosition.gameObject.SetActive(false);
        TransactionPanel.SetActive(true);
        LeanTween.scale(TransactionPanel, new Vector3(1, 1, 1), 0.3f);
    }

    internal void CloseTransacton()
    {

        LeanTween.scale(TransactionPanel, new Vector3(0, 0, 0), 0.3f);
        TransactionPanel.SetActive(false);
        LoadoutState ld = FindObjectOfType<LoadoutState>();
        ld.charPosition.gameObject.SetActive(true);
    }

    #endregion
}

public abstract class AState : MonoBehaviour
{
    [HideInInspector]
    public GameManager manager;

    public abstract void Enter(AState from);
    public abstract void Exit(AState to);
    public abstract void Tick();

    public abstract string GetName();
}