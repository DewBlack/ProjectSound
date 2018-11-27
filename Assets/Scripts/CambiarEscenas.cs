using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Escenas
{
    Menu,
    Nivel1,
    Nivel2,
    Nivel3,
    Nivel4
}

public class CambiarEscenas : MonoBehaviour {

    public Escenas cargarEscena;

    public void OnPointerClick()
    {
        PantallaDeCarga.Instancia.CargarEscena(cargarEscena.ToString());        
    }

    public void SetCargarEscena(Escenas naw) { cargarEscena = naw; }
    public Escenas GetCargarEscena() { return cargarEscena; }
    
}
