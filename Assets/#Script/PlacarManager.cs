using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlacarManager : MonoBehaviourPunCallbacks
{
    // Declara variaveis pra UI
    [Header("Telas")]
    [SerializeField] GameObject Vitoria;
    [SerializeField] GameObject Derrota;
    [SerializeField] GameObject HUD;

    [Header("Placar")]
    public TextMeshProUGUI Player1Placar;
    public TextMeshProUGUI Player2Placar;

    // Botoes de UI
    [Header("buttons")]
    [SerializeField] Button Vitoria_btn;
    [SerializeField] Button Derrota_btn;

    //Adiciona listeners aos botões
    private void Start()
    {
        Vitoria_btn.onClick.AddListener(BacktoMenu);
        Derrota_btn.onClick.AddListener(BacktoMenu);
    }

    // Atualiza o placar, chama qdo um jogador faz ponto
    public void AtualizarPlacar(int player, int pontos)
    {
        if (player == 1)
        {
            Player1Placar.text = pontos.ToString();
            //qdo jogador chegar a 5 pontos, termina o jogo
            if (pontos >= 5)
            {
                EndGame(1);
            }
        }
        else if (player == 2)
        {
            Player2Placar.text = pontos.ToString();
            if (pontos >= 5)
            {
                EndGame(2);
            }
        }
    }

    // RPC pra atualizar o placar
    [PunRPC]
    public void AtualizarPlacarRPC(int player, int pontos)
    {
        AtualizarPlacar(player, pontos);
    }

    // Metodo chamado qdo o jogo termina
    void EndGame(int winner)
    {
        // Desativa a UI do jogo e ativa a de vitória/derrota
        AtivarDesativarUI("HUD", false);

        // Mostra tela de vitória ou derrota dependendo do jogador
        if ((winner == 1 && photonView.IsMine) || (winner == 2 && !photonView.IsMine))
        {
            AtivarDesativarUI("Vitoria", true);
        }
        else
        {
            AtivarDesativarUI("Derrota", true);
        }
    }

    // Ativar/desativar as UIs
    public void AtivarDesativarUI(string uiName, bool ativar)
    {
        switch (uiName)
        {
            case "Vitoria":
                Vitoria.SetActive(ativar);
                break;
            case "Derrota":
                Derrota.SetActive(ativar);
                break;
            case "HUD":
                HUD.SetActive(ativar);
                break;
        }
    }

    // Volta pro menu principal
    public void BacktoMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Chamado qdo o jogador sai da sala
    public override void OnLeftRoom()
    {
        // Procura e destroi o obj "Multiplayer" q pode causar conflito de ID
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (var view in photonViews)
        {
            if (view.gameObject.name == "Multiplayer" && view.IsMine)
            {
                PhotonNetwork.CleanRpcBufferIfMine(view);
                PhotonNetwork.Destroy(view.gameObject);
                break;
            }
        }

        // Carrega a cena do menu
        SceneManager.LoadScene("SampleScene");
    }

}
