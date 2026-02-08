using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;

public class ChooseManager : MonoBehaviour
{
    public TypewriterCore mainText;
    public CanvasGroup canvas;
    
    public List<Choose> choices = new List<Choose>();
    
    private Dictionary<int, Choose> Dictionary = new Dictionary<int, Choose>();
    private bool isShowen = false;
    private Action<bool, int> res;
    private Choose currentChoose;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach(Choose choice in choices) 
        {
            Dictionary[choice.id] = choice;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowen) {return;}

        if (Input.GetKeyDown(KeyCode.Y))
        {
            getChoose(true);
        } else if (Input.GetKeyDown(KeyCode.N))
        {
            getChoose(false);
        }
    }

    public void SetChoose(int id, Action<bool, int> close)
    {
        isShowen = true;
        res = close;
        currentChoose = choices[id];
        canvas.alpha = 1;
        mainText.ShowText(Dictionary[id].name);
    }

    private void getChoose(bool yes)
    {
        isShowen = false;

        if (currentChoose.dialogueIdOnYes == 0 && currentChoose.dialogueIdOnNo == 0)
        {
            res(yes, 0);
        }
        else
        {
            GameManager.Instance.DialogueManager.PlayDialogue(
                yes ? currentChoose.dialogueIdOnYes : currentChoose.dialogueIdOnNo,
                () => { res(yes, yes ? currentChoose.dialogueIdOnYes : currentChoose.dialogueIdOnNo); });
        }

        canvas.alpha = 0;
    }
}

[System.Serializable]
public class Choose
{
    public int id; 
    public string name;
    public int dialogueIdOnYes;
    public int dialogueIdOnNo;
}