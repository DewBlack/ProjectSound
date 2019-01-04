using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;



public class DialogueManager : MonoBehaviour {
 
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    bool first = false;
    private Queue<string> sentences;
    private Queue<Reply> reply;
    public Button[] replyButtons;
    public Canvas canvas;
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
            DisplayNextSentece();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.name);
        canvas.gameObject.SetActive(true);
        nameText.text = dialogue.name;
        sentences.Clear();

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
        
        DisplayNextSentece();
        first = true;
    }

    public void DisplayNextSentece()
    {
        foreach (Button b in replyButtons)
        {
            b.interactable = true;
            b.gameObject.SetActive(false);
        }
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopCoroutine("TypeSentece");
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        bool vuton = false;
        if (sentence.Substring(0, "reply-".Length) == "reply-")
            vuton = true;
        dialogueText.text = "";

        if (vuton)
        {
            foreach (char letter in sentence.Substring("reply-".Length).ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
            CreateButtons();
        }
        else
        {
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
        }
    }

    public void CreateButtons()
    {
        if (reply.Count == 0)
            Debug.LogError("Se esta intentando acceder a una respuesta que no esta inicializada ni guardada en la Queue");
        Reply r = reply.Dequeue();
        for (int i = 0; i < r.replys.Length; i++)
        {
            // replyButtons[i].gameObject.SetActive(true);

            if (r.Bloqued[i])
                replyButtons[i].interactable = false;

            replyButtons[i].gameObject.SetActive(true);
        }
    }

    public void EndDialogue()
    {
        Debug.Log("End of Dialogue");
        canvas.gameObject.SetActive(false);
    }
}

