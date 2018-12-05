using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThankForPlaying : MonoBehaviour {

    public TextMeshProUGUI text;
    public float seconds;
    public GameObject Cascos;
	// Use this for initialization
	void Start () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        InvokeRepeating("DOWN", 0, 1f);
    }
	
	// Update is called once per frame
	void Update () {
        Cascos.transform.Rotate(new Vector3(0, 1f, 0));
	}

    private void DOWN()
    {
        if (seconds <= 0)
            PantallaDeCarga.Instancia.CargarEscena(Escenas.Menu.ToString());
        else
        {
            seconds--;
            text.text = "0:0" + seconds.ToString();
        }
    }
}
