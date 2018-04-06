using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spikes : MonoBehaviour
{
    [SerializeField] float MaxMove;
    [SerializeField] float Time;
    [SerializeField] float pullBackForce;
    [SerializeField] bool up = true; //Esta variable es para diferenciar los pinchos que están debajo de una plataforma con los que están encima de una

    /// <summary>
    /// Hacemos que los pinchos suban y bajen si se han definido MaxMove y Time.
    /// </summary>
    void Start()
    {
        float movement = MaxMove + transform.position.y;
        transform.DOMoveY(movement, Time).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// En caso de colisionar con los pinchos, si es el jugador le dañamos y le aplicamos un pullBackForce para sacarlo.
    /// Si no queremos que salga del trigger, no le ponemos PullbackForce
    /// </summary>
    /// <param name="collision">jugador</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            collision.GetComponent<Damageable>().GetDamage();

            //Lanzamos sólo hacia arriba o hacia abajo porque el SimpleMove nos resetea la velocidad horizontal en todos los frames
            Vector3 pullback = ((up ? Vector3.up : Vector3.up * -1)) * pullBackForce; 
            Rigidbody2D rig = collision.GetComponent<Rigidbody2D>();

            rig.velocity = new Vector2();
            rig.AddForce(pullback, ForceMode2D.Impulse);
        }
    }
}
