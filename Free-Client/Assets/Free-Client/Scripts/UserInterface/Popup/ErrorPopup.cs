using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class ErrorPopup : BasePopup
    {
        [SerializeField] Button _oKButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text informationText;

        public void Setup(string title, string information, Action onOK)
        {
            titleText.text = title;
            informationText.text = information;

            _oKButton.onClick.RemoveAllListeners();
            _oKButton.onClick.AddListener(() => { onOK?.Invoke(); });
        }

        public override void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
