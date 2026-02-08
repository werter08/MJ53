using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;

public class Transactor : Singleton<Transactor>
{
    float fadeSpeed = 2f;
    public CanvasGroup canvasGroup;
    public TypewriterCore typo;



    public IEnumerator FadeOut()
    {
        yield return FadeCanvasGroup(canvasGroup, 0);
    }

    public IEnumerator FadeIn()
    {
        yield return  FadeCanvasGroup(canvasGroup, 1);
    }

    public void ShowText(string text)
    {
        typo.ShowText(text);
    }

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
}
