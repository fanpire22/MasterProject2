using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Spring : MonoBehaviour
{
    [SerializeField] float _springForce;
    [SerializeField] AudioClip _sfxSpring;
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
            Rigidbody2D rig = collision.GetComponent<Rigidbody2D>();
            ani.SetTrigger("Pressed");
            rig.velocity = new Vector2(rig.velocity.x, _springForce);
            if (_sfxSpring) AudioSource.PlayClipAtPoint(_sfxSpring, transform.position);
        }
    }

}
