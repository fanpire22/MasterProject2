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
    [SerializeField] private GameManager _gm;
    [SerializeField] private LayerMask _jumpingLayer;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _runningMultiplier = 1.5f;

    [Header("Variables de sonidos de Knucles")]
    [SerializeField] private AudioClip _sfxJump;
    [SerializeField] private AudioClip _sfxDeath;
    [SerializeField] private AudioClip _sfxBrake;

    public static bool bRestoreLocation;
    public static Vector3 RestoreLocation;

    public bool bDodge { get; private set; }
    public int iMaxJump { get; private set; }
    public bool bUppercut { get; private set; }

    private int iJumps = 0;
    private float nextTimeCanJump;

    private bool bIsDodging = false;
    private bool bIsAttacking = false;
    private bool bJumpStarting = false;

    //TODO: TEMPORAL
    bool bTheEnd = false;
    //END TODO

    private float _horizontalAxis;
    private float _verticalAxis;
    private bool _bJumpPressed;
    private bool _bAttackPressed;
    private bool _bInAir;
    private BoxCollider2D triggerAttack; //Este es el trigger que se activa durante las animaciones de ataque

    protected override void Awake()
    {
        base.Awake();

        bDodge = false;
        iMaxJump = 0;
        bUppercut = false;
    }

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
            if (_sfxJump) PlaySound(_sfxJump);
        }

        //Estamos animando el salto
        if (state.IsName("Jump.DoubleJump") && state.normalizedTime < 0.1)
        {
            rig.velocity = new Vector2(rig.velocity.x, _jumpForce);
            if (_sfxJump) PlaySound(_sfxJump);
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
        float speed = (Input.GetKey(KeyCode.LeftShift) ? _runningMultiplier * _speed : _speed);


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
                    bIsDodging = false;
                    nextTimeCanJump = Time.time + _jumpCooldown;

                    if (!_bInAir)
                    {
                        SimpleMove(new Vector2());
                        iJumps = 0;
                        bJumpStarting = true;
                        base.ani.SetTrigger("Jump");
                        ani.SetBool("DidJump", true);
                    }
                    else
                    {
                        //Hacemos doble o triple salto si no hemos llegado al límite de saltos
                        if (iJumps < iMaxJump)
                        {
                            iJumps++;
                            bJumpStarting = false;
                            base.ani.SetTrigger("Jump");
                            ani.SetBool("DidJump", true);
                        }
                    }
                }
            }

            //Movimiento. No podemos movernos al iniciar el salto (luego en el aire, sí)
            if (!bJumpStarting && !bIsAttacking)
            {
                bIsDodging = false;
                SimpleMove(Vector2.right * _horizontalAxis * speed);
                if (_horizontalAxis != 0)
                {
                    transform.rotation = Quaternion.Euler(0,(_horizontalAxis > 0? 180: 0),0);
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
                bIsDodging = false;
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
        if (_bInAir)
        {
            //Para evitar que la animación se rompa en el caso de lanzar un puño al caer y no al saltar, "trampeamos" y decimos que sí ha saltado antes de atacar
            ani.SetBool("DidJump", true);
        }
    }

    public override void OnDeath()
    {
        if (_sfxDeath) PlaySound(_sfxDeath);
    }

    /// <summary>
    /// cuando se ha cargado el nivel, restauramos la localización de Knuckles y sus atributos
    /// </summary>
    private void OnLevelWasLoaded()
    {
        if (bRestoreLocation)
        {
            rig.MovePosition(RestoreLocation);
            bRestoreLocation = false;
        }

    }

    /// <summary>
    /// Esta función se llama desde la carga de datos, y nos permite establecer las habilidades de Knuckles de nuevo
    /// </summary>
    /// <param name="MaxJump"></param>
    /// <param name="Dodge"></param>
    /// <param name="Uppercut"></param>
    public void RestoreSkills(int MaxJump, bool Dodge, bool Uppercut)
    {
        iMaxJump = MaxJump;
        bDodge = Dodge;
        bUppercut = Uppercut;
    }

    /// <summary>
    /// Evento que se llama desde el animator al finalizar la esquiva
    /// </summary>
    public void OnFinishDodge()
    {
        bIsDodging = false;
    }

    /// <summary>
    /// Evento que se llama desde el animator al finalizar un ataque
    /// </summary>
    public void OnFinishAttack()
    {
        bIsAttacking = false;
    }

    /// <summary>
    /// Control para volver el rigidBody Kinematic durante ciertas animaciones
    /// </summary>
    public void PleaseBeKinematic()
    {
        rig.isKinematic = true;
        rig.velocity = new Vector2();
    }

    /// <summary>
    /// Control para devolver el rigidBody a su estado normal durante ciertas animaciones
    /// </summary>
    public void PleaseDontBeKinematic()
    {
        rig.isKinematic = false;
    }

    /// <summary>
    /// Control para que suene el ruido de frenazo cuando el personaje frene
    /// </summary>
    public void OnBrake()
    {
        PlaySound(_sfxBrake);
    }

    /// <summary>
    /// Control para el animator: hemos dejado de saltar y ya podemos caer
    /// </summary>
    public void OnJumpEnd()
    {
        ani.SetBool("DidJump", false);
    }

    public void GetPowerUp(ePowerUps powerUp)
    {
        switch (powerUp)
        {
            case ePowerUps.Dodge:
                bDodge = true;
                break;
            case ePowerUps.DoubleJump:
                iMaxJump = 1;
                break;
            case ePowerUps.TripleJump:
                iMaxJump = 2;
                break;
            case ePowerUps.DownwardPunch:
                bUppercut = true;
                break;
        }
    }

    public override void updateLife(int life)
    {
        _gm.SetLifeShown(life);
    }
    
    public void AddPoints(int amount)
    {
        _gm.AddPoints(amount);
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
