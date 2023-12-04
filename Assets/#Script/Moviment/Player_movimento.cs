using UnityEngine;
using Photon.Pun;

public class Player_movimento : MonoBehaviourPun
{
    // Velocidade do player
    [SerializeField] float speed = 10.0f;

    // Rigidbody do player
    private Rigidbody2D rb;

    void Start()
    {
        // Pega o componente Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Checa se o PhotonView do jogador local
        if (photonView.IsMine)
        {
            // Chama a funcao de movimento
            Movimentacao();
        }
    }

    // Funcao que controla o movimento vertical
    void Movimentacao()
    {
        // Pega o input vertical (pra cima/baixo)
        float moveVertical = Input.GetAxis("Vertical");
        // Aplica a velocidade ao Rigidbody
        rb.velocity = new Vector2(0, moveVertical) * speed;
    }
}
