using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("-------------MainUI-------------")]
    [SerializeField] internal GameObject mainUi;
    [SerializeField] internal GameObject homePageCharacter;
    
    [Header("-------------Login Panels-------------")]
    [SerializeField] internal GameObject loginPanel;
    [SerializeField] internal GameObject loginDataPage;

    [Header("-------------Login Data Fields-------------")]
    [SerializeField] internal TMP_InputField email;
    [SerializeField] internal TMP_InputField password;



    [Header("-------------Sign up Panels--------------")]
    [SerializeField] internal GameObject signUpPanel;
    [SerializeField] internal GameObject signUpDataPage;
    
    
    [Header("-------------Sign Up Data Fields-------------")]
    [SerializeField] internal TMP_InputField sUsername;
    [SerializeField] internal TMP_InputField sEmail;
    [SerializeField] internal TMP_InputField sMobileNo;
    [SerializeField] internal TMP_InputField sPassword;
    [SerializeField] internal TMP_InputField sConfirmPassword;


    [Header("-------------Button-------------")] 
    [SerializeField] internal Button loginButton;
    [SerializeField] internal Button createAnAccountBtn;
    [SerializeField] internal Button closeButton;
    [SerializeField] internal Button signUpButton;

    [Header("-------------Login Warning Fields-------------")] 
    [SerializeField]internal TMP_Text emailWarning;
    [SerializeField]internal TMP_Text passWarning;


    [Header("-------------SignUp Warning Fields-------------")] 
    [SerializeField]internal TMP_Text sUsernameWarning;
    [SerializeField]internal TMP_Text sEmailWarning;
    [SerializeField]internal TMP_Text sMobileNoWaring;
    [SerializeField]internal TMP_Text sPassWarning;
    [SerializeField]internal TMP_Text sConfirmPassWarning;
    
    


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        homePageCharacter.SetActive(true);
        if (loginPanel != null) loginPanel.SetActive(false);
        if (signUpPanel != null) signUpPanel.SetActive(false);
    }

    private void Start()
    {
        Application.targetFrameRate = 45; 

        // Find the start button and enable it offline
        StartButton startBtnScript = FindObjectOfType<StartButton>();
        if (startBtnScript != null)
        {
            Button btn = startBtnScript.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = true;
                var txt = btn.GetComponentInChildren<UnityEngine.UI.Text>();
                if (txt != null)
                {
                    txt.text = "Start";
                }
            }
        }
    }


    internal void OpenLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
    }
    
    internal void OpenRegisterPanel()
    {
        if (signUpPanel != null) signUpPanel.SetActive(false);
    }

    internal void CloseLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
    }  
    
    internal void CloseSignUpPanel()
    {
        if (signUpPanel != null) signUpPanel.SetActive(false);
    }

    
    [ContextMenu("Clear")]
    void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
