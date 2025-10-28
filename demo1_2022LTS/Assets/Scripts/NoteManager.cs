
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;

    public void Start()
    {

    }

    public void SpawnNotes(NoteData[] notes, int speed)
    {
        float spawnRadius = 10f;

        foreach (var noteData in notes)
        {
            float angleRad = noteData.x*8 * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector3 spawnPosition = Vector2.zero + dir * spawnRadius;

            GameObject noteObj = Instantiate(notePrefab, spawnPosition, Quaternion.identity, this.transform);

            var noteScript = noteObj.GetComponent<Note1>();
            if (noteScript != null)
            {
                noteScript.direction = dir;
                noteScript.spawnDistance = spawnRadius;
                noteScript.moveDuration = 2.5f;
                noteScript.TargetTime = noteData.x + 0.6f;
            }
        }
    }
}
