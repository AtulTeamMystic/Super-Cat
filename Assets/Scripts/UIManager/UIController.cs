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
        string loginEmail = PlayerPrefs.GetString("Email");
        string loginPassword = PlayerPrefs.GetString("Password");

        if (!string.IsNullOrEmpty(loginEmail) || !string.IsNullOrEmpty(loginPassword))
        {
            email.text = loginEmail;
            password.text = loginPassword;
            Login();
        }
        else
        {
            OpenLoginPanel();
            if (loginPanel.activeInHierarchy || signUpPanel.activeInHierarchy)
            {
                homePageCharacter.SetActive(false);
            }
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 45; 
        createAnAccountBtn.onClick.AddListener(delegate { OpenRegisterPanel(); CloseLoginPanel(); });
        closeButton.onClick.AddListener(delegate { OpenLoginPanel(); CloseSignUpPanel(); });
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(Register);
        
    }


    internal void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        LeanTween.scale(loginDataPage, new Vector3(1, 1, 1), 0.3f);
    }
    
    internal void OpenRegisterPanel()
    {
        signUpPanel.SetActive(true);
        ClearFields();
        LeanTween.scale(signUpDataPage, new Vector3(1, 1, 1), 0.3f);
    }

    internal void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
        LeanTween.scale(loginDataPage, new Vector3(0, 0, 0), 0f);
    }  
    
    internal void CloseSignUpPanel()
    {
        signUpPanel.SetActive(false);
        LeanTween.scale(signUpDataPage, new Vector3(0, 0, 0), 0f);
    }


    private void Login()
    {
      

        string email = this.email.text;
        string password = this.password.text;

        bool isValidEmail = StaticData.IsValidEmail(email);
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
        {
            StartCoroutine(LoginWarning("*Please enter email", true));
            return;
        }
        else
        {
            if (!isValidEmail)
            {
                StartCoroutine(LoginWarning("*Please enter a valid email", true));
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                {
                    StartCoroutine(LoginWarning("*Please enter password"));
                    return;
                }
                if (password.Length<6)
                {
                    StartCoroutine(LoginWarning("*Password must contains 6 characters"));
                    return;
                }
                else
                {
                    loginButton.GetComponentInChildren<TMP_Text>().text = "Loading....";
                    loginButton.interactable = false;
                    LoginManager.instance.CallLoginApi(email, password, (isSuccess) =>
                    {
                        if (isSuccess)
                        {
                            homePageCharacter.SetActive(true);
                            CloseLoginPanel();
                            loginButton.GetComponentInChildren<TMP_Text>().text = "Login";
                            loginButton.interactable = true;
                            PlayerPrefs.SetString("Email",email);
                            PlayerPrefs.SetString("Password",password);
                            ProfileAndShopManager.instance.FetchProfileData();
                        }
                        else
                        {
                            StartCoroutine(EmailPassNotMatches(LoginManager.instance.loginResponseData.message));
                            loginButton.GetComponentInChildren<TMP_Text>().text = "Login";
                            loginButton.interactable = true;
                        }
                       
                        
                    });
                }
            }
        }
        
    } 
    
    private void Register()
    {
        string email = this.sEmail.text;
        string password = this.sPassword.text;
        string username = sUsername.text;
        string mobileNO = sMobileNo.text;
        string confirmPassword = sConfirmPassword.text;
        bool isValidEmail = StaticData.IsValidEmail(email);
        if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
        {
            StartCoroutine(RegisterWarning("*Please enter username", isUsername:true));
            return;
        }
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
        {
            StartCoroutine(RegisterWarning("*Please enter email", isEmail:true));
            return;
        }
        else
        {
            if (!isValidEmail)
            {
                StartCoroutine(RegisterWarning("*Please enter a valid email", isEmail:true));
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(mobileNO) || string.IsNullOrWhiteSpace(mobileNO))
                {
                    StartCoroutine(RegisterWarning("*Please enter mobile no", isMobile:true));
                    return;
                } 
                if (mobileNO.Length < 10)
                {
                    StartCoroutine(RegisterWarning("*Mobile no must consist 10 digits", isMobile:true));
                    return;
                } 
                if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                {
                    StartCoroutine(RegisterWarning("*Please enter password", isPassword:true));
                    return;
                }
                if (password.Length < 6)
                {
                    StartCoroutine(RegisterWarning("*Password must contains 6 character", isPassword:true));
                    return;
                }  
                if (string.IsNullOrEmpty(confirmPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    StartCoroutine(RegisterWarning("*Please enter confirm password", isConfirmPass:true));
                    return;
                }
                if (confirmPassword != password)
                {
                    StartCoroutine(RegisterWarning("*Password and Confirm Password do not match", isConfirmPass:true));
                    return;
                }
                else
                {
                    signUpButton.GetComponentInChildren<TMP_Text>().text = "Loading....";
                    signUpButton.interactable = false;
                    LoginManager.instance.CallRegisterApi(email,password,username,mobileNO, (isSuccess) =>
                    {
                        if (isSuccess)
                        {
                            signUpButton.GetComponentInChildren<TMP_Text>().text = "SIGN UP";
                            OpenLoginPanel();
                            CloseSignUpPanel();
                            StartCoroutine(EmailPassNotMatches("Register Successfully.Please Login to proceed"));
                            signUpButton.interactable = true;
                        }
                        else
                        {
                            StartCoroutine(EmailPassNotMatchesRegister(LoginManager.instance.regResponse.message));
                            signUpButton.GetComponentInChildren<TMP_Text>().text = "SIGN UP";
                            signUpButton.interactable = true;
                        }
                       
                        
                    });
                }
            }
        }
        
    }


    private IEnumerator LoginWarning(string msg,bool isEmail = false)
    {
        if (isEmail)
        {
            emailWarning.text = msg;
            yield return new WaitForSeconds(3);
            emailWarning.text = "";
        }
        else
        {
            passWarning.text = msg;
            yield return new WaitForSeconds(3);
            passWarning.text = "";
        }
    }

    private IEnumerator EmailPassNotMatches(string msg)
    {
        passWarning.text = msg;
        yield return new WaitForSeconds(3);
        passWarning.text = "";
    }
    private IEnumerator EmailPassNotMatchesRegister(string msg)
    {
        sConfirmPassWarning.text = msg;
        yield return new WaitForSeconds(3);
        sConfirmPassWarning.text = "";
    }

    private IEnumerator RegisterWarning(string msg, bool isEmail = false, bool isUsername = false,
        bool isPassword = false, bool isMobile = false, bool isConfirmPass = false)
    {
        if (isEmail)
        {
            sEmailWarning.text = msg;
            yield return new WaitForSeconds(3);
            sEmailWarning.text = "";
        }
        if (isUsername)
        {
            sUsernameWarning.text = msg;
            yield return new WaitForSeconds(3);
            sUsernameWarning.text = "";
        } 
        if (isPassword)
        {
            sPassWarning.text = msg;
            yield return new WaitForSeconds(3);
            sPassWarning.text = "";
        } 
        if (isMobile)
        {
            sMobileNoWaring.text = msg;
            yield return new WaitForSeconds(3);
            sMobileNoWaring.text = "";
        } 
        if (isConfirmPass)
        {
            sConfirmPassWarning.text = msg;
            yield return new WaitForSeconds(3);
            sConfirmPassWarning.text = "";
        }
    }

    
    [ContextMenu("Clear")]
    void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    void ClearFields()
    {
        email.text = "";
        password.text = "";
        sUsername.text = "";
        sEmail.text = "";
        sMobileNo.text = "";
        sPassword.text = "";
        sConfirmPassword.text = "";
    }
}
