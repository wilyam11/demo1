using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    public float MaxScale = 2.5f;
    public float MinScale = 1.5f;
    public float ShrinkDuration = 0.4f;


    private Coroutine scaleCoroutine;

    public void PlayScaleEffect()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleEffectCoroutine());
    }

    private IEnumerator ScaleEffectCoroutine()
    {
        transform.localScale = Vector3.one * MaxScale * 0.5f;

        float elapsed = 0f;
        while (elapsed < ShrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ShrinkDuration;
            float scale = Mathf.Lerp(MaxScale, MinScale, t);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.one * MinScale;
        scaleCoroutine = null;
    }
}
