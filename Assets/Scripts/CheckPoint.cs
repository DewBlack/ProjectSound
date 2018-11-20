using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public bool saved;

    // Use this for initialization
    void Start () {
        saved = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !saved)
        {
            saved = true;
            Debug.Log("Guardando");
            SaveLoad.SaveGameData();
            Debug.Log("Guardado Existoso");
        }
    }
}
