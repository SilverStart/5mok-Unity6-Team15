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
        if (HasEmptyFields(email.text, password.text))
        {
            ShowErrorMessage("이메일과 패스워드를 입력해주세요.");
            return;
        }

        String savedEmail = PlayerPrefs.GetString("email");
        String savedEncryptedPassword = PlayerPrefs.GetString("password");

        if (HasNoAccount(savedEmail, savedEncryptedPassword))
        {
            ShowErrorMessage("계정이 없습니다. 회원가입을 먼저 진행해주세요.");
            return;
        }

        String encryptedPassword = AESCrypto.Encrypt(password.text);

        if (email.text == savedEmail && encryptedPassword == savedEncryptedPassword)
        {
            SceneManager.LoadScene((int)Constants.SceneName.InGame);
        }
        else
        {
            ShowErrorMessage("이메일 또는 패스워드가 다릅니다.");
        }
    }

    private bool HasEmptyFields(String email, String password)
    {
        return email == "" || password == "";
    }

    private bool HasNoAccount(String email, String password)
    {
        return email == "" && password == "";
    }

    private void ShowErrorMessage(String errorMessage)
    {
        error.enabled = true;
        error.text = errorMessage;
    }

    void OnDestroy()
    {
        email.onValueChanged.RemoveAllListeners();
        password.onValueChanged.RemoveAllListeners();
        loginButton.onClick.RemoveAllListeners();
        signUpButton.onClick.RemoveAllListeners();
    }
}