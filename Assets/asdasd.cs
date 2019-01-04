using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asdasd : MonoBehaviour {
    bool start = false;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !start)
        {
            start = true;
            GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }
    
}
