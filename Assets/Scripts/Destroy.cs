using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("DestroyIT", 5f, 0);
	}
	public void DestroyIT()
    {
        Destroy(gameObject); 
    }
}
