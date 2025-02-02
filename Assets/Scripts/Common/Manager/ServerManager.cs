﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public delegate void ServerDelegate<T>(T id, T data);

enum ServerConnectionKind
{
    ServerConnectionKind_Login,
    ServerConnectionKind_SignUp,
    ServerConnectionKind_End
}

public class ServerManager : SingletonMonoBehaviour<ServerManager>
{
    [Header("Server")]
    public const string url = "http://127.0.0.1:80/";
    public const string loginConnectionName = "login";
    public const string signupConnectionName = "SignUp";
    public const string saveConnectionName = "Save";
    public const string loadConnectionName = "Load";

    //해당 아이디는 테스트 아이디로 서버 통신을 더이상 하지 않는다.(어떤 비밀번호와도 로그인이 되고 서버와 통신은 하지 않아 저장, 로드가 없다.)
    public const string exceptionID = "1234";

    private bool _isUseServer = true;

    public TextMeshProUGUI resultTextUI;

    public static void Function<T>(T id, T data, ServerDelegate<T> dele)
    {
        dele(id, data);
    }
    
    public void Awake()
    {
        if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    public void Login(string id, string passward, TextMeshProUGUI textUI = null)
    {
        if (textUI != null)
            resultTextUI = textUI;
        else
            resultTextUI = null;

        //로그인을 수행합니다.
        StartCoroutine(PostLoginConnection(id, passward));
    }

    public void SignUp(string id, string passward, TextMeshProUGUI textUI = null)
    {
        if (textUI != null)
            resultTextUI = textUI;
        else
            resultTextUI = null;

        //회원가입을 수행합니다.
        StartCoroutine(PostSignUpConnection(id, passward));
    }

    public void Save(string id, string saveData)
    {
        //세이브를 수행합니다.
        StartCoroutine(PostSaveConnection(id, saveData));
    }

    public void Load(string id, ServerDelegate<string> dele)
    {
        StartCoroutine(PostLoadConnection(id, dele));
        StartCoroutine(SaveCycle());
    }

    IEnumerator SaveCycle()
    {
        while (true)
        {
            if ((int)SceneLoader.instance.NowSceneKind() > (int)SceneKind.Start)
            {
                PlayerManager.instance.Save();
            }

            yield return new WaitForSeconds(3.0f);

            if (GameManager.instance == null)
                break;
        }
    }

    IEnumerator PostLoginConnection(string id, string passward)
    {
        if (string.Equals(id, exceptionID))
        {
            //예외 접근 로그인을 수행합니다.(초기화면으로 서버 통신 x)
            //서버를 제외한 기능을 테스트할때 사용합니다.
            _isUseServer = false;

            resultTextUI.text = "로그인에 성공하였습니다.\n게임을 접속합니다.";
            resultTextUI.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);

            UIManager.instance.UISetting(UiKind.UiKind_LoginUI);
            UIManager.instance.UISetting(UiKind.UiKind_CustomUI);
        }

        List<IMultipartFormSection> loginForm = new List<IMultipartFormSection>();

        loginForm.Add(new MultipartFormDataSection("ID", id));
        loginForm.Add(new MultipartFormDataSection("Passward", passward));


        UnityWebRequest WebRequest = UnityWebRequest.Post(string.Format("{00}{01}.php", url, loginConnectionName), loginForm);

        yield return WebRequest.SendWebRequest();

        if (WebRequest != null)
        {
            string result = WebRequest.downloadHandler.text;

            if (string.Equals(result, "Success"))
            {
                if (resultTextUI != null)
                {
                    resultTextUI.text = "로그인에 성공하였습니다.\n게임을 접속합니다.";
                    resultTextUI.gameObject.SetActive(true);

                    PlayerManager.instance.Id = id;
                    yield return new WaitForSeconds(2.0f);
                    PlayerManager.instance.Load(id);
                }
            }
            else if (string.Equals(result, "ID") || string.Equals(result, "Passward"))
            {
                if (resultTextUI != null)
                {
                    resultTextUI.text = "아이디나 비밀번호가 틀렸습니다.\n확인해 주세요";
                    resultTextUI.gameObject.SetActive(true);
                }
            }
        }
    }

    IEnumerator PostSignUpConnection(string id, string passward)
    {
        List<IMultipartFormSection> signForm = new List<IMultipartFormSection>();

        signForm.Add(new MultipartFormDataSection("NewID", id));
        signForm.Add(new MultipartFormDataSection("NewPassward", passward));

        UnityWebRequest WebRequest = UnityWebRequest.Post(string.Format("{00}{01}.php", url, signupConnectionName), signForm);

        yield return WebRequest.SendWebRequest();

        if (WebRequest != null)
        {
            string result = WebRequest.downloadHandler.text;

            if (string.Equals(result, "Create"))
            {
                if (resultTextUI != null)
                {
                    resultTextUI.text = "회원가입에 성공했습니다.";
                    resultTextUI.gameObject.SetActive(true);
                    yield return new WaitForSeconds(2.0f);

                    UIManager.instance.UISetting(UiKind.UiKind_LoginUI);
                    UIManager.instance.UISetting(UiKind.UIKind_SignUpUI);
                }
            }
            else if (string.Equals(result, "Exist"))
            {
                if (resultTextUI != null)
                {
                    resultTextUI.text = "아이디가 존재합니다.";
                    resultTextUI.gameObject.SetActive(true);
                }
            }
            else if (string.Equals(result, "Error"))
            {
                if (resultTextUI != null)
                {
                    resultTextUI.text = "아이디 생성 오류입니다.";
                    resultTextUI.gameObject.SetActive(true);
                }
            }

            Debug.Log(result);
        }

    }

    IEnumerator PostSaveConnection(string id, string saveData)
    {
        List<IMultipartFormSection> saveForm = new List<IMultipartFormSection>();

        saveForm.Add(new MultipartFormDataSection("ID", id));
        saveForm.Add(new MultipartFormDataSection("SaveData", saveData));

        UnityWebRequest WebRequest = UnityWebRequest.Post(string.Format("{00}{01}.php", url, saveConnectionName), saveForm);

        yield return WebRequest.SendWebRequest();

        if (WebRequest != null)
        {
            string result = WebRequest.downloadHandler.text;

            if (string.Equals(result, "Success"))
            {

            }
            else if (string.Equals(result, "Error"))
            {

            }
            Debug.Log(result);

        }
    }

    IEnumerator PostLoadConnection(string id, ServerDelegate<string> dele)
    {
        List<IMultipartFormSection> loadForm = new List<IMultipartFormSection>();

        loadForm.Add(new MultipartFormDataSection("ID", id));

        UnityWebRequest WebRequest = UnityWebRequest.Post(string.Format("{00}{01}.php", url, loadConnectionName), loadForm);
        yield return WebRequest.SendWebRequest();

        if (WebRequest != null)
        {
            string result = WebRequest.downloadHandler.text;

            if (string.Equals(result, "Error"))
            {
                if (resultTextUI != null)
                {
                    resultTextUI.text = "서버 오류입니다.";
                    resultTextUI.gameObject.SetActive(true);
                }
            }
            else if (string.Equals(result, "NotExisted"))
            {
                UIManager.instance.UISetting(UiKind.UiKind_LoginUI);
                UIManager.instance.UISetting(UiKind.UiKind_CustomUI);
            }
            else
            {
                Function(id, result, dele);
            }

        }

    }
}
