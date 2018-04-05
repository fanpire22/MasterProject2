using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditosToPpal : MonoBehaviour
{

    [SerializeField] private AudioClip _sfxCICE;
    [SerializeField] private AudioClip _menuBGM;
    [SerializeField] private GameObject _titulo;
    [SerializeField] private GameObject _creditos;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audio.PlayOneShot(_sfxCICE);
    }

    private void Update()
    {
        if (!_audio.isPlaying)
        {
            _creditos.SetActive(false);
            _titulo.SetActive(true);
            _audio.clip = _menuBGM;
            _audio.loop = true;
            _audio.Play();

            gameObject.GetComponent<CreditosToPpal>().enabled=false;
        }
    }
}
