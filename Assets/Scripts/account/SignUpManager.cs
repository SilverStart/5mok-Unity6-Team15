using common;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignUpManager : MonoBehaviour
{
    [SerializeField] private TMP_Text error;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField passwordConfirm;

    [SerializeField] private Button signUpButton;

    [SerializeField] private GameObject successPanel;
    [SerializeField] private Button goToLoginButton;

    public void GoToLogin()
    {
        SceneManager.LoadScene((int)Constants.SceneName.Login);
    }

    void Start()
    {
        email.onValueChanged.AddListener((_) => error.enabled = false);
        password.onValueChanged.AddListener((_) => error.enabled = false);
        passwordConfirm.onValueChanged.AddListener((_) => error.enabled = false);
        signUpButton.onClick.AddListener(SignUp);
        goToLoginButton.onClick.AddListener(GoToLogin);
    }

    private void SignUp()
    {
        if (IsInvalidEmail())
        {
            error.enabled = true;
            error.text = "이메일을 입력해주세요.";
            return;
        }

        if (IsInvalidPassword())
        {
            error.enabled = true;
            error.text = "패스워드를 입력해주세요.";
            return;
        }

        if (IsInvalidPasswordConfirm())
        {
            error.enabled = true;
            error.text = "패스워드와 패드워드 확인의 입력값이 다릅니다.";
            return;
        }

        DoSignUp();
    }

    private bool IsInvalidEmail()
    {
        return email.text == "";
    }

    private bool IsInvalidPassword()
    {
        return password.text == "";
    }

    private bool IsInvalidPasswordConfirm()
    {
        return password.text != passwordConfirm.text;
    }

    private void DoSignUp()
    {
        PlayerPrefs.SetString("email", email.text);
        PlayerPrefs.SetString("password", AESCrypto.Encrypt(password.text));
        PlayerPrefs.Save();

        successPanel.SetActive(true);
    }

    void OnDestroy()
    {
        email.onValueChanged.RemoveAllListeners();
        password.onValueChanged.RemoveAllListeners();
        passwordConfirm.onValueChanged.RemoveAllListeners();
        signUpButton.onClick.RemoveAllListeners();
        goToLoginButton.onClick.RemoveAllListeners();
    }
}