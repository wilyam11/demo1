using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float spawnDistance = 20f;
    public float moveDuration = 2.5f;
    public float TargetTime;
    public int trackIndex = -1; // Added: Track index (0-11)

    private Vector3 startPosition;
    private Vector3 targetPosition = Vector3.zero;
    private float timeElapsed = 0f;

    void Start()
    {
        startPosition = transform.position;

        // --- Added: Rotation Logic ---
        // Calculate angle from direction vector (pointing towards center)
        // Add 90 degrees because default sprite rotation (like a square) often points up/right
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        // --- End Added ---
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / moveDuration;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    // OnMouseDown is removed - InputManager will handle destruction now
    /*
    void OnMouseDown()
    {
        Debug.Log("Note Clicked!");
        Destroy(gameObject);
    }
    */

    // --- Added: Function for InputManager to call ---
    public void DestroyNote()
    {
        // Add any effects or scoring later
        Debug.Log($"Destroying note in track {trackIndex}");
        Destroy(gameObject);
    }
    // --- End Added ---
}