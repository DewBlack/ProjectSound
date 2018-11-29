using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newSong : MonoBehaviour {

    public AudioSource song;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            song.Play();

        }
        InvokeRepeating("NextLevel", 3f, 0f);
    }
    private void NextLevel()
    {
        PantallaDeCarga.Instancia.CargarEscena(Escenas.Nivel2.ToString());
    }
}
