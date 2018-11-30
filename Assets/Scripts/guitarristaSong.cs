using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guitarristaSong : MonoBehaviour {

    public GameObject cave;
    public ChangeSong songs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && cave.activeSelf)
        {

            if (songs.maxSongs < 3)
            {
                songs.maxSongs++;
                songs.UptadeSong(songs.maxSongs-1);
                songs.UpdateLevel();
            }
        }            
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && cave.activeSelf)
            if(songs.maxSongs > 2) songs.maxSongs--;
    }
}
