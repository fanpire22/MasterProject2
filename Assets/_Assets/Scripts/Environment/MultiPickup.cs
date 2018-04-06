using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ePickup
{
    Health = 0,
    Rings5 = 1
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class MultiPickup : MonoBehaviour {

    [SerializeField] private AudioClip _sfxBreak;
    [SerializeField] private ePickup pickup;

    private Animator ani;

    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            //Nos ha golpeado el jugador: Rompemos y activamos el efecto del PowerUp
            AudioSource.PlayClipAtPoint(_sfxBreak, transform.position);
            ani.SetTrigger("Explode");

            switch (pickup)
            {
                case ePickup.Health:
                    collision.GetComponent<Damageable>().Heal(1);
                    break;
                case ePickup.Rings5:
                    collision.GetComponent<Knuckles>().AddPoints(5);
                    break;
            }

            Destroy(gameObject,1f);
        }
    }
}
