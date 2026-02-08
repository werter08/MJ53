using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class onEnterTheSafeZoneCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Player"))
        {
            if (QuestManager.Instance.currentQuest.guest == Quests.getToBase) {
                QuestManager.Instance.ifQuestIsThisThenQuestDone(Quests.getToBase);
                GameManager.Instance.ChangeState(GameState.truthOrDare);
            }
            EventManager.Instance.StopWind();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.PlayWind();
        }
    }
}
