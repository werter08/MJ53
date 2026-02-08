using UnityEngine;

public class FireHelper : MonoBehaviour
{
    public Transform holdPoint;           // ← drag from player or pass in TryPickup
    public float followSmoothness = 1f;   // 1 = instant (no lag/shake)
    public bool isGun = false;
    public Light flameLight;
    public ParticleSystem flameParticles;

    public bool isHeld = false;
    public bool hasBeenFired = false;
    public Rigidbody rb;
    public Collider col;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if (!rb) Debug.LogError("No Rigidbody on " + gameObject.name);
        if (!col) Debug.LogError("No Collider on " + gameObject.name);

        if (flameLight) flameLight.enabled = false;
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

    public void TryPickup(Transform newHoldPoint, Transform holdPointForGun)
    {
        if (isHeld) return;
        if (newHoldPoint == null)
        {
            Debug.LogError("TryPickup: holdPoint is NULL!");
            return;
        }
        holdPoint = isGun ? holdPointForGun : newHoldPoint;
        isHeld = true;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        col.enabled = false;           // ← stops any physics fighting

        // Force snap immediately (kills initial shake)
        transform.SetParent(holdPoint);          // ← parenting is often more stable than lerp
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    AudioSystem.Instance.PlayGrabSound();
        QuestManager.Instance.ifQuestIsThisThenQuestDone(Quests.getLampe);

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
                if (hasBeenFired)
                {
                    deIgnitate();
                } else {
                    Ignite();
                }
            }
        }
    }

    public void Drop()
    {
        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
    }

    private void Ignite()
    {
        hasBeenFired = true;
        if (flameLight) flameLight.enabled = true;
        if (flameParticles) flameParticles.Play();
        QuestManager.Instance.ifQuestIsThisThenQuestDone(Quests.fireLampe);
    }
    
    private void deIgnitate()
    {
        hasBeenFired = false;
        if (flameLight) flameLight.enabled = false;
        if (flameParticles) flameParticles.Stop();
    }
}