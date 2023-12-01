using UnityEngine;
//Usar Photon
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Bola_Movimento : MonoBehaviourPunCallbacks
{
    [Header("Velocidade")]
    [SerializeField] float speed = 5.0f; // Velocidade inicial da bola
    [SerializeField] float acc = 0.25f; // Aceleração da bola

    TextMeshProUGUI Player1;
    TextMeshProUGUI Player2;

    private PlacarManager placarManager;

    private int hitCounter;
    private Rigidbody2D rb;

    void Start()
    {
        placarManager = FindObjectOfType<PlacarManager>();
        if (placarManager != null)
        {
            Player1 = placarManager.Player1Placar;
            Player2 = placarManager.Player2Placar;
            placarManager.AtivarDesativarUI("Vitoria", false);
            placarManager.AtivarDesativarUI("Derrota", false);
            placarManager.AtivarDesativarUI("HUD", true);

        }

        rb = GetComponent<Rigidbody2D>();
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("LancarBola", 3f);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed + (acc * hitCounter));
    }

    void LancarBola()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            float x = Random.Range(0, 2) == 0 ? -1 : 1;
            float y = Random.Range(0, 2) == 0 ? -1 : 1;
            rb.velocity = new Vector2(speed * x, speed * y);
        }
    }

    void ResetarBola()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            rb.velocity = Vector2.zero;
            transform.position = Vector2.zero;
            hitCounter = 0;
            Invoke("LancarBola", 3f);
        }
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
        if (PhotonNetwork.IsMasterClient)
        {
            if (transform.position.x > 0)
            {
                ResetarBola();
                placarManager.photonView.RPC("AtualizarPlacarRPC", RpcTarget.All, 1, int.Parse(Player1.text) + 1);
            }
            else if (transform.position.x < 0)
            {
                ResetarBola();
                placarManager.photonView.RPC("AtualizarPlacarRPC", RpcTarget.All, 2, int.Parse(Player2.text) + 1);
            }
        }
    }

}
