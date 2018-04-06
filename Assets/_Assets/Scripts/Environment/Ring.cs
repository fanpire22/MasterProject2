using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Ring : MonoBehaviour
{
    [SerializeField] private AudioClip _sfxRing;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            //Nos ha golpeado el jugador: Rompemos y activamos el efecto del PowerUp
            AudioSource.PlayClipAtPoint(_sfxRing, transform.position);

            collision.GetComponent<Knuckles>().AddPoints(1);

            Destroy(gameObject);
        }
    }
}
