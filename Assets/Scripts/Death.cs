using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    public AudioSource Deathsound;
    public bool play;

	// Use this for initialization
	void Start () {
        Deathsound = GetComponent<AudioSource>();
        play = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!play)
            {
                Deathsound.Play();
                play = true;
            }
            else if(!Deathsound.isPlaying)
            {

                Debug.Log("Cargando");
                SaveLoad.LoadGameData();
                Debug.Log("Cargado");

            }
        
        }
    }
}
