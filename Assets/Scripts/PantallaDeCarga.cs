using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaDeCarga : MonoBehaviour {

    public static PantallaDeCarga Instancia { get; private set; }

    public Image imagenDeCarga;
    [Range(0.01f, 0.1f)]
    public float velocidadAparecer = 0.05f;
    [Range(0.01f, 0.1f)]
    public float velocidadOcultar = 0.5f;

    public Image rotacionCascos;
    [Range(5, 20)]
    public float rotacion = float.Epsilon;


    // Use this for initialization
    private void Awake()
    {
        DefinirSingleton();
    }

    private void DefinirSingleton()
    {
        if(Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(this);
            imagenDeCarga.gameObject.SetActive(false);
            rotacionCascos.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CargarEscena(string nombreEscena)
    {
        StartCoroutine(MostrarPantallaDeCarga(nombreEscena));
        StartCoroutine(GirarCascos());
    }

    public void CargarEscena(int numeroEscena)
    {        
        StartCoroutine(MostrarPantallaDeCarga(SceneManager.GetSceneAt(numeroEscena).ToString()));
        StartCoroutine(GirarCascos());
    }

    private IEnumerator MostrarPantallaDeCarga(string nombreEscena)
    {
        imagenDeCarga.gameObject.SetActive(true);
        rotacionCascos.gameObject.SetActive(true);

        Color c = imagenDeCarga.color;
        Color c2 = rotacionCascos.color;
        c.a = 0.0f;
        c2.a = 0.0f;
        while(c.a < 1)
        {
            imagenDeCarga.color = c;
            rotacionCascos.color = c2;
            c.a += velocidadAparecer;
            c2.a += velocidadAparecer;
            yield return null;
        }
        
        SceneManager.LoadScene(nombreEscena);

        while(!nombreEscena.Equals(SceneManager.GetActiveScene().name))
        {
            yield return null;
        }

        while (c.a > 0)
        {
            imagenDeCarga.color = c;
            rotacionCascos.color = c2;
            c.a -= velocidadOcultar;
            c2.a -= velocidadOcultar;
            yield return null;
        }

        imagenDeCarga.gameObject.SetActive(false);
        rotacionCascos.gameObject.SetActive(false);
        StopCoroutine("GirarCascos");
    }

    private IEnumerator GirarCascos()
    {
        while (true)
        {
            rotacion = 5;
            rotacionCascos.transform.rotation = Quaternion.Euler(0, rotacionCascos.transform.rotation.eulerAngles.y + rotacion, 0);
            yield return null;
        }
    }
   
}
