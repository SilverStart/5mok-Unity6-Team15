using System;
using common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_Text error;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        email.onValueChanged.AddListener((_) => error.enabled = false);
        password.onValueChanged.AddListener((_) => error.enabled = false);
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(() => { SceneManager.LoadScene((int)Constants.SceneName.SignUp); });
    }

    private void Login()
    {
        String savedEmail = PlayerPrefs.GetString("email");
        String savedPassword = PlayerPrefs.GetString("password");

        if (HasNoAccount(savedEmail, savedPassword))
        {
            ShowNoAccountMessage();
            return;
        }

        if (email.text == savedEmail && password.text == savedPassword)
        {
            SceneManager.LoadScene((int)Constants.SceneName.InGame);
        }
        else
        {
            error.enabled = true;
        }
    }

    private bool HasNoAccount(String email, String password)
    {
        return email == "" && password == "";
    }

    private void ShowNoAccountMessage()
    {
        error.enabled = true;
        error.text = "계정이 없습니다. 회원가입을 먼저 진행해주세요.";
    }

    void OnDestroy()
    {
        email.onValueChanged.RemoveAllListeners();
        password.onValueChanged.RemoveAllListeners();
        loginButton.onClick.RemoveAllListeners();
        signUpButton.onClick.RemoveAllListeners();
    }
}