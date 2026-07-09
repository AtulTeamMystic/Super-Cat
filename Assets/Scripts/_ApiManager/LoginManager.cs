using System;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public static LoginManager instance;

    #region LoginData

    public class LoginData
    {
        public string userName;
        public string email;
        public string mobile;
        public string password;
    }

    #endregion

    #region LoginResponseResult

    public class Data
    {
        public string _id;
        public string email;
        public long mobile;
        public DateTime createdAt;
        public DateTime updatedAt;
        public string jwtToken;
    }

    public class ResponseData
    {
        public int code;
        public string message;
        public Data data;
        public bool error;
    }

    [SerializeField] internal ResponseData loginResponseData = new ResponseData();

    #endregion

    #region RegisterData

    public class RegisterData
    {
        public string userName;
        public string email;
        public string mobile;
        public string password;
    }

    #endregion

    #region RegisterResponseData

    public class RegisterResponseData
    {
        public string userName;
        public string email;
        public string password;
        public long mobile;
        public string address;
        public string privateKey;
        public string _id;
        public DateTime createdAt;
        public DateTime updatedAt;
        public int __v;
    }

    public class RegResponse
    {
        public int code;
        public string message;
        public RegisterResponseData data;
        public bool error;
    }

    [SerializeField] internal RegResponse regResponse = new RegResponse();

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
    }


    internal void CallLoginApi(string email, string password, Action<bool> isSuccess)
    {
        string url = EndPoint.GetLoginAPI();
        LoginData loginData = new LoginData();
        loginData.email = email;
        loginData.password = password;

        string jsonData = loginData.CallUTFSerialization();
        this.CallPostAPI(url, jsonData, OnComplete);

        void OnComplete(UnityWebRequest request, string output)
        {
            if (request.error != null)
            {
                Debug.Log(request.error);
                isSuccess?.Invoke(false);
            }
            else
            {
                string response = request.downloadHandler.text;
                loginResponseData = response.CallUTFDeserialization<ResponseData>();
                Debug.Log(response);
                if (loginResponseData.error)
                {
                    isSuccess?.Invoke(false);
                }
                else
                {
                    isSuccess?.Invoke(true);
                }
            }
        }
    }

    internal void CallRegisterApi(string email, string password, string username, string mobileNo,
        Action<bool> isSuccess)
    {
        string url = EndPoint.GetRegisterAPI();
        RegisterData registerData = new RegisterData();
        registerData.userName = username;
        registerData.email = email;
        registerData.mobile = mobileNo;
        registerData.password = password;

        string jsonData = registerData.CallUTFSerialization();
        this.CallPostAPI(url, jsonData, OnComplete);

        void OnComplete(UnityWebRequest request, string output)
        {
            if (request.error != null)
            {
                Debug.Log(request.error);
                isSuccess?.Invoke(false);
            }
            else
            {
                string response = request.downloadHandler.text;
                regResponse = response.CallUTFDeserialization<RegResponse>();
                Debug.Log(response);
                if (regResponse.error)
                {
                    isSuccess?.Invoke(false);
                }
                else
                {
                    isSuccess?.Invoke(true);
                }
            }
        }
    }
}