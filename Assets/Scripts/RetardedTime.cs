using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetardedTime : MonoBehaviour {

    public float timeToDie;
    public float time;
    public bool play;
    public AudioSource Deathsound;

    private void Start()
    {
        Deathsound = GetComponent<AudioSource>();
        play = false;
        time = 0;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(timeToDie > time)
            {
                time += Time.deltaTime;
            }
            else
            {
                if (!play)
                {
                    Deathsound.Play();
                    play = true;
                }
                else if (!Deathsound.isPlaying)
                {
                    Debug.Log("Cargando");
                    SaveLoad.LoadGameData();
                    Debug.Log("Cargado");
                }
            }
        }
    }
}
