﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongIcon : MonoBehaviour {

    public GameObject Level;
    public int numsongs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Level.GetComponent<ChangeSong>().maxSongs = numsongs;
            gameObject.SetActive(false);
        }

    }
}
