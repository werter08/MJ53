using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class onEnterTheWaterZoneCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " ENTER");

        if(other.CompareTag("Player"))
        {
            EventManager.Instance.ChangeWalkingPlace(WalkingSoundType.swimming);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.name + " EXIT");
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.ChangeWalkingPlace(WalkingSoundType.walking);

        }
    }
}
