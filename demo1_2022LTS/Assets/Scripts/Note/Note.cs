using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note11 : MonoBehaviour
{
    public Vector2 direction;
    public float spawnDistance;
    public float moveDuration;
    public float TargetTime; 

    private Vector2 center = Vector2.zero; // �w�]�����e������


    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }
        if (AudioManager.Instance.BgmProgressDSP <= 0)
        {
            return;
        }

        double bgmElapsed = AudioManager.Instance.BgmProgressDSP;

        float t = Mathf.Clamp01(1f - ((float)bgmElapsed - (TargetTime - moveDuration)) / moveDuration);
        Debug.Log(t);

        float currentDistance = spawnDistance * t;

        Vector2 newPos = center + direction * currentDistance;
        transform.position = new Vector3(newPos.x, newPos.y, 0f);
        if (t <= 0f)
        {
            Destroy(gameObject);
            Camera.main.backgroundColor = GetRandomVividColor();
        }
    }

    private static Color GetRandomVividColor()
    {
        float h = Random.value;
        float s = Random.Range(0.6f, 1f);
        float v = Random.Range(0.7f, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}
