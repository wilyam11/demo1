using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager
{
    // ¦P¨B
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        T obj = Resources.Load<T>(path);
        if (obj == null)
        {
            Debug.LogWarning($"Resource not found : {path}");
        }
        return obj;
    }

    private static IEnumerator LoadResourceAsync<T>(string path, Action<T> onLoaded) where T : UnityEngine.Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(path);
        yield return request;
        if (request.asset == null)
        {
            Debug.LogWarning($"Resource not found : {path}");
        }
        onLoaded?.Invoke(request.asset as T);
    }
}
