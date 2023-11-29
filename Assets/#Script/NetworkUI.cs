using System.Collections;
using UnityEngine;
//Usar Photon
using Photon.Realtime;
using Photon.Pun;
//UsarUi
using UnityEngine.UI;
using TMPro;
//Configurar cenas
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviourPunCallbacks
{
    [Header("NetworkMenager")]
    [SerializeField] GameObject Network;

    [Header("Player")]
    [SerializeField] GameObject myPlayer;

    [Header("Screens/UI")]
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject roomUI;

    [Header("InGame")]
    [SerializeField] GameObject inGame;

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

        startGame.SetActive(false); // Initially disabled until we know the player is the host
        loginUI.gameObject.SetActive(true);
        lobbyUI.gameObject.SetActive(false);
        roomUI.gameObject.SetActive(false);
        inGame.gameObject.SetActive(false);

        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        logInButton.onClick.AddListener(OnLogIn);
        joinRoomButton.onClick.AddListener(searchQuickGame);
        createRoomButton.onClick.AddListener(CreateRoom);

        GenerateDefaultName();

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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    void CreatePlayer()
    {
        PhotonNetwork.Instantiate(myPlayer.name, myPlayer.transform.position, myPlayer.transform.rotation);
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UiHandler(roomUI, false);
            UiHandler(inGame, true);
        }
    }

}

