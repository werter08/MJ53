using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingCameraWobble : StaticInstance<SwimmingCameraWobble>
{
    [Header("Water Shake Settings")]
    public bool isSwimming = false;           // ← set this from your player controller
    public float intensity     = 0.07f;       // overall strength (0.03–0.15)
    public float frequency     = 1.8f;        // how fast it wobbles
    public float bobMultiplier = 1.4f;        // stronger vertical bob
    public float noiseSpeed    = 0.6f;        // slow Perlin drift

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private float timer = 0f;

    void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
    }

    void LateUpdate()   // ← LateUpdate prevents fighting other camera scripts
    {
        if (!isSwimming)
        {
            // Optional: smooth back to original position
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPos, Time.deltaTime * 5f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalLocalRot, Time.deltaTime * 5f);
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        // 1. Gentle constant sine-wave bob (like breathing / floating)
        float bobY   = Mathf.Sin(timer * frequency)          * intensity * bobMultiplier;
        float bobX   = Mathf.Cos(timer * frequency * 0.7f)   * intensity * 0.6f;

        // 2. Slow organic Perlin noise drift (feels like water currents)
        float noiseX = (Mathf.PerlinNoise(timer * noiseSpeed, 0)     - 0.5f) * 2f * intensity * 0.7f;
        float noiseY = (Mathf.PerlinNoise(0, timer * noiseSpeed * 1.3f) - 0.5f) * 2f * intensity * 0.9f;
        float noiseRoll = (Mathf.PerlinNoise(timer * noiseSpeed * 0.4f, 100) - 0.5f) * 2f * intensity * 4f;

        // Combine
        Vector3 offsetPos = new Vector3(bobX + noiseX, bobY + noiseY, 0);
        Quaternion offsetRot = Quaternion.Euler(noiseY * 8f, noiseX * 6f, noiseRoll);

        transform.localPosition = originalLocalPos + offsetPos;
        transform.localRotation = originalLocalRot * offsetRot;
    }

    // Call this from your swimming controller
    public void SetSwimming(bool swimming)
    {
        isSwimming = swimming;
    }
}
