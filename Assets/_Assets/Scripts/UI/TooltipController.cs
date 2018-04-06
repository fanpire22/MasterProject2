using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour {

    //Singleton
    static System.Action OwnerCallback;
    private static bool bOpenTool;

    private static TooltipController _instance;
    public static TooltipController Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    [SerializeField] Text txtTitulo;
    [SerializeField] Text txtDescripcion;
    [SerializeField] Animator ani;

    private void Awake()
    {
        _instance = this;

        OwnerCallback.Invoke();
    }

    /// <summary>
    /// Función que abre una escena aditiva del resumen del PowerUp
    /// </summary>
    /// <returns>El estado de la escena</returns>
    public static bool LoadToolTip(System.Action Callback)
    {
        OwnerCallback = Callback;
        if (!bOpenTool)
        {
            SceneManager.LoadScene("PowerUp", LoadSceneMode.Additive);
            bOpenTool = true;
        }
        return bOpenTool;
    }

    /// <summary>
    /// inicialización serializada del texto del PowerUp
    /// </summary>
    /// <param name="Titulo">Título de la habilidad obtenida</param>
    /// <param name="Descripcion">Descripción de la habilidad obtenida</param>
    /// <param name="powerUp">Identificador dentro de la lista de PowerUps</param>
    public void InitializeToolTip(string Titulo, string Descripcion, ePowerUps powerUp)
    {

        txtTitulo.text = Titulo;
        txtDescripcion.text = Descripcion;
        ani.SetInteger("Skill", (int)powerUp);

    }

    /// <summary>
    /// Función que cierra una escena aditiva de tienda
    /// </summary>
    /// <returns></returns>
    public static bool unloadToolTip()
    {
        if (bOpenTool)
        {
            _instance.StartCoroutine(_instance.unloadToolTip_Corrutine());
            bOpenTool = false;
        }
        return bOpenTool;
    }

    public IEnumerator unloadToolTip_Corrutine()
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync("PowerUp");
        yield return op;
    }
}
