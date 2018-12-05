using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMSArrived : MonoBehaviour {

    public int Message;
    public bool enter;
    public Sprite notification;

    public Image phoneIMG;
    public PhoneController phone;
    public GameObject redCircle;

    private void Start()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && phone.smsRecived == Message)        {

            if (phone.smsRecived == Message)
            {
                phone.smsRecived++;
                phoneIMG.sprite = notification;
                phone.MessageArrived();
                redCircle.SetActive(true);
                
            }
            else
                Debug.LogError("ERROR: Try to send a sms when already sent ir");
        }
    }
}
