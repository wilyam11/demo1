using System;
using System.Collections;
using UnityEngine;


public class DetectLine : MonoBehaviour
{
    public void SetRadius(float radius)
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.localPosition = new Vector3(radius, 0f, 0f);
        }
    }

    public void SetLength(float length)
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.localScale = new Vector3(length, child.localScale.y, child.localScale.z);
        }
    }

    public void SetColor(Color color)
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    public IEnumerator Grow(Vector2 scale, float duration)
    {
        var _transform = transform.GetChild(0);
        Vector3 _original = _transform.localScale;
        _transform.localScale = _original * scale;

        float _elapsed = 0f;
        while (_elapsed < duration)
        {
            _elapsed += Time.deltaTime;
            _transform.localScale = Vector3.Lerp(_original * scale, _original, _elapsed / duration);
            yield return null;
        }
        _transform.localScale = _original;
    }
}
