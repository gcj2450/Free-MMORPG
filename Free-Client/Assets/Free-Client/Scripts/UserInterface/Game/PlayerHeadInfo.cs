using Assambra.FreeClient.Entities;
using Assambra.FreeClient.Managers;
using UnityEngine;
using TMPro;

namespace Assambra.FreeClient.UserInterface
{
    public class PlayerHeadInfo : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private TMP_Text playerNameText;

        private void LateUpdate()
        {
            transform.rotation = GameManager.Instance.CameraController.transform.rotation;
        }

        public void SetPlayername(string playerName)
        {
            playerNameText.text = playerName;
        }

        public void SetPlayerInfoPosition(float heightDiff)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - heightDiff, transform.position.z);
        }
    }
}
