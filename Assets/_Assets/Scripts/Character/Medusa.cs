using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este enemigo se mueve flotando de un punto a otro (como una plataforma) y si colisiona con el jugador, 
//le hace daño, pero sólo cuando suelta la descarga eléctrica, que lanza a intervalos regulares. Siempre flota por encima del suelo,
//por lo que es necesario atacarla desde el aire, generalmente
[RequireComponent(typeof(Rigidbody2D))]
public class Medusa : BaseCharacter
{
    [SerializeField] Transform _positionA;
    [SerializeField] Transform _positionB;
    [SerializeField] AudioClip _sfxDeath;
    [SerializeField] AudioClip _sfxAttack;
    [SerializeField] Knuckles _knux;
    [SerializeField] float pullBackForce = 8;
    CircleCollider2D col;

    private Sequence sequence;
    private float distance;
    private bool bAlive = true;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<CircleCollider2D>();

    }

    private void Start()
    {
        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(_positionB.position, base._speed))
        .Append(transform.DOMove(_positionA.position, base._speed));

        sequence.SetLoops(-1).Pause();
        StartMoving();
    }

    public void StartMoving()
    {
        sequence.Play();
    }

    public void StopMoving()
    {

        sequence.Pause();
    }

    protected override void Update()
    {
        if (GameManager.Pause || !bAlive)
        {
            StopMoving();
            return;
        }
        else if (!sequence.IsPlaying())
        {
            StartMoving();
        }

        Vector2 dirToKnux = _knux.transform.position - transform.position;
        distance = dirToKnux.magnitude;
        Attack();
    }

    protected override void OnAttack()
    {
        ani.SetTrigger("Attack");
        if (_sfxAttack && distance < 6) PlaySound(_sfxAttack);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Obtenemos la información sobre el estado de la animación de ataque (puede que nos hayan golpeado)
        AnimatorStateInfo state = ani.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("MedusaAttack") && collision.CompareTag("Knuckles"))
        {
            //Es el jugador, y le hemos impactado, así que le hacemos daño y le empujamos
            if(collision.GetComponent<Damageable>().GetDamage()){

                Vector3 pullback = ((Vector3.up)) * pullBackForce;
                _knux.rig.AddForce(pullback, ForceMode2D.Impulse);
            }
        }
    }

    public override void OnDeath()
    {
        //Es posible que entremos en el OnDeath dos veces porque hayamos entrado Y salido del trigger, 
        //por lo que nos aseguramos de que ocurra sólo una vez

        if (bAlive)
        {
            bAlive = false;
            StopMoving();
            ani.SetTrigger("Death");
            if (_sfxDeath) PlaySound(_sfxDeath);
            Destroy(gameObject, 1f);
        }
    }
}
