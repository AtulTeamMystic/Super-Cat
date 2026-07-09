using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager instance;
    [SerializeField] internal GameObject conversionPopUp;
    [SerializeField] internal GameObject coinBuyconfirmationPopUp;
    [SerializeField] internal TMP_Text ConversionText;
    [SerializeField] internal Button gspBuyConfirm;
    [SerializeField] internal Button coinBuyConfirm;
    [SerializeField] internal TMP_Text ConfirmBtnText;
    [SerializeField] internal Button CloseButton;
    [SerializeField] internal Button CloseButtonCoinBuyPanel;
    [SerializeField] internal Text gspCount;
    [SerializeField] internal TMP_Text assetName;
    [SerializeField] internal TMP_Text warningText;
    [SerializeField] internal TMP_Text balancWearning;
    

    

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        FetchGspBalance();
        CloseButton.onClick.AddListener(CloseConfirmationPanel);
        CloseButtonCoinBuyPanel.onClick.AddListener(CloseCoinBuyConfirmationPanel);
    }

    internal void FetchGspBalance()
    {
        if (gspCount != null)
        {
            gspCount.text = "0";
        }
    }

    #endregion

    #region Internal Methods

    internal void OpenConfirmationPanel(Action onConfirm)
    {
    }
    
    internal void OpenCoinBuyConfirmationPanel(Action onConfirm)
    {
        coinBuyconfirmationPopUp.SetActive(true);
        LeanTween.scale(coinBuyconfirmationPopUp, new Vector3(1, 1, 1), 0.3f);
        coinBuyConfirm.onClick.AddListener(delegate { onConfirm?.Invoke(); });

    }

    internal void CloseConfirmationPanel()
    {
    }
    internal void CloseCoinBuyConfirmationPanel()
    {
        LeanTween.scale(coinBuyconfirmationPopUp, new Vector3(0, 0, 0), 0.3f);
        coinBuyconfirmationPopUp.SetActive(false);
        coinBuyConfirm.onClick.RemoveAllListeners();
    }


    internal IEnumerator ShowWarnings(string msg)
    {
        warningText.text = msg;
        yield return new WaitForSeconds(1);
        warningText.text = "Buying Asset From GSP Token";
    }
    
    internal IEnumerator ShowBalanceWarnings()
    {
        balancWearning.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        balancWearning.gameObject.SetActive(false);
    }

    #endregion
    
    
}
