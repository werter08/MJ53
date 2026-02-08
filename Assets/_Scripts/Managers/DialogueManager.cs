using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using ColorUtility = UnityEngine.ColorUtility;

public class DialogueManager : StaticInstance<DialogueManager>
{
    public CanvasGroup panel;
    public TextMeshProUGUI name;
    public TypewriterCore text;
    public TypewriterCore nameCore;
    
    public List<Dialogue> dialogues;
    private Dictionary<int, Dialogue> dictionary = new Dictionary<int, Dialogue>();
    private bool canContinue = false;

    private Coroutine coroutine;
    private void Awake()
    {
        base.Awake();
        foreach (Dialogue dialogue in dialogues)
        {
            dictionary[dialogue.id] = dialogue;
        }
    }

    public void PlayDialogue(int id, Action end)
    {
        canvasOpen();
        Dialogue dialogue = dictionary[id];
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        
        coroutine = StartCoroutine(StartDialogue(dialogue, end));
    }


    IEnumerator StartDialogue(Dialogue dialogue, Action end)
    {
        canvasOpen();
        foreach (DialogueLine line in dialogue.lines)
        {
            configurePanel(line);
            
            yield return new WaitUntil(() => canContinue);


            canContinue = false;
            yield return new WaitForSeconds(line.waitTime);
        }
        canvasClose();
        end.Invoke();
    }

    void configurePanel(DialogueLine dialogue)
    {
        string hex = ColorUtility.ToHtmlStringRGB(dialogue.color);
        string s = $"<color=#{hex}>{dialogue.title}</color>";
        nameCore.ShowText(s); 

        text.ShowText(dialogue.desc);
        text.onTextShowed.AddListener(delegate { ContinueDialogue(); });
        canContinue = false;
    }

    void canvasOpen()
    {
        panel.alpha = 1f;
    }
    void canvasClose()
    {
        panel.alpha = 0f;
    }
    
    void ContinueDialogue()
    {
        canContinue= true;
        text.onTextShowed.RemoveListener(delegate { ContinueDialogue(); });

    }
}


[System.Serializable]
public class Dialogue
{
    public int id;
    public List<DialogueLine>  lines;
}

[System.Serializable]
public class DialogueLine
{
     public string title;

     public string desc;
     
     public Color color;
     public float waitTime = 0.8f;
}

