using UnityEngine;
using TMPro;


public class UILogin : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;

    private string password;
    private string username;


    public void OnButtonQuit()
    {
        NetworkManager.Instance.Disconnect();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnButtonLogin()
    {
        username = inputFieldUsername.text;
        password = inputFieldPassword.text;

        NetworkManager.Instance.Login(username, password, true);
    }

    public void OnButtonNeedAccount()
    {
        GameManager.Instance.ChangeScene(Scenes.CreateAccount);
    }

    public void OnButtonForgotData()
    {
        GameManager.Instance.ChangeScene(Scenes.ForgotData);
    }
}
