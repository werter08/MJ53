using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine;
using System.Collections;


public class MonologueManager : MonoBehaviour
{
    [Header("References")]
    public string mainMonologueText = "";
    public TypewriterCore monologueTypewriter;
    public CanvasGroup monologueCanvasGroup;
    public CanvasGroup secondaryHelperCanvas;

    [Header("Timing")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float waitAfterShow = 1.5f;
    [SerializeField] private float waitBeforeDestroy = 1f;

    private bool hasShownText = false;
    private bool isBusy = false;

    private void Update()
    {
        if (isBusy) return;

        if (Input.anyKeyDown)
        {
            if (!hasShownText)
            {
                isBusy = true;
                StartCoroutine(ShowMonologueSequence());
            }
            else
            {
                isBusy = true;
                StartCoroutine(FadeOutAndDestroy());
            }
        }
    }

    private IEnumerator ShowMonologueSequence()
    {
        // Fade out secondary helper first
        yield return FadeCanvasGroup(secondaryHelperCanvas, targetAlpha: 0f);

        // Show the text
        monologueTypewriter.ShowText(mainMonologueText);

        // Wait a moment so player can read beginning
        yield return new WaitForSeconds(waitAfterShow);

        hasShownText = true;
        isBusy = false;

        // Fade secondary helper back in (if you really need it)
        yield return FadeCanvasGroup(secondaryHelperCanvas, targetAlpha: 1f);
    }

    private IEnumerator FadeOutAndDestroy()
    {
        GameManager.Instance.ChangeState(GameState.firstDialogue);
        // Fade out main monologue canvas
        yield return FadeCanvasGroup(monologueCanvasGroup, targetAlpha: 0f);

        // Optional delay before destruction
        yield return new WaitForSeconds(waitBeforeDestroy);
        Destroy(gameObject);
    }

    // Reusable fade coroutine
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float duration = Mathf.Abs(startAlpha - targetAlpha) / fadeSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null; // better than fixed WaitForSeconds(0.01f)
        }

        // Ensure exact final value
        canvasGroup.alpha = targetAlpha;
    }

    // Optional: allow starting from code (e.g. trigger from another script)
    public void StartMonologue()
    {
        if (!isBusy && !hasShownText)
        {
            isBusy = true;
            StartCoroutine(ShowMonologueSequence());
        }
    }
}