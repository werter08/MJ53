using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : StaticInstance<EventManager> {
    

    public static event Action playWInd;
    public static event Action stopWind;

    
    public static event Action<WalkingSoundType> onChangeWalkingPlace;


    public void ChangeWalkingPlace(WalkingSoundType state)
    {
        onChangeWalkingPlace?.Invoke(state);
    }

    public void PlayWind()
    {
        playWInd?.Invoke();
    }

   
    public void StopWind()
    {
        stopWind?.Invoke();
    }

}


public enum TimeState
{
    usual, 
    persent50,
    stoped,
}

