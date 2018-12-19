using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {

    public Vector3 Start;
    public Vector3 End;
    public GameObject player;
    public Vector3 direction;
    public Transform child;
    public float distance;
    private bool isMoving;

    public void Awake()
    {
        distance = 1.5f;
        Start = transform.position;
        End = child.position;
    }

    public Vector3 diference()
    {
        return End - Start;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            isMoving = true;
        else
            isMoving = false;
    }

    public void Move(Vector3 pos)
    {
        Vector3 a = Start + pos;

        if (player != null && a != End && a != Start)
        {
            var aux = Start + pos + Vector3.up * 2.55f;
            
            if (isMoving)
                player.transform.position = new Vector3(player.transform.position.x, aux.y, player.transform.position.z);
            else
                player.transform.position = new Vector3(aux.x + distance, aux.y, player.transform.position.z);
        }
        transform.position = Start + pos;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            player = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            distance = player.transform.position.x - transform.position.x;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player != null)
            distance = player.transform.position.x - transform.position.x;

    }

}
