using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [Range(1, 15)] public float Speed;
    public Vector2 Objective;

    Rigidbody2D _rig;
    Vector2 dirToObjective;

    private void Awake()
    {
        _rig = gameObject.GetComponent<Rigidbody2D>();

        Destroy(gameObject, 5f); // Si en cinco segundos de vuelo no choca con nada, desaparece
    }

    public void Shoot()
    {
        dirToObjective = Objective - (Vector2)transform.position;
        _rig.velocity = (dirToObjective.normalized * Speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Knuckles"))
        {
            //Es el jugador, le dañamos
            collision.collider.GetComponent<Damageable>().GetDamage();
        }

        //Golpee contra lo que golpee (suelos, paredes, otros badnicks), la bala desaparece
        CancelInvoke();
        Destroy(gameObject);
    }
}
