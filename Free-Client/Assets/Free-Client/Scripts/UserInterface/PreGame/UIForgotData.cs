using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Helper;
using Assambra.FreeClient.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class UIForgotData : MonoBehaviour
    {
        public Button ButtonTabUsername;
        public Button ButtonTabPassword;
        public Button ButtonBackUsername;
        public Button ButtonSendUsername;
        public Button ButtonBackPassword;
        public Button ButtonSendPassword;

        [SerializeField] private GameObject _forgotPassword;
        [SerializeField] private GameObject _forgotUsername;

        [SerializeField] private TMP_InputField _inputFieldUsernameOrEmail;
        [SerializeField] private TMP_InputField _inputFieldEmail;

        public void OnButtonBack()
        {
            GameManager.Instance.ChangeScene(Scenes.Login);
        }

        public void OnButtonSendPassword()
        {
            if (NetworkManagerAccount.Instance.Connected())
            {
                ButtonSendPassword.interactable = false;
                ButtonBackPassword.interactable = false;
                ButtonTabUsername.interactable = false;

                if (InputValidator.IsNotEmpty(_inputFieldUsernameOrEmail.text))
                    NetworkManagerAccount.Instance.ForgotPassword(_inputFieldUsernameOrEmail.text);
                else
                    ErrorPopup("Please note: You must enter either a username or email address. Please fill in the required field and try again.");
            }
            else
                ErrorPopup("Please note: We are currently not connected to a server.");
        }

        public void OnButtonSendUsername()
        {
            if (NetworkManagerAccount.Instance.Connected())
            {
                ButtonSendUsername.interactable = false;
                ButtonBackUsername.interactable = false;
                ButtonTabPassword.interactable = false;

                if (InputValidator.IsNotEmpty(_inputFieldEmail.text))
                    NetworkManagerAccount.Instance.ForgotUsername(_inputFieldEmail.text);
                else
                    ErrorPopup("Please note: The email address field cannot be empty. Please enter your email address and try again.");
            }
            else
                ErrorPopup("Please note: We are currently not connected to a server.");
        }

        public void OnButtonTabPassword()
        {
            if (!_forgotPassword.activeSelf)
                _forgotPassword.SetActive(true);
            if (_forgotUsername.activeSelf)
                _forgotUsername.SetActive(false);
        }

        public void OnButtonTapUsername()
        {
            if (!_forgotUsername.activeSelf)
                _forgotUsername.SetActive(true);
            if (_forgotPassword.activeSelf)
                _forgotPassword.SetActive(false);
        }

        private void ErrorPopup(string error)
        {
            string title = "Error";
            string info = error;

            ErrorPopup popup = PopupManager.Instance.ShowErrorPopup<ErrorPopup>(title, info, null);

            popup.Setup(
                title,
                info,
                () => { popup.Destroy(); }
            );
        }
    }
}
