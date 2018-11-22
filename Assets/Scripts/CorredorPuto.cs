using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorredorPuto : MonoBehaviour {

    public float vel;
	
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(transform.localScale.x < 0)
         transform.Translate(-transform.right * vel);
        else
            transform.Translate(transform.right * vel);

    }
}
