using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlacarManager : MonoBehaviourPunCallbacks
{
    [Header("Telas")]
    [SerializeField] GameObject Vitoria;
    [SerializeField] GameObject Derrota;
    [SerializeField] GameObject HUD;

    [Header("Placar")]
    public TextMeshProUGUI Player1Placar;
    public TextMeshProUGUI Player2Placar;

    [Header("buttons")]
    [SerializeField] Button Vitoria_btn;
    [SerializeField] Button Derrota_btn;

    private void Start()
    {
        Vitoria_btn.onClick.AddListener(BacktoMenu);
        Derrota_btn.onClick.AddListener(BacktoMenu);
    }

    public void AtualizarPlacar(int player, int pontos)
    {
        if (player == 1)
        {
            Player1Placar.text = pontos.ToString();
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


    [PunRPC]
    public void AtualizarPlacarRPC(int player, int pontos)
    {
        AtualizarPlacar(player, pontos);
    }

    // Método para ser chamado quando o jogo termina
    void EndGame(int winner)
    {
        AtivarDesativarUI("HUD", false);

        if ((winner == 1 && photonView.IsMine) || (winner == 2 && !photonView.IsMine))
        {
            AtivarDesativarUI("Vitoria", true);
        }
        else
        {
            AtivarDesativarUI("Derrota", true);
        }
    }


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
    public void BacktoMenu()
    {
        // Primeiro deixar a sala para disparar o callback OnLeftRoom
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // Limpar todos os objetos de rede
        foreach (var photonView in FindObjectsOfType<PhotonView>())
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }

        // Depois de sair da sala e limpar os objetos, desconectar do Photon e carregar a cena do menu
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("SampleScene");
    }

}