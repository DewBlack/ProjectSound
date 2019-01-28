using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialouge;
    public bool trigger;
    public Follow t;
    public bool auto;
   
    public void TriggerDialogue()
    {
        FindObjectOfType<PhoneController>().FixedHeadphones();
        FindObjectOfType<DialogueManager>().StartDialogue(dialouge, auto);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(trigger && collision.tag == "Player")
        {
            FindObjectOfType<DialogueManager>().EndDialogue();
            trigger = false;

            if (t)
                t.Enableded();
            else
                Debug.Log("XD");
            collision.gameObject.GetComponent<Invector.CharacterController.vThirdPersonInput>().enabled = false;
            collision.gameObject.GetComponent<Invector.CharacterController.vThirdPersonController>().input = Vector2.zero;
            collision.gameObject.GetComponent<Invector.CharacterController.vThirdPersonController>()._rigidbody2D.velocity = Vector2.zero;
            collision.gameObject.GetComponent<Invector.CharacterController.vThirdPersonController>().StopAnimations();

            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            TriggerDialogue();
        }
    }
    
}
