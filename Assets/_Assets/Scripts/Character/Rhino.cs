using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este enemigo enemigo es muy similar a la medusa, con dos diferencias principales:
//La primera, que no tiene intervalos de ataque: chocar con él se considera ser impactado.
//Y la segunda, que en vez de seguir movimientos aéreos, sólo puede ir por el suelo
[RequireComponent(typeof(Rigidbody2D))]
public class Rhino : BaseCharacter
{
    [SerializeField] Transform _positionA;
    [SerializeField] Transform _positionB;
    [SerializeField] AudioClip _sfxDeath;
    [SerializeField] AudioClip _sfxTurn;
    [SerializeField] Knuckles _knux;
    [SerializeField] float pullBackForce = 8;

    private Sequence sequence;
    private float distance;
    CapsuleCollider2D col;
    private bool bAlive = true;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<CapsuleCollider2D>();

    }

    private void Start()
    {
        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(_positionB.position, base._speed).SetDelay(0.5f).OnComplete(() => { ani.SetTrigger("Flip"); }))
        .Append(transform.DOMove(_positionA.position, base._speed).SetDelay(0.5f).OnComplete(() => { ani.SetTrigger("Flip"); }));

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
    }

    //Evento para el animator de modo que suene cuando gire
    public void OnTurn()
    {
        if (_sfxTurn && distance < 6) PlaySound(_sfxTurn);
    }

    //Evento para el animator para que se voltee
    public void OnFinishTurn()
    {
        transform.rotation = (transform.rotation == Quaternion.Euler(0, 0, 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (bAlive && collision.collider.CompareTag("Knuckles"))
        {
            //Es el jugador, y le hemos impactado, así que le hacemos daño y le empujamos
            if (collision.collider.GetComponent<Damageable>().GetDamage())
            {
                Vector3 pullback = ((Vector3.up)) * pullBackForce;
                _knux.rig.AddForce(pullback, ForceMode2D.Impulse);
            }
        }
    }

    public override void OnDeath()
    {
        AnimatorStateInfo state = ani.GetCurrentAnimatorStateInfo(0);
        //Es posible que entremos en el OnDeath dos veces porque hayamos entrado Y salido del trigger, 
        //por lo que nos aseguramos de que ocurra sólo una vez

        if (bAlive)
        {
            bAlive = false;
            col.enabled = false;

            StopMoving();
            ani.SetTrigger("Death");
            if (_sfxDeath) PlaySound(_sfxDeath);
            Destroy(gameObject, 1f);
        }
    }

    protected override void OnAttack()
    {
    }
}
