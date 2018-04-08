using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseCharacter))]
public class Damageable : MonoBehaviour
{

    [SerializeField] private int _maxLife;
    private int _life;
    private BaseCharacter _chara;

    private void Awake()
    {
        _life = _maxLife;
        _chara = GetComponent<BaseCharacter>();

        //Mandamos esta orden para que si es Knuckles, se muestre su vida en pantalla
        _chara.updateLife(_life, false);
    }

    /// <summary>
    /// En este juego, se trabaja con "golpes", por lo que los puntos de vida sólo se quitan de uno en uno
    /// </summary>
    public bool GetDamage()
    {
        if (_chara.Invulnerable) return false;
        _life--;

        //Mandamos esta orden para que si es Knuckles, se muestre su vida en pantalla
        _chara.updateLife(_life, true);
        if (_life == 0)
        {
            _chara.OnDeath();
        }
        return true;
    }

    /// <summary>
    /// A no ser que toques los límites del juego, en cuyo caso, mueres automáticamente
    /// </summary>
    public void InstaKill()
    {
        _life = 0;

        //Mandamos esta orden para que si es Knuckles, se muestre su vida en pantalla
        _chara.updateLife(_life, true);
        _chara.OnDeath();
    }

    public void Heal(int amount)
    {
        _life = Mathf.Min(_maxLife, _life + amount);
        _chara.updateLife(_life, false);
    }
}
