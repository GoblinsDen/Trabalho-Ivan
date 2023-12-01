using System.Collections;
using UnityEngine;
//Usar Photon
using Photon.Realtime;
using Photon.Pun;
//UsarUi
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Bola_Movimento : MonoBehaviourPunCallbacks
{
    [Header("Velocidade")]
    [SerializeField] float speed = 5.0f; // Velocidade inicial da bola
    [SerializeField] float acc = 0.25f; // Aceleração da bola

    [Header("Placar")]
    [SerializeField] TextMeshProUGUI Player1;
    [SerializeField] TextMeshProUGUI Player2;

    [Header("Telas")]
    [SerializeField] GameObject Vitoria;
    [SerializeField] GameObject Derrota;
    [SerializeField] GameObject HUD;

    [Header("Button")]

    [SerializeField] Button BackMenu;
    [SerializeField] Button BackMenu2;

    private int hitCounter;
    private Rigidbody2D rb;

    void Start()
    {
        Vitoria.gameObject.SetActive(false);
        Derrota.gameObject.SetActive(false);
        HUD.gameObject.SetActive(true);

       BackMenu.onClick.AddListener(BacktoMenu);
       BackMenu2.onClick.AddListener(BacktoMenu);

        rb = GetComponent<Rigidbody2D>();
        Invoke("LancarBola", 3f);
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed + (acc * hitCounter));
    }

    public void LancarBola()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new Vector2(speed + (acc * hitCounter) * x, speed + (acc * hitCounter) * y);
    }

    void ResetarBola()
    {
        rb.velocity = new Vector2(0, 0);
        transform.position = new Vector2(0, 1);
        hitCounter = 0;
        Invoke("LancarBola", 3f);
    }

    void Rebatimento(Transform myObject)
    {
        hitCounter++;
        Vector2 bolaPos = transform.position;
        Vector2 playerPos = myObject.position;

        float direcaoX, direcaoY;
        if (transform.position.x > 0)
        {
            direcaoX = -1;
        }
        else
        {
            direcaoX = 1;
        }
        direcaoY = (bolaPos.y - playerPos.y) / myObject.GetComponent<Collider2D>().bounds.size.y;
        if (direcaoY == 0)
        {
            direcaoY = 0.25f;
        }

        rb.velocity = new Vector2(direcaoX, direcaoY) * (speed + (acc * hitCounter));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rebatimento(collision.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.x > 0)
        {
            ResetarBola();
            UpdateScore(1); // Atualiza o placar do Player 1
        }
        else if (transform.position.x < 0)
        {
            ResetarBola();
            UpdateScore(2); // Atualiza o placar do Player 2
        }
    }

    [PunRPC]
    void UpdateScore(int player)
    {
        if (player == 1)
        {
            Player1.text = (int.Parse(Player1.text) + 1).ToString();
            if (Player1.text == "5")
            {
                photonView.RPC("EndGame", RpcTarget.All, 1);
            }
        }
        else if (player == 2)
        {
            Player2.text = (int.Parse(Player2.text) + 1).ToString();
            if (Player2.text == "5")
            {
                photonView.RPC("EndGame", RpcTarget.All, 2);
            }
        }
    }

    void BacktoMenu()
    {
        PhotonNetwork.LoadLevel("SampleScene");
    }

    [PunRPC]
    void EndGame(int winner)
    {
        
        if ((winner == 1 && photonView.IsMine) || (winner == 2 && !photonView.IsMine))
        {
            HUD.gameObject.SetActive(false);
            Vitoria.gameObject.SetActive(true);
        }
        else
        {
            HUD.gameObject.SetActive(false);
            Derrota.gameObject.SetActive(true);
        }

        PhotonNetwork.Disconnect();
    }

}
