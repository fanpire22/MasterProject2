using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void NewGame()
    {
        SceneManager.LoadSceneAsync("Stage1");
        Knuckles.bRestoreLocation = true;
        Knuckles.RestoreLocation = new Vector3(-4.25f, -0.332f, -2); //Esta es la posición inicial en la fase 1 de Knuckles. 
        //Lo hacemos así para asegurarnos de que empiece con la habilidad inicial de esquiva
        Knuckles.bRestoreDodge = true;
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Continue()
    {
        
        string Json = File.ReadAllText(string.Format("{0}{1}", FSaveData.FullPath, FSaveData.FileName));
        FSaveData datos = JsonUtility.FromJson<FSaveData>(Json);

        if (datos!=null)
        {

            Knuckles.bRestoreLocation = true;
            Knuckles.RestoreLocation = datos.Position;
            Knuckles.bRestoreDodge = datos.bDodge;
            Knuckles.bRestoreUppercut = datos.bUppercut;
            Knuckles.iRestoreMaxJump = datos.iMaxJumps;

            SceneManager.LoadSceneAsync(datos.SceneIndex, LoadSceneMode.Single);
        }
        else
        {
            //Como no hay nada que cargar, empezamos nueva partida
            NewGame();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
