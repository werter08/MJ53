using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class woodCollectorUI : MonoBehaviour
{
    int currentWood = 0;
    public int maxWood = 5;
    public TextMeshProUGUI  woodText;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.woodCollected += woodCollected;
        woodText.text = "";
    }

    private void woodCollected()
    {
        currentWood++;
        woodText.text = $"{maxWood}/{currentWood}";
        if (currentWood >= maxWood)
        {
            QuestManager.Instance.ifQuestIsThisThenQuestDone(Quests.getWood);
            woodText.text = "";
        }
    }
}
