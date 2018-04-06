using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Goal : MonoBehaviour
{

    [SerializeField] AudioClip _sfxGoal;
    private Animator ani;


    // Use this for initialization
    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            Knuckles knux = collision.GetComponent<Knuckles>();
            ani.SetTrigger("Active");
            knux.Victory();
            if (_sfxGoal) AudioSource.PlayClipAtPoint(_sfxGoal, transform.position);
        }
    }

}
