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
    [SerializeField] private LayerMask _jumpingLayer;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _jumpForce;

    public static bool bRestoreLocation;
    public static Vector3 RestoreLocation;

    private bool bDodge = true;
    private bool bDblJump = false;
    private bool bUppercut = true;

    private bool bIsDodging = false;
    private bool bIsAttacking = false;

    int _currentWeapon;

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

        //Obtenemos la información sobre el estado de la animación del inicio del salto.
        AnimatorStateInfo state = ani.GetCurrentAnimatorStateInfo(0);

        //Estamos animando el salto
        if (state.IsName("Jump.Jump") && state.normalizedTime > 0.3f && state.normalizedTime < 0.4f)
        {
            rig.velocity = new Vector2(rig.velocity.x, _jumpForce);
        }
    }

    protected override void Update()
    {
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
            if (_bJumpPressed && !_bInAir)
            {
                base.ani.SetTrigger("Jump");
            }

            //Movimiento
            SimpleMove(Vector2.right * _horizontalAxis * _speed);
            if (_horizontalAxis != 0)
            {
                spr.flipX = _horizontalAxis > 0;
            }
        }

        //Esquiva
        if (_verticalAxis < 0 && bDodge && !bIsDodging)
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
    /// Aquí sólo llegamos si podíamos golpear
    /// </summary>
    protected override void OnAttack()
    {
        bIsAttacking = true;
        base.ani.SetTrigger("Attack");
    }

    public override void OnDeath()
    {


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
}
