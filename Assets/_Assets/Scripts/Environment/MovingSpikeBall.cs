using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpikeBall : MonoBehaviour {

    [SerializeField] Transform PositionA;
    [SerializeField] Transform PositionB;
    [SerializeField] bool bPlayOnAwake = true;
    [SerializeField] float pullBackForce;

    Sequence sequence;

    void Start()
    {
        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(PositionB.position, 2))
        .Append(transform.DOMove(PositionA.position, 2));

        sequence.SetLoops(-1).Pause();
        if (bPlayOnAwake) StartMoving();
    }

    public void StartMoving()
    {
        sequence.Play();
    }

    public void StopMoving()
    {

        sequence.Pause();
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

            //Lanzamos sólo hacia arriba porque el SimpleMove nos resetea la velocidad horizontal en todos los frames
            Vector3 pullback = Vector3.up * pullBackForce;
            Rigidbody2D rig = collision.GetComponent<Rigidbody2D>();

            rig.velocity = new Vector2();
            rig.AddForce(pullback, ForceMode2D.Impulse);
        }
    }
}
