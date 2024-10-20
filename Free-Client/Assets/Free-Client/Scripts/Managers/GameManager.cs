using Assambra.GameFramework.GameManager;
using Assambra.GameFramework.CameraController;
using Assambra.GameFramework.MouseHandler;
using Assambra.FreeClient.Entities;
using CharacterInfo = Assambra.FreeClient.Entities.CharacterInfo;
using Assambra.FreeClient.Helper;
using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;


namespace Assambra.FreeClient.Managers
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        public string Account;
        [SerializeField] private NetworkManagerAccount _networkManagerAccount;

        [field: SerializeField] public CameraController CameraController { get; private set; }
        [field: SerializeField] public UIHandler UIHandler { get; private set; }
        [field: SerializeField] public SceneHandler SceneHandler { get; private set; }
        [field: SerializeField] public MouseHandler MouseHandler { get; private set; }
        [field: SerializeField] public Light DirectionalLight { get; private set; }
        [field: SerializeField] public Camera MainCamera { get; private set; }

        public Player Player { get; private set; }
        public DynamicCharacterAvatar Avatar { get; private set; }
        public List<CharacterInfo> CharacterInfos { get; set; } = new List<CharacterInfo>();
        public List<Character> CharacterList { get; set; } = new List<Character>();
        public Dictionary<string, PlayerController> PlayerSyncPositionDictionary = new Dictionary<string, PlayerController>();

        public bool CharacterCreatedAndReadyToPlay = false;
        public long CharacterId = 0;

        [Header("Player Prefab")]
        [SerializeField] GameObject playerPrefab;

        // Private
        private GameObject playerGameObject;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            ChangeState(GameState.Lobby);
        }

        private void Update()
        {
            CameraController.IsOverUIElement = MouseHandler.IsOverUIElement;

            switch (_currentState)
            {
                case GameState.Lobby:
                    //Debug.Log("GameManager::Update() Demo GameState Lobby");
                    break;
                case GameState.Game:
                    //Debug.Log("GameManager::Update() Demo GameState Game");
                    break;
            }
        }

        protected override void OnSceneChanged(Scene lastScene, Scene newScene)
        {
            if (newScene.name == Scenes.Login.ToString())
            {
                SetCameraDefaultValues();
            
                if(playerGameObject == null)
                {
                    playerGameObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    CameraController.CameraTarget = playerGameObject;
                    Player = playerGameObject.GetComponent<Player>();
                    Avatar = Player.Avatar;
                }
            }

            if (newScene.name == Scenes.SelectCharacter.ToString() ||
                newScene.name == Scenes.CreateCharacter.ToString())
            {
                SetCameraPreGameValues();
            }

            if(newScene.name == Scenes.World.ToString()) 
            {
                if(_currentState != GameState.Game)
                {
                    ChangeState(GameState.Game);

                    SetCameraGameCameraValues();
                    Destroy(playerGameObject);

                    _networkManagerAccount.enabled = false;
                }  
            }
            else
            {
                if(_currentState != GameState.Lobby)
                {
                    ChangeState(GameState.Lobby);

                    if (!_networkManagerAccount.enabled)
                        _networkManagerAccount.enabled = true;
                }
            }
        }

        private void SetCameraPreGameValues()
        {
            CameraController.ChangeCameraPreset("PreGameCamera");
            CameraController.ResetCameraAngles();
            CameraController.SetCameraPanAbsolutAngle(-180f);
        }

        private void SetCameraDefaultValues()
        {
            CameraController.ChangeCameraPreset("DefaultCamera");
            CameraController.ResetCameraAngles();
        }

        private void SetCameraGameCameraValues()
        {
            CameraController.ChangeCameraPreset("GameCamera");
            CameraController.ResetCameraAngles();
        }

        public GameObject SpawnPlayer(Character character)
        {
            GameObject pgo = GameObject.Instantiate(playerPrefab, character.position, Quaternion.Euler(character.rotation));
            pgo.name = character.characterName;
            PlayerController playerController = pgo.AddComponent<PlayerController>();

            PlayerSyncPositionDictionary.Add(character.accountUsername, playerController);

            Player player = pgo.GetComponent<Player>();
            playerController.Player = player;

            player.SetPlayerName(character.characterName);

            if(character.isLocalPlayer)
            {
                player.IsLocalPlayer = true;
            
                CameraController.CameraTarget = pgo;
                CameraController.ResetCameraAngles();
            }
        
            StartCoroutine(WaitForCharacterCreated(player, character.characterModel));

            return pgo;
        }

        IEnumerator WaitForCharacterCreated(Player player, string model)
        {
            while (!player.Initialized && !player.IsAvatarCreated)
            {
                Debug.Log("WaitForCharacterCreated");
                yield return new WaitForSeconds(0.05f);
            }
        
            player.Animator = player.Avatar.GetComponent<Animator>();
            player.GetCapsuleCollider();
            UMAHelper.SetAvatarString(player.Avatar, model);
        }
    }
}
