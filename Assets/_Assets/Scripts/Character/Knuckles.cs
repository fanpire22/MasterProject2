using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePowerUps
{
    Dodge = 0,
    DoubleJump = 1,
    TripleJump = 2,
    DownwardPunch = 3
}

public class Knuckles : BaseCharacter
{
    [Header("Variables propias de Knuckles")]
    [SerializeField]
    private LayerMask _jumpingLayer;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;

    [Header("Variables de sonidos de Knucles")]
    [SerializeField]
    private AudioClip _sfxJump;
    [SerializeField] private AudioClip _sfxDeath;

    public static bool bRestoreLocation;
    public static Vector3 RestoreLocation;

    private bool bDodge = true;
    private int iMaxJump = 2;
    private bool bUppercut = true;

    private int iJumps = 0;
    private float nextTimeCanJump;

    private bool bIsDodging = false;
    private bool bIsAttacking = false;
    private bool bJumpStarting = false;

    int _currentWeapon;

    //TODO: TEMPORAL
    bool bTheEnd = false;
    //END TODO


    private float _horizontalAxis;
    private float _verticalAxis;
    private bool _bJumpPressed;
    private bool _bAttackPressed;
    private bool _bInAir;
    private BoxCollider2D triggerAttack; //Este es el trigger que se activa durante las animaciones de ataque

    private Collider2D[] _footDetection = new Collider2D[5]; //Collider que detecta qué objetos estamos tocando con los pies
    void UpdateJump()
    {
        base.ani.SetBool("OnAir", _bInAir);
        if (bJumpStarting) bJumpStarting = !_bInAir;

        //Obtenemos la información sobre el estado de la animación del inicio del salto.
        AnimatorStateInfo state = ani.GetCurrentAnimatorStateInfo(0);

        //Estamos animando el salto
        if (state.IsName("Jump.Jump") && state.normalizedTime > 0.3f && state.normalizedTime < 0.4f)
        {
            rig.velocity = new Vector2(rig.velocity.x, _jumpForce);
            if (_sfxJump) AudioSource.PlayClipAtPoint(_sfxJump, transform.position);
        }

        //Estamos animando el salto
        if (state.IsName("Jump.DoubleJump") && state.normalizedTime < 0.1)
        {
            rig.velocity = new Vector2(rig.velocity.x, _jumpForce);
            if (_sfxJump) AudioSource.PlayClipAtPoint(_sfxJump, transform.position);
        }
    }

    protected override void Update()
    {

        //TODO: TEMPORAL
        if (bTheEnd) return;
        //END TODO

        base.Update();

        UpdateJump();


        _horizontalAxis = Input.GetAxis("Horizontal");
        _verticalAxis = Input.GetAxis("Vertical");
        _bJumpPressed = Input.GetButton("Jump");
        _bAttackPressed = Input.GetButton("Fire1");


        //Detectamos suelo por medio de una esfera ubicada en el punto de pivote (pies)
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, _detectionRadius, _footDetection, _jumpingLayer);
        _bInAir = count == 0;

        if (!bIsAttacking)
        {
            //Si no hay animación de ataque, podemos saltar o movernos. Si estamos atacando, sólo podemos cancelar la animación con una esquiva
            //( el equivalente a un Guard Cancel de un juego de lucha)

            //Salto
            if (_bJumpPressed)
            {
                if (Time.time > nextTimeCanJump)
                {
                    nextTimeCanJump = Time.time + _jumpCooldown;

                    if (!_bInAir)
                    {
                        SimpleMove(new Vector2());
                        iJumps = 0;
                        bJumpStarting = true;
                        base.ani.SetTrigger("Jump");
                    }
                    else
                    {
                        //Hacemos doble o triple salto si no hemos llegado al límite de saltos
                        if (iJumps < iMaxJump)
                        {
                            iJumps++;
                            bJumpStarting = false;
                            base.ani.SetTrigger("Jump");
                        }
                    }
                }
            }

            //Movimiento. No podemos movernos al iniciar el salto (luego en el aire, sí)
            if (!bJumpStarting && !bIsAttacking)
            {
                SimpleMove(Vector2.right * _horizontalAxis * _speed);
                if (_horizontalAxis != 0)
                {
                    spr.flipX = _horizontalAxis > 0;
                }
            }
        }

        //Esquiva. Sólo se puede hacer en el suelo
        if (_verticalAxis < 0 && bDodge && !bIsDodging && !_bInAir)
        {
            bIsAttacking = false;
            bIsDodging = true;
            //Hemos presionado "esquiva"
            base.ani.SetTrigger("Dodge");
        }

        //Ataque. Podemos encadenar ataques en el suelo, así que solo preguntamos si estamos en una animación de ataque si estamos en el aire
        if (_bAttackPressed)
        {
            if ((_bInAir && bUppercut && !bIsAttacking) || (!_bInAir))
            {
                base.Attack();
            }
        }
    }

    /// <summary>
    /// Acabamos de golpear a algo.
    /// </summary>
    /// <param name="collision">Objeto al que golpeamos</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable dmg = collision.GetComponent<Damageable>();
        if (dmg)
        {
            //Es un objeto que se puede dañar, le dañamos
        }
    }

    /// <summary>
    /// Aquí sólo llegamos si podíamos golpear
    /// </summary>
    protected override void OnAttack()
    {
        bIsAttacking = true;
        SimpleMove(new Vector2());
        base.ani.SetTrigger("Attack");
    }

    public override void OnDeath()
    {


        if (_sfxDeath) AudioSource.PlayClipAtPoint(_sfxDeath, transform.position);
    }

    /// <summary>
    /// cuando se ha cargado el nivel, restauramos la localización de Simon
    /// </summary>
    /// <param name="level">Nivel cargado</param>
    private void OnLevelWasLoaded(int level)
    {
        if (bRestoreLocation)
        {
            rig.MovePosition(RestoreLocation);
            bRestoreLocation = false;
        }
    }

    /// <summary>
    /// Evento que se llama desde el animator
    /// </summary>
    public void OnFinishDodge()
    {
        bIsDodging = false;
    }

    /// <summary>
    /// Evento que se llama desde el animator
    /// </summary>
    public void OnFinishAttack()
    {
        bIsAttacking = false;
    }

    public void PleaseBeKinematic()
    {
        rig.isKinematic = true;
        rig.velocity = new Vector2();
    }

    public void PleaseDontBeKinematic()
    {
        rig.isKinematic = false;
    }

    //TODO: TEMPORAL
    /// <summary>
    /// Entramos en el poste de victoria. Hacemos la animación
    /// </summary>
    public void Victory()
    {
        Move(new Vector2());
        rig.MovePosition(transform.position + Vector3.right);
        ani.SetTrigger("Victory");
        bTheEnd = true;
    }

    //END TODO
}
