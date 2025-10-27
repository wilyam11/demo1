using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DodecagonDrawer : MonoBehaviour
{
    [SerializeField] private int dodecagonSides = 12;
    [SerializeField] private float dodecagonRadius = 5f; // Size of the dodecagon
    [SerializeField] private float lineLength = 20f; // How far the radial lines extend

    private LineRenderer lineRenderer;
    private const float ANGLE_OFFSET_DEG = 15f; // Offset to align sides

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawShapes();
    }

    void DrawShapes()
    {
        if (lineRenderer == null) return;

        int pointCount = (dodecagonSides + 1) + (dodecagonSides * 2);
        lineRenderer.positionCount = pointCount;

        int currentPointIndex = 0;
        float angleOffsetRad = ANGLE_OFFSET_DEG * Mathf.Deg2Rad; // Convert offset to radians

        // --- Draw Dodecagon ---
        for (int i = 0; i <= dodecagonSides; i++)
        {
            // Add the offset here
            float angle = angleOffsetRad + (i * (360f / dodecagonSides) * Mathf.Deg2Rad);
            float x = Mathf.Cos(angle) * dodecagonRadius;
            float y = Mathf.Sin(angle) * dodecagonRadius;
            lineRenderer.SetPosition(currentPointIndex++, new Vector3(x, y, 0));
        }

        // --- Draw Radial Lines ---
        for (int i = 0; i < dodecagonSides; i++)
        {
             // Add the offset here too for consistency if needed,
             // though radial lines might look better without it. Test both ways.
            // For lines pointing TO the vertices:
            float vertexAngle = angleOffsetRad + (i * (360f / dodecagonSides) * Mathf.Deg2Rad);
            Vector3 direction = new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

            // Start point (center)
            lineRenderer.SetPosition(currentPointIndex++, Vector3.zero);
            // End point (far out)
            lineRenderer.SetPosition(currentPointIndex++, direction * lineLength);
        }

        // --- Configure Line Renderer Style (Optional) ---
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }
}