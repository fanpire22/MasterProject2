using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

//Guardamos en un archivo de texto dónde nos encontramos, etc
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Checkpoint : MonoBehaviour
{
    bool isActive = false;
    [SerializeField] AudioClip _sfxActivated;
    [SerializeField] AudioClip _bgmChange;
    [SerializeField] AudioSource _bgmSource;

    Animator _ani;
    private void Awake()
    {
        _ani = GetComponent<Animator>();
        _ani.SetTrigger("Reset");
    }

    public void Reset()
    {
        isActive = false;
        _ani.SetTrigger("Reset");
    }

    /// <summary>
    /// Recogemos los datos del jugador y los guardamos en el fichero de guardado
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Knuckles") || isActive) return;
        //Es un checkpoint válido. Reseteo todos los Chekpoints y lo habilito

        Knuckles knux = collision.GetComponent<Knuckles>();
        isActive = true;
        _ani.SetTrigger("Activate");

        if(_bgmChange && _bgmSource)
        {
            _bgmSource.clip = _bgmChange;
            _bgmSource.Play();
        }

        AudioSource.PlayClipAtPoint(_sfxActivated, transform.position);

        FSaveData data = new FSaveData();
        data.Position = collision.transform.position;
        data.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        data.bDodge = knux.bDodge;
        data.bUppercut = knux.bUppercut;
        data.iMaxJumps = knux.iMaxJump;

        string s = JsonUtility.ToJson(data);

        Save(s);

    }

    /// <summary>
    /// Guardamos los datos generados por el punto de guardado
    /// </summary>
    /// <param name="Json"></param>
    private void Save(string Json)
    {

        File.WriteAllText(string.Format("{0}{1}", FSaveData.FullPath, FSaveData.FileName), Json);

    }
}
