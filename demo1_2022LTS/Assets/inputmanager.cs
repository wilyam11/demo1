using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;
    private int currentMouseSector = -1; // Stores the current sector (0-11)

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Ensure your camera is tagged 'MainCamera'.");
        }
    }

    void Update()
    {
        if (mainCamera == null) return;

        // --- Calculate Mouse Position & Sector ---
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        if (mouseWorldPos.magnitude > 0.01f)
        {
            float angleDeg = Mathf.Atan2(mouseWorldPos.y, mouseWorldPos.x) * Mathf.Rad2Deg;

            // --- Corrected Sector Calculation ---
            // Add 15 degrees to shift the boundary, then normalize to 0-360
            float shiftedAngleDeg = (angleDeg + 15f);

            // Normalize the shifted angle to be within 0-360 range
            // Using modulo (%) handles negative results correctly in C# for positive divisor
            float normalizedAngle = (shiftedAngleDeg % 360f + 360f) % 360f;

            // Now divide by 30 to get the track index
            currentMouseSector = Mathf.FloorToInt(normalizedAngle / 30f);
            // --- End Correction ---

            // Optional log:
            // Debug.Log($"Mouse Angle: {angleDeg:F1}, Shifted/Normalized: {normalizedAngle:F1}, Sector: {currentMouseSector}");
        }
        else
        {
            currentMouseSector = -1; // Center
        }

        // --- Detect Click ---
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Mouse Clicked! Current Sector: {currentMouseSector}");
        }
    }
}