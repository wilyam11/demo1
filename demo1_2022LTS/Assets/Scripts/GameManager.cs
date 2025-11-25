using System.Collections;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (s_Instance == null && !s_IsShuttingDown)
            {
                var newInstance = Instantiate(Resources.Load<GameManager>("GameManager"));
                newInstance.Awake();
            }
#endif
            return s_Instance;
        }

        private set => s_Instance = value;
    }
    private static GameManager s_Instance;

    public static bool IsShuttingDown 
    {
        get 
        { 
            return s_IsShuttingDown;
        } 
    }
    private static bool s_IsShuttingDown = false;

    private void OnDestroy()
    {
        if (s_Instance == this) s_IsShuttingDown = true;
    }

    void Awake()
    {
        if (s_Instance == this)
        {
            return;
        }

        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
        }
    }

    private void Update()
    {

    }

    public void GameStart()
    {
        Level.Instance.Load("TakeMeHome");
        Level.Instance.GameStart();
    }
}
