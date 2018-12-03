using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMSArrived : MonoBehaviour {

    public int Message;
    public bool enter;
    public Sprite notification;

    public Image phoneIMG;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PhoneController phone = GameObject.FindGameObjectWithTag("Phone").GetComponent<PhoneController>();

        if (collision.tag == "Player" && phone.smsRecived == Message)        {

            if (phone.smsRecived == Message)
            {
                phone.smsRecived++;
                phoneIMG.sprite = notification;
            }
            else
                Debug.LogError("ERROR: Try to send a sms when already sent ir");

            Destroy(gameObject);
        }
    }
}
