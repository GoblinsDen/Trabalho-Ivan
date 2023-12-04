using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//Usar Photon
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Photon.Pun;
//UsarUi
using UnityEngine.UI;
using TMPro;

public class NetworkUI : MonoBehaviourPunCallbacks
{

    public static NetworkUI instance;

    [Header("NetworkMenager")]
    [SerializeField] GameObject Network;

    [Header("Player e Bola")]
    [SerializeField] GameObject myPlayer;
    [SerializeField] GameObject Bola;

    [Header("Screens/UI")]
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject roomUI;

    [Header("Button")]
    [SerializeField] GameObject startGame; // Só o Host pode ver
    [SerializeField] Button startGameButton; //Só o Host pode ver
    [SerializeField] Button logInButton;
    [SerializeField] Button joinRoomButton;
    [SerializeField] Button createRoomButton;

    [Header("InputField")]
    [SerializeField] InputField playerInputName;
    [SerializeField] InputField roomInputName;

    [Header("Lista de Nomes")]
    [SerializeField] Text name;

    string playerNameTemp;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        startGame.SetActive(false); // Inicialmente desabilitado ate decidir quem eh o host
        loginUI.gameObject.SetActive(true);
        lobbyUI.gameObject.SetActive(false);
        roomUI.gameObject.SetActive(false);

        startGameButton.onClick.AddListener(StartGameRPC);
        logInButton.onClick.AddListener(OnLogIn);
        joinRoomButton.onClick.AddListener(searchQuickGame);
        createRoomButton.onClick.AddListener(CreateRoom);

        GenerateDefaultName();

    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnGameplayLoaded;
    }

    void GenerateDefaultName()
    {
        playerNameTemp = "Player_" + Random.Range(1, 99999);
        playerInputName.text = playerNameTemp;
    }

    public void UiHandler(GameObject ui, bool isActive)
    {
        ui.gameObject.SetActive(isActive);
    }

    public void OnLogIn()
    {

        Debug.Log("Botão Login pressionado");

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = playerInputName.text;
        name.text = playerInputName.text;
        UiHandler(loginUI, false);

    }

    public void searchQuickGame()
    {
        Debug.Log("Search quick game!");
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        Debug.Log("Criar Sala!");

        string roomName = roomInputName.text;

        if (roomName == "")
            roomName = "Room_" + Random.Range(1, 9999);

        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 2
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        
    }

    public override void OnConnected()
    {
        Debug.Log("Me conectei");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Me conectei ao master");
        UiHandler(loginUI, false);
        UiHandler(lobbyUI, true);

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entrei no Lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()

    {
        Debug.Log("Sala " + PhotonNetwork.CurrentRoom.Name + " criada");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrei na room " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Tem um total de " + PhotonNetwork.CurrentRoom.PlayerCount + " jogadores na sala");
        UiHandler(lobbyUI, false);
        UiHandler(roomUI, true);
        if (PhotonNetwork.IsMasterClient)
        {
            startGame.SetActive(true);
        }
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGame.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void JoinOrCreateNewGame()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Reconectando ao Photon...");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NickName = playerNameTemp;
        }
        else
        {
            Debug.Log("Já conectado. Tentando entrar em uma sala aleatória...");
            PhotonNetwork.JoinRandomRoom();
        }

        UiHandler(lobbyUI, false);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    void CreateBola()
    {
        PhotonNetwork.Instantiate(Bola.name, Bola.transform.position, Bola.transform.rotation);
    }

    void CreatePlayer()
    {
        object playerType;

        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("characterType", out playerType);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Vector2 startPositionPlayer = new Vector2(-(float)7.5, 0);
            Quaternion startRotationPlayer = new Quaternion(0, 0, 0, 1);
            PhotonNetwork.Instantiate(myPlayer.name, startPositionPlayer, startRotationPlayer);

        }
        else
        {
            Vector2 startPositionPlayer = new Vector2((float)7.5, 0);
            Quaternion startRotationPlayer = new Quaternion(0, 0, 0, 1);
            PhotonNetwork.Instantiate(myPlayer.name, startPositionPlayer, startRotationPlayer);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Gameplay");
    }
    public void StartGameRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("StartGame", RpcTarget.All);
        }
    }


    void OnGameplayLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "Gameplay")
        {
            if (FindObjectOfType<Player_movimento>() == null)
            {
                CreatePlayer();
            }
            else
            {
                Debug.Log("Nada");
            }

            // Verifica se já existe uma bola na cena para evitar duplicatas
            if (FindObjectOfType<Bola_Movimento>() == null && PhotonNetwork.IsMasterClient)
            {
                CreateBola();
            }
        }
        
    }



}

