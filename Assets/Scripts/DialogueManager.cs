using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using Invector.CharacterController;

public class DialogueManager : MonoBehaviour {
 
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    bool first = false;
    private Queue<string> sentences;
    private Queue<Reply> reply;
    public Button[] replyButtons;
    public TextMeshProUGUI[] replyText;
    public Canvas canvas;
    public vThirdPersonMotor player;
    public float speed;
    private Coroutine c;
    public bool auto;
    // Use this for initialization
    void Start()
    {
        canvas.gameObject.SetActive(false);
        sentences = new Queue<string>();
        reply = new Queue<Reply>();
        foreach (Button b in replyButtons)
        {
            b.onClick.AddListener(DisplayNextSentece);
            b.interactable = true;
            b.gameObject.SetActive(false);
        }
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(1) && nameText.text != "" && first && !replyButtons[0].gameObject.activeSelf)
        {
            PressedButton();
        }
    }

    public void PressedButton()
    {
        StopCoroutine(c);
        DisplayNextSentece();
    }

    public void StartDialogue(Dialogue dialogue, bool ao)
    {
        canvas.gameObject.SetActive(true);
        sentences.Clear();
        auto = ao;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        if (dialogue.reply.Length != 0)
        {
            reply.Clear();

            foreach (Reply answer in dialogue.reply)
            {
                reply.Enqueue(answer);
            }
        }
        first = false;
        DisplayNextSentece();
    }

    public void DisplayNextSentece()
    {
        foreach (Button b in replyButtons)        
            b.gameObject.SetActive(false);
        
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        Debug.Log(sentence.IndexOf("\n") + 1);
        nameText.text = sentence.Substring(0, sentence.IndexOf("\n"));
        c = StartCoroutine(TypeSentence(sentence.Substring(sentence.IndexOf("\n") + 1)));
        first = true;
    }
    IEnumerator TypeSentence(string sentence)
    {
        int aux = sentence.IndexOf("-") + 1;
        dialogueText.text = "";
        if (aux > 1 && sentence.Substring(0, aux) == "reply-")
        {
            foreach (char letter in sentence.Substring(aux).ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(speed);
            }
            CreateButtons();
        }
        else
        {
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(speed);
            }
        }
        if (auto)
        {
            PressedButton();
        }
    }

    public void CreateButtons()
    {
        if (reply.Count == 0)
            Debug.LogError("Se esta intentando acceder a una respuesta que no esta inicializada ni guardada en la Queue");

        Reply r = reply.Dequeue();

        for (int i = 0; i < r.replys.Length; i++)
        {            
            replyButtons[i].interactable = !r.Bloqued[i];
            replyText[i].text = r.replys[i];
            replyButtons[i].gameObject.SetActive(true);
        }
    }

    public void EndDialogue()
    {
        sentences.Clear();
        reply.Clear();

        player.gameObject.GetComponent<Invector.CharacterController.vThirdPersonInput>().enabled = true;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        canvas.gameObject.SetActive(false);
    }
}

