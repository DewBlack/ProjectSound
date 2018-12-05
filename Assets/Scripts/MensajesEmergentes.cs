using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MensajesEmergentes : MonoBehaviour {

    public float vel;    

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
            StartCoroutine(Fumine(vel));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopCoroutine(Fumine(vel));
        StartCoroutine(Difumine(vel));
    }

    private IEnumerator Difumine(float vel)
    {
        Color c = GetComponent<SpriteRenderer>().color;
        while (c.a > 0)
        {
            c.a -= vel;
            GetComponent<SpriteRenderer>().color = c;
            yield return null;
        }
        Destroy(gameObject);
    }
    private IEnumerator Fumine(float vel)
    {
        Color c = GetComponent<SpriteRenderer>().color;
        while (c.a < 1)
        {
            c.a += vel;
            GetComponent<SpriteRenderer>().color = c;
            yield return null;
        }
    }
}
