using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScroll : MonoBehaviour {

    public float vel;
    
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.right * vel);
	}


}
