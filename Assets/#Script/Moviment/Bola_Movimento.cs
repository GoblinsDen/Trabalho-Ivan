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

public class Bola_Movimento : MonoBehaviour
{
    [Header("Velocidade")]
    [SerializeField] float speed = 5.0f; // Velocidade inicial da bola
    [SerializeField] float acc = 0.25f; // Aceleração da bola

    [Header("Placar")]
    [SerializeField] TextMeshProUGUI Player1;
    [SerializeField] TextMeshProUGUI Player2;

    private int hitCounter;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("LancarBola", 3f);
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed + (acc * hitCounter));
    }

    void LancarBola()
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
            Player1.text = (int.Parse(Player1.text) + 1).ToString();
        }
        else if (transform.position.x < 0)
        {
            ResetarBola();
            Player2.text = (int.Parse(Player2.text) + 1).ToString();
        }
        if (Player1.text == "5")
        {
            Debug.Log("Player 1 venceu");
        }
        else if (Player2.text == "5")
        {
            Debug.Log("Player 2 venceu");
        }
    }

}
