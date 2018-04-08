using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlat : MonoBehaviour {

    [SerializeField] Transform PositionA;
    [SerializeField] Transform PositionB;
    [SerializeField] float _timeToTraverse = 5;
    [SerializeField] bool bPlayOnAwake = true;

    Sequence sequence;

    void Start()
    {
        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(PositionB.position, _timeToTraverse))
        .Append(transform.DOMove(PositionA.position, _timeToTraverse));

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
    /// Hacemos al jugador hijo de la plataforma para que se mueva respecto a ella
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Knuckles"))
        {
            other.transform.parent = this.transform;
        }
    }

    /// <summary>
    /// Liberamos al objeto de ser hijo de la plataforma
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            collision.transform.parent = null;
        }
    }
}
