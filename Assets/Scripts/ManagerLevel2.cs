using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLevel2 : MonoBehaviour {

    public GameObject[] Maps;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (!Maps[1].activeSelf)
            {
                Maps[0].SetActive(false);
                Maps[1].SetActive(true);
            }

        }
    }
}
