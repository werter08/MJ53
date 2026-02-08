using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;

public class QuestManager : StaticInstance<QuestManager>
{
    public List<QuestStruct> quests = new List<QuestStruct>();
    
    Dictionary<Quests, QuestStruct> questDictionary = new Dictionary<Quests, QuestStruct>();
    QuestStruct currentQuest = null;

    public TypewriterCore title;
    public TypewriterCore description;
    public GameObject panel;

    public void Start()
    {
        panel.SetActive(false);
        
        foreach (QuestStruct quest in quests)
        {
            questDictionary[quest.guest] = quest;
        }
        
        setQuest(Quests.getLampe);
    }


    public void setQuest(Quests quest)
    {
        currentQuest = questDictionary[quest];
        ConfigureQuest();
    }

    public void ifQuestIsThisThenQuestDone(Quests quest)
    {
        if (currentQuest.guest == quest)
        {
            QuestDone();
        }
    }


    private void QuestDone()
    {
        AudioSystem.Instance.PlaySuccessSound();
        if (currentQuest.guest == Quests.getLampe)
        {
            setQuest(Quests.fireLampe);
        } else if (currentQuest.guest == Quests.fireLampe)
        {
            setQuest(Quests.getWood);
        } else if (currentQuest.guest == Quests.getWood)
        {
            setQuest(Quests.getToBase);
        }
        else
        {
            disappear();
        }
        
    }
    
    private void ConfigureQuest()
    {
        panel.SetActive(true);
        title.ShowText(currentQuest.name);
        description.ShowText(currentQuest.description);
    }

    private void disappear()
    {
        currentQuest = null;
        panel.SetActive(false);
    }
}

[System.Serializable]
public class QuestStruct
{
    public Quests guest;
    public string name;
    public string description;
}

[System.Serializable]
public enum Quests
{ 
    getLampe, fireLampe, getWood, getToBase, getAllOfThem
}