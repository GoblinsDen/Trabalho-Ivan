using UnityEngine;
using Photon.Pun;

public class Player_movimento : MonoBehaviourPun
{
    [SerializeField] float speed = 10.0f; // Velocidade da raquete

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Movimentacao();
        }
    }

    void Movimentacao()
    {
        float moveVertical = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(0, moveVertical) * speed;
    }
}
