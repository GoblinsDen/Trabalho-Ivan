using UnityEngine;
using UnityEngine.SceneManagement;
//Usar Photon
using Photon.Realtime;
using Photon.Pun;
//UsarUi
using UnityEngine.UI;

public class NetworkUI : MonoBehaviourPunCallbacks
{
    // Variavel serializada pra UI, jogador, bola etc.
    public static NetworkUI instance;
    [Header("NetworkMenager")]
    [SerializeField] GameObject Network;
    [Header("Player e Bola")]
    [SerializeField] GameObject myPlayer;
    [SerializeField] GameObject Bola;

    // UIs
    [Header("Screens/UI")]
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject roomUI;

    // Botoes
    [Header("Button")]
    [SerializeField] GameObject startGame; // So o master ve
    [SerializeField] Button startGameButton; // idem
    [SerializeField] Button logInButton;
    [SerializeField] Button joinRoomButton;
    [SerializeField] Button createRoomButton;

    // Campos de texto pra nome do player e da sala
    [Header("InputField")]
    [SerializeField] InputField playerInputName;
    [SerializeField] InputField roomInputName;

    string playerNameTemp;

    void Start()
    {
        // Sincroniza as cenas entre jogadores
        PhotonNetwork.AutomaticallySyncScene = true;

        // Configuração inicial da UI
        startGame.SetActive(false);
        loginUI.gameObject.SetActive(true);
        lobbyUI.gameObject.SetActive(false);
        roomUI.gameObject.SetActive(false);

        // Add listeners dos botoes
        startGameButton.onClick.AddListener(StartGameRPC);
        logInButton.onClick.AddListener(OnLogIn);
        joinRoomButton.onClick.AddListener(searchQuickGame);
        createRoomButton.onClick.AddListener(CreateRoom);

        // Gera um nome padrao pro jogador
        GenerateDefaultName();
    }

    // Garante que o objeto n seja destruido ao carregar novas cenas
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnGameplayLoaded;
    }

    // Gera um nome random
    void GenerateDefaultName()
    {
        playerNameTemp = "Player_" + Random.Range(1, 99999);
        playerInputName.text = playerNameTemp;
    }

    // Controla o trigger das UI
    public void UiHandler(GameObject ui, bool isActive)
    {
        ui.gameObject.SetActive(isActive);
    }

    // Conecta ao Photon quando clica no botão de login
    public void OnLogIn()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = playerInputName.text;
        UiHandler(loginUI, false);
    }

    // Entra em uma room ja existente
    public void searchQuickGame()
    {
        PhotonNetwork.JoinLobby();
    }

    // Cria uma sala com um nome aleatorio ou especific
    public void CreateRoom()
    {
        string roomName = roomInputName.text;
        if (roomName == "")
            roomName = "Room_" + Random.Range(1, 9999);
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // metodos que lidam com eventos de conexão
    public override void OnConnected() 
    { 
        Debug.Log("Me conectei"); 
    }
    public override void OnConnectedToMaster()
    {
        //Ajusta as telas
        UiHandler(loginUI, false); // parece redundante, mas no 2 ciclo de gameplay tava aparecendo qd n deveria
        UiHandler(lobbyUI, true);
    }
    public override void OnJoinedLobby() 
    { 
        PhotonNetwork.JoinRandomRoom(); 
    }
    public override void OnCreatedRoom() 
    { 
        Debug.Log("Sala criada"); 
    }
    public override void OnJoinedRoom()
    {
        UiHandler(lobbyUI, false);
        UiHandler(roomUI, true);
        if (PhotonNetwork.IsMasterClient) 
        { 
            startGame.SetActive(true); //metodo que deixa so o master startar o game
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient) 
    { 
        startGame.SetActive(PhotonNetwork.IsMasterClient); 
    }

    // Metodo criado para funcionar mais de um ciclo de gameplay
    public void JoinOrCreateNewGame()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NickName = playerNameTemp;
        }
        else 
        { 
            PhotonNetwork.JoinRandomRoom(); 
        }
        UiHandler(lobbyUI, false); 
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }
    //Metodo que instancia a bola
    void CreateBola()
    {
        PhotonNetwork.Instantiate(Bola.name, Bola.transform.position, Bola.transform.rotation);
    }
    //Metodo que instancia os players
    void CreatePlayer()
    {
        object playerType;

        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("characterType", out playerType);

        if (PhotonNetwork.LocalPlayer.IsMasterClient) // Se for o master vai na esquerda do mapa
        {
            Vector2 startPositionPlayer = new Vector2(-(float)7.5, 0);
            Quaternion startRotationPlayer = new Quaternion(0, 0, 0, 1);
            PhotonNetwork.Instantiate(myPlayer.name, startPositionPlayer, startRotationPlayer);

        }
        else // Se nao, vai a direita do mapa
        {
            Vector2 startPositionPlayer = new Vector2((float)7.5, 0);
            Quaternion startRotationPlayer = new Quaternion(0, 0, 0, 1);
            PhotonNetwork.Instantiate(myPlayer.name, startPositionPlayer, startRotationPlayer);
        }
    }

    //Troca de cena
    [PunRPC]
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Gameplay");
    }
    public void StartGameRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (GetComponent<PhotonView>() != null)
            {
                GetComponent<PhotonView>().RPC("StartGame", RpcTarget.All);
            }
            else
            {
                Debug.LogWarning("PhotonView nao foi encotrado no objeto.");
            }
        }
    }
    void OnGameplayLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay")
        {
            // Verifica se ja existe players na cena para evitar duplicatas
            if (FindObjectOfType<Player_movimento>() == null)
            {
                CreatePlayer();
            }
            else
            {
                Debug.Log("Nada");
            }

            // Verifica se ja existe uma bola na cena para evitar duplicatas
            if (FindObjectOfType<Bola_Movimento>() == null && PhotonNetwork.IsMasterClient)
            {
                CreateBola();
            }
        }        
    }
}

