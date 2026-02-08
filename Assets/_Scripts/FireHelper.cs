using UnityEngine;

public class FireHelper : MonoBehaviour
{
    public Transform holdPoint;           // ← drag from player or pass in TryPickup
    public float followSmoothness = 1f;   // 1 = instant (no lag/shake)
    public bool isGun = false;
    public Light flameLight;
    public ParticleSystem flameParticles;
    [Header("Gun shooting")]
    public ParticleSystem shootParticles; // Muzzle flash / smoke – assign in inspector
    public float shootRange = 50f;
    public float shootCooldown = 0.6f;   // Prevents double-firing
    private float _lastShootTime = -999f;

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
        bool isGunItem = isGun || gameObject.name.IndexOf("gun", System.StringComparison.OrdinalIgnoreCase) >= 0;
        // Gun uses holdPointForGun; fallback to main hold point if not set
        holdPoint = isGunItem ? (holdPointForGun != null ? holdPointForGun : newHoldPoint) : newHoldPoint;
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
        if (isGunItem && QuestManager.Instance != null)
            QuestManager.Instance.setQuest(Quests.getAllOfThem);
        else if (!isGunItem && QuestManager.Instance != null)
            QuestManager.Instance.ifQuestIsThisThenQuestDone(Quests.getLampe);
    }

    private void Update()
    {
        if (!isHeld) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            bool treatAsGun = isGun || gameObject.name.IndexOf("gun", System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (treatAsGun)
            {
                if (Time.time - _lastShootTime < shootCooldown)
                    return;
                _lastShootTime = Time.time;
                if (GameManager.Instance != null && GameManager.Instance.AllNpcsDead())
                    GameManager.Instance.PlayerSuicide();
                else
                    Shoot();
            }
            else
            {
                if (hasBeenFired)
                    deIgnitate();
                else
                    Ignite();
            }
        }
    }

    private void Shoot()
    {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.PlayShotgunSound();
        if (shootParticles != null)
            shootParticles.Play();

        Camera cam = Camera.main;
        if (cam == null) return;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            NpcKillable npc = hit.collider.GetComponentInParent<NpcKillable>();
            if (npc != null)
                npc.Kill();
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