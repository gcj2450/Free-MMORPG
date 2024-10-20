using Assambra.FreeClient.Helper;
using Assambra.FreeClient.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class UIAccountActivation : MonoBehaviour
    {
        [SerializeField] private Button _buttonSendActivationCode;
        [SerializeField] private Button _buttonResendActivationEmail;
        [SerializeField] private Button _buttonQuit;
        [SerializeField] private TMP_InputField _inputFieldActivationCode;

        public void OnButtonSendActivationCode()
        {
            string activationCode = _inputFieldActivationCode.text;

            if (InputValidator.IsNotEmpty(activationCode))
                NetworkManagerAccount.Instance.ActivateAccount(activationCode);
            else
                ErrorPopup("Please note: The activation input field cannot be empty. Please enter your activation code and try again.");

        }

        public void OnButtonResendActivationEmail()
        {
            _buttonResendActivationEmail.interactable = false;
            NetworkManagerAccount.Instance.ResendActivationCodeEmail();
        }

        public void OnButtonQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
