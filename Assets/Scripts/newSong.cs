using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newSong : MonoBehaviour {

    public AudioSource song;
    public GameObject Icon;
    public GameObject HUD;
    public Escenas Next;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            song.Play();
            Icon.SetActive(false);
            HUD.SetActive(false);
            PantallaDeCarga.Instancia.CargarEscena(Next.ToString());
        }
    }
}
