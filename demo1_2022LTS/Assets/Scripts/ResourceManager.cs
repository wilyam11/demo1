using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // 同步
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        T obj = Resources.Load<T>(path);
        if (obj == null)
        {
            Debug.LogWarning($"Resource not found : {path}");
        }
        return obj;
    }

    // 非同步
    public void LoadAsync<T>(string path, Action<T> onLoaded) where T : UnityEngine.Object
    {
        StartCoroutine(LoadResourceAsync(path, onLoaded));
    }

    private IEnumerator LoadResourceAsync<T>(string path, Action<T> onLoaded) where T : UnityEngine.Object
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
