using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to NPCs that can be killed by the shotgun.
/// Call Kill() when hit by a shot. Plays death animation (if "Death" trigger exists) then deactivates.
/// </summary>
public class NpcKillable : MonoBehaviour
{
    [Tooltip("Optional: Animator trigger name for death. If empty or missing, NPC just deactivates after delay.")]
    public string deathTrigger = "Death";
    [Tooltip("Time before NPC is deactivated after death.")]
    public float hideAfterSeconds = 2f;

    private Animator _animator;
    private bool _isDead;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Kill()
    {
        if (_isDead) return;
        _isDead = true;

        if (_animator != null && !string.IsNullOrEmpty(deathTrigger))
        {
            _animator.SetTrigger(deathTrigger);
        }

        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0);
        gameObject.SetActive(false);
    }

    public bool IsDead => _isDead;
}
