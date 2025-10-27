using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float spawnRadius = 20f; // Changed from XOffset to Radius
    [SerializeField] private float noteSpeedDuration = 3.0f;

    private float timer;
    // Removed Camera reference as we don't need it for this logic

    void Start()
    {
        if (notePrefab == null)
        {
            Debug.LogError("Note Prefab not assigned in NoteSpawner!");
        }
        // Spawn immediately at start for testing (optional)
        // SpawnNoteAtRandomAngle();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && notePrefab != null)
        {
            timer = 0f;
            SpawnNoteAtRandomAngle(); // Changed function call
        }
    }

    // Renamed and modified function
    void SpawnNoteAtRandomAngle()
    {
        // 1. Pick a random side index (0 to 11)
        int randomSideIndex = Random.Range(0, 12);

        // 2. Calculate the angle perpendicular to that side
        // With the 15-degree offset, the perpendicular angles are multiples of 30 degrees.
        float spawnAngleDeg = randomSideIndex * 30f;
        float spawnAngleRad = spawnAngleDeg * Mathf.Deg2Rad;

        // 3. Calculate the spawn direction and position
        Vector2 spawnDirection = new Vector2(Mathf.Cos(spawnAngleRad), Mathf.Sin(spawnAngleRad));
        Vector3 spawnPosition = (Vector3)(spawnDirection * spawnRadius); // Spawn far out

        // 4. Instantiate the note
        GameObject noteObj = Instantiate(notePrefab, spawnPosition, Quaternion.identity);

        // 5. Configure the note's script
        Note noteScript = noteObj.GetComponent<Note>();
        if (noteScript != null)
        {
            // Direction should be TOWARDS the center (opposite of spawn direction)
            noteScript.direction = -spawnDirection;
            noteScript.spawnDistance = spawnRadius;
            noteScript.moveDuration = noteSpeedDuration;
            // TargetTime calculation needs real game logic, placeholder:
            noteScript.TargetTime = Time.time + noteSpeedDuration;
        }
        else
        {
            Debug.LogError("Spawned Note Prefab is missing the 'Note' script!");
        }
    }
}