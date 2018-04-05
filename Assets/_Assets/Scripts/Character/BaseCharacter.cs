using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ponemos los componentes que son obligatorios para que pueda ser un personaje. Es decir, un Animator (pues todos son animados),
//un RigidBody (pues todos tendrán detección de colisión), un SpriteRenderer (pues son sprites) y el Damageable (todos pueden ser dañados)
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Damageable))]

public abstract class BaseCharacter : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] [Range(1, 15)] protected float _speed;
    public Animator ani { get; protected set; }
    public Rigidbody2D rig { get; protected set; }
    public SpriteRenderer spr { get; protected set; }

    [Header("Combat")]
    [SerializeField] float _attackRate;
    public Damageable HealthManager { get; protected set; }
    protected float _nextTimeCanAttack;

    /// <summary>
    /// Inicializamos los componentes
    /// </summary>
    protected virtual void Awake()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        HealthManager = GetComponent<Damageable>();
    }

    protected void Move(Vector2 direction)
    {
        rig.velocity = direction;
        ani.SetFloat("Speed", direction.magnitude);
    }

    protected void SimpleMove(Vector2 direction)
    {
        rig.velocity = new Vector2(direction.x, rig.velocity.y);
        ani.SetFloat("Speed", Mathf.Abs(direction.x));
    }

    protected void Jump()
    {

    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// Aquí dejamos la lógica de si el personaje puede atacar. Si puede atacar, llamamos al "OnAttack" y ponemos el ataque en cooldown
    /// </summary>
    /// <returns>Si ha realizado el ataque (true) o si no (false)</returns>
    protected bool Attack()
    {
        if (Time.time > _nextTimeCanAttack)
        {
            _nextTimeCanAttack = Time.time + _attackRate;
            OnAttack();
            return true;
        }
        return false;
    }

    public virtual void OnDeath()
    {

    }

    protected abstract void OnAttack();
}
