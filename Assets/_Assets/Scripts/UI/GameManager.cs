using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private Damageable _KnucklesLife;

    [SerializeField] Image[] _lifeImages;
    [SerializeField] Text _txtPoints;
    
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
        _txtPoints.text = Points.ToString().PadLeft(4,'0');
    }
}
