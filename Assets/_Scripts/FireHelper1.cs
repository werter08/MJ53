using System.Collections;
using UnityEngine;

public class Gun : FireHelper
{
    private bool Fireing = false;

    private void Awake()
    {
        base.Awake();
        if (flameParticles) flameParticles.Stop();

        Debug.Log($"FireHelper ready | Trigger collider: {col.isTrigger} | Kinematic: {rb.isKinematic}");
    }

    private void LateUpdate()
    {
        if (isHeld && holdPoint != null)
        {
            // Instant snap first frame + smooth after
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;

            // Or smooth if you prefer (but instant usually fixes shake best):
            // transform.position = Vector3.Lerp(transform.position, holdPoint.position, followSmoothness);
            // transform.rotation = Quaternion.Slerp(transform.rotation, holdPoint.rotation, followSmoothness);
        }
    }

    private void Update()
    {
        if (isHeld)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Drop();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if (!Fireing) {
                    Fire();
                }
            }
        }
    }
    

    private void Fire()
    {
        Fireing = true;
        if (flameLight) flameLight.enabled = true;
        if (flameParticles) flameParticles.Play();
        StartCoroutine(isFireing());
    }

    private IEnumerator isFireing()
    {
        yield return new WaitForSeconds(1f);
        Fireing = false;
    }

}