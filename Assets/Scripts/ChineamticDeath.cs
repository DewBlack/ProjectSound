using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChineamticDeath : MonoBehaviour {
    
    public GameObject Corredores;
    public Image brillo;
    public float degradar;
    public Transform[] spawn;
    public Transform[] corredores;

    private vThirdPersonController controller;
    private vThirdPersonInput input;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Oscurecer()
    {
        var tempColor = brillo.color;
        Debug.Log(tempColor.a);
        if (tempColor.a >= 1)
        {
            corredores[0].transform.position = spawn[0].transform.position;
            corredores[1].transform.position = spawn[1].transform.position;
            tempColor.a = 0;
            brillo.color = tempColor;

            input.enabled = true;
            controller.enabled = true;
            Corredores.SetActive(false);

            CancelInvoke("Oscurecer");
            SaveLoad.LoadGameData();
        }
        else
        {
            tempColor.a = tempColor.a + degradar;
            brillo.color = tempColor;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            controller = collision.GetComponent<Invector.CharacterController.vThirdPersonController>(); // No podra moverse 
            input = collision.GetComponent<Invector.CharacterController.vThirdPersonInput>(); // No podra moverse 

            controller.enabled = false;
            controller.input = Vector2.zero;
            input.enabled = false;

            Corredores.SetActive(true);
            InvokeRepeating("Oscurecer", 0, 0.2f);
        }
    }
}
