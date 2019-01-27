using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    public AudioSource Deathsound;
    public bool play;
    public bool startHate;
    public Dialogue dialogue;
    private bool existDialogue;

    // Use this for initialization
    void Start() {

        existDialogue = GetComponent<ManagerLevel2>() != null;
        if (existDialogue)
        {
            dialogue = GetComponent<ManagerLevel2>().dialogue;
            Destroy(GetComponent<ManagerLevel2>());
        }
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
                else if (!Deathsound.isPlaying)
                {

                    Debug.Log("Cargando");
                    SaveLoad.LoadGameData();
                    if(existDialogue)
                        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
                    Debug.Log("Cargado");

                }
        }       
    }
}
