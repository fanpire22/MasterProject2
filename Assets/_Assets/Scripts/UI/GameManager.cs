using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static bool Pause = false;
    private Damageable _KnucklesLife;

    [SerializeField] Image[] _lifeImages;
    [SerializeField] Text _txtPoints;
    [SerializeField] Text _txtScore;
    [SerializeField] AudioClip _bgmVictory;
    [SerializeField] AudioClip _bgmDefeat;
    [SerializeField] AudioClip _sfxDAWAE;
    [SerializeField] AudioSource audio;
    [SerializeField] GameObject VictoryScreen;
    [SerializeField] GameObject DefeatScreen;
    [SerializeField] GameObject MainUI;

    public int Points;

    public void SetLifeShown(int life)
    {


        for (int i = 0; i < _lifeImages.Length; i++)
        {
            _lifeImages[i].enabled = (i <= life - 1);
        }
    }

    public void AddPoints(int points)
    {
        Points += points;
        _txtPoints.text = Points.ToString().PadLeft(4, '0');
        _txtScore.text = Points.ToString().PadLeft(4, '0');
    }

    public void OnDefeat()
    {
        Pause = true;
        MainUI.SetActive(false);
        DefeatScreen.SetActive(true);

        audio.Stop();
        audio.clip = _bgmDefeat;
        audio.loop = false;
        audio.Play();
        AudioSource.PlayClipAtPoint(_sfxDAWAE, new Vector2());
    }

    public void OnVictory()
    {
        if (_txtScore)
        {
            Pause = true;
            MainUI.SetActive(false);
            VictoryScreen.SetActive(true);

            audio.Stop();
            audio.clip = _bgmVictory;
            audio.Play();
        }
        else
        {
            //Estábamos en el tutorial, así que cargamos la primera pantalla
            VictoryScreen.SetActive(true);

            SceneManager.LoadSceneAsync("Stage1");
            Knuckles.bRestoreLocation = true;
            Knuckles.RestoreLocation = new Vector3(-4.25f, -0.332f, -2); //Esta es la posición inicial en la fase 1 de Knuckles. 
                                                                         //Lo hacemos así para asegurarnos de que empiece con la habilidad inicial de esquiva
            Knuckles.bRestoreDodge = true;
        }
    }
}
