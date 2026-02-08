using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMouseDetecter : MonoBehaviour
{
    private bool alreadyMade = false;
    public GameObject monologueManager;
    // Update is called once per frame
    void Update()
    {
        if (alreadyMade) return;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            alreadyMade = true;
            CameraController.Instance.useArrows = false;
            CameraController.Instance.sensitivity = 2;
            Destroy(gameObject, 0.3f);
            monologueManager.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.N))
        {
            alreadyMade = true;
            CameraController.Instance.useArrows = true;
            CameraController.Instance.sensitivity = 1;
            Destroy(gameObject, 0.3f);
            monologueManager.SetActive(true);
        }
        
    }
}
