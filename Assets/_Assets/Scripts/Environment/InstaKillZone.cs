using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este script es para los límites de pantalla: Si llegas a esta zona, es muerte automática porque te has caído por un barranco
public class InstaKillZone : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knuckles"))
        {
            //Es el jugador, le matamos
            collision.GetComponent<Damageable>().InstaKill();
        }
    }

}
