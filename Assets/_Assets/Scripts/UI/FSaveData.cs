using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.IO;
using UnityEngine;


public class FSaveData
{
    public Vector3 Position;
    public int SceneIndex;
    public int iMaxJumps;
    public bool bDodge;
    public bool bUppercut;

    [System.NonSerialized] public const string FileName = "savedata.txt";
    [System.NonSerialized] public const string Path = "saves";

    public static string FullPath
    {
        get
        {
            char separator = System.IO.Path.DirectorySeparatorChar;

            string currentPath = Directory.GetCurrentDirectory();

            //Si el último caracter no es el separador de directorios, lo añadimos.
            if (currentPath[currentPath.Length - 1] != separator)
                currentPath += separator;

            string file = currentPath += FSaveData.Path + separator;

            //Si no existe la carpeta de guardado, la creamos
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }

            return file;

        }

    }
}