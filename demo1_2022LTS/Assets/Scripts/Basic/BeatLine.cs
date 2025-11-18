using UnityEngine;

public class BeatLine : MonoBehaviour
{
    public Vector2 UnitDirectionVector;
    public float StartPointRadius;
    public float TargetPointRadius;
    public float Bpm;

    private float moveTime;
    private float startTime;
    private int cout = 0;

    void Start()
    {
        // 一拍移動一次
        moveTime = 8f * 5f / 14f;
        startTime = Time.time;
    }

    void Update()
    {
        float t = (Time.time - startTime) / moveTime;
        t = Mathf.Clamp01(t);

        float curRadius = Mathf.Lerp(StartPointRadius, TargetPointRadius, t);
        Vector2 pos = UnitDirectionVector * curRadius;
        transform.position = new Vector3(pos.x, pos.y, 0f);

        if(cout % 8 == 0)
        {
            transform.localScale = new Vector3(0.05f, transform.localScale.y  - 0.1f);
        }
        cout++;

        // 到達中心後自動消失
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
