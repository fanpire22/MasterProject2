using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : BaseCharacter
{

    [SerializeField] Bullet _prefBullet;
    [SerializeField] Transform _bulletSpawner;
    [SerializeField] Knuckles _knux;
    [SerializeField] float _aggroDistance = 6;
    [SerializeField] float _maxAttackRange = 3;
    [SerializeField] AudioClip _sfxMove;
    [SerializeField] AudioClip _sfxShoot;
    [SerializeField] AudioClip _sfxDeath;

    private bool bAlive = true;
    private CapsuleCollider2D col;

    protected override void Awake()
    {
        base.Awake();

        col = GetComponent<CapsuleCollider2D>();
    }

    protected override void OnAttack()
    {
        ani.SetTrigger("Attack");
    }

    public void OnShoot()
    {
        Bullet bala = Instantiate(_prefBullet);
        bala.transform.position = _bulletSpawner.position;
        bala.Objective = _knux.transform.position;
        bala.Shoot();

        AudioSource.PlayClipAtPoint(_sfxShoot, transform.position);
    }

    // Si detecta a Knuckles a menos de 6 metros, se lanza a por él. Si está a 3 metros o menos, le dispara, pero siempre se mantiene a 1 metro por encima
    protected override void Update()
    {
        if (GameManager.Pause || !bAlive) return;

        Vector2 knuxOffset = new Vector2(_knux.transform.position.x, _knux.transform.position.y + 1f);

        Vector2 dirToKnuckles = knuxOffset - (Vector2)transform.position;
        Move(dirToKnuckles.normalized * 0); //Paramos el movimiento, por si acaso

        if (dirToKnuckles.magnitude < _aggroDistance)
        {
            PlaySound(_sfxMove);
            if (_maxAttackRange > dirToKnuckles.magnitude)
            {
                Attack();
            }
            else
            {
                //Vamos a por Knuckles
                Move(dirToKnuckles.normalized * _speed);
            }
        }
        else
        {
            //Esta muy lejos. Paramos los sonidos
            audio.Stop();
        }

        if (rig.velocity.normalized.x > 0)
        {
            //Vamos hacia la derecha
            if (transform.rotation.eulerAngles.y == 0)
            {
                ani.SetTrigger("Flip");
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

        }else if (rig.velocity.normalized.x < 0)
        {
            //Vamos hacia la izquierda
            if (transform.rotation.eulerAngles.y == 180 || transform.rotation.eulerAngles.y == -180)
            {
                ani.SetTrigger("Flip");
                transform.rotation = Quaternion.Euler(0, 0, 0);
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
            col.enabled = false;

            ani.SetTrigger("Death");
            audio.Stop();
            audio.loop = false;
            audio.volume = 1;

            PlaySound(_sfxDeath);

            Destroy(gameObject, 1);
        }

    }
}
