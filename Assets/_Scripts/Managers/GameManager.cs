using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;


public class GameManager : StaticInstance<GameManager> {
    // public static event Action<GameState> OnBeforeStateChanged;
    // public static event Action<GameState> OnAfterStateChanged;
    public float timeChangeForce = 1;

    public GameState State { get; private set; }

    void Start()
    {
        // EventManager.onChangeTimeState += onChangeTimeState;
        // EventManager.onQuestOpen += questOpen;
        // EventManager.onQuestClose += questClose;
       
        StartGame();
    }

   
    public void StartGame()
    {
        ChangeState(GameState.Starting);
    }
    
    public void ChangeState(GameState newState) {

        State = newState;
        switch (newState) {
          case GameState.Starting: 
          {
            HandleStarting();
            break;
          }
        }
    }

    async private void HandleStarting()
    {
        await Task.Delay(4000);
    }
    }

[Serializable]
public enum GameState {
    Starting = 0
}