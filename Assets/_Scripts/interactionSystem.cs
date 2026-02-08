using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class interactionSystem : StaticInstance<interactionSystem>
{
    [Header("Interaction")]
    public Transform holdpointTransform;           // your hand hold point
    public Transform holdpointForGun;           // your hand hold point
    public float interactionRange = 3f;
    public bool canIntereact = false;
    
    private FireHelper currentPickupTarget;        // what we can pickup right now
    private Highlight    currentHighlighted;       // what is visually glowing right now
    private wood    currentWood;     

    private void Update()
    {
        if(!canIntereact)  return;
        // ── Always do raycast for highlight ───────────────────────────────
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Highlight newHighlightTarget = null;
        currentPickupTarget = null;  // clear until we hit something grabbable
        currentWood = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            // Check for pickup (FireHelper) – gun can be picked up too (!hasBeenFired is for lamp)
            FireHelper item = hit.collider.GetComponent<FireHelper>();
            if (item != null && !item.isHeld && (!item.hasBeenFired || item.isGun))
            {
                currentPickupTarget = item;
            }

            // Check for highlight (any object with Highlight component)
            Highlight hl = hit.collider.GetComponent<Highlight>();
            if (hl != null)
            {
                newHighlightTarget = hl;
            }
            
            wood w = hit.collider.GetComponent<wood>();
            if (w != null)
            {
                currentWood = w;
            }
        }

        // ── Update highlight ──────────────────────────────────────────────
        if (newHighlightTarget != currentHighlighted)
        {
            if (currentHighlighted != null)
            {
                currentHighlighted.ToggleHighlight(false);
            }

            if (newHighlightTarget != null)
            {
                newHighlightTarget.ToggleHighlight(true);
            }

            currentHighlighted = newHighlightTarget;
        }

        // ── Handle pickup input only when looking at valid item ──────────
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentPickupTarget != null)
            {
                currentPickupTarget.TryPickup(holdpointTransform, holdpointForGun);
                currentPickupTarget = null;

                // Optional: de-highlight immediately after pickup
                if (currentHighlighted != null)
                {
                    currentHighlighted.ToggleHighlight(false);
                    currentHighlighted = null;
                }
            }

            if (currentWood != null)
            {
                currentWood.TryPickup();
                currentWood = null;
            }
        }
        

        // Optional: show UI prompt only when looking at something grabbable
        // if (currentPickupTarget != null) → show "Press E to take"
    }
}