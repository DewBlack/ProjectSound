using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialouge;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialouge);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Invector.CharacterController.vThirdPersonMotor>().stoped = true;
            collision.gameObject.GetComponent<Invector.CharacterController.vThirdPersonMotor>().input = Vector2.zero;
            TriggerDialogue();
        }
    }
}
