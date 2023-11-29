using UnityEngine;

public class Player_movimento : MonoBehaviour
{
    [SerializeField] float speed = 10.0f; // Velocidade da raquete
    [SerializeField] GameObject Bola;

    private Rigidbody2D rb;
    private Vector2 mov;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Movimentacao();
    }

    void Movimentacao()
    {
        mov = new Vector2(0, Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        rb.velocity = mov * speed;
    }
}
