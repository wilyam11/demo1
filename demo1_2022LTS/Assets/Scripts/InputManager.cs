using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public KeyCode[] HitKey = new KeyCode[4];


    private bool[] hitKeyList = new bool[4] { false, false, false, false};

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        for (int i = 0; i < HitKey.Length; i++)
        {
            if (Input.GetKeyDown(HitKey[i]))
            {
                hitKeyList[i] = true;
            }
            else
            {
                hitKeyList[i] = false;
            }
        }
    }

    public KeyCode[] GetCurrentHitKey()
    {
        List<KeyCode> _result = new List<KeyCode>();

        for (int i = 0; i < hitKeyList.Length; i++)
        {
            if (hitKeyList[i])
            {
                _result.Add(HitKey[i]);
            }
        }

        return _result.ToArray();
    }

    public bool IsKeyDown(KeyCode key)
    {
        return Input.GetKey(key);
    }
}

