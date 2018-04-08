using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class PowerUpMonitor : MonoBehaviour
{

    [SerializeField] string _titulo;
    [SerializeField] string _descripcion;
    [SerializeField] ePowerUps _powerUp;
    [SerializeField] AudioClip _sfxBreak;

    bool bOpenToolTip;
    private Animator ani;

    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            //Nos ha golpeado el jugador: Rompemos y mostramos el powerup
            bOpenToolTip = TooltipController.LoadToolTip(ToolTipLoaded);

            AudioSource.PlayClipAtPoint(_sfxBreak, transform.position);
            ani.SetTrigger("Explode");

            collision.GetComponent<Knuckles>().GetPowerUp(_powerUp);
            GameManager.Pause = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && bOpenToolTip)
        {
           TooltipController.unloadToolTip();
            GameManager.Pause = false;
            Destroy(gameObject);
        }
    }

    private void ToolTipLoaded()
    {
        TooltipController.Instance.InitializeToolTip(_titulo,_descripcion,_powerUp);
    }

}
