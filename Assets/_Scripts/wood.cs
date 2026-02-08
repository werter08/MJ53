using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wood : MonoBehaviour
{
    public void TryPickup()
    {
        EventManager.Instance.WoodCollected();
        AudioSystem.Instance.PlayGrabSound();
        Destroy(gameObject, 0.2f);
    }
}
