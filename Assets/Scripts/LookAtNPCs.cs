using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtNPCs : MonoBehaviour {

    public Animator[] NPCs;
    public bool looked;

	// Use this for initialization
	void Start () {
        looked = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LookAt(bool param)
    {
        foreach (Animator anim in NPCs)
            anim.SetBool("Mirar", param);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!looked && collision.tag == "Player")
        {
            LookAt(true);
            looked = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LookAt(false);
            Destroy(gameObject);
        }
    }
}
