using System.Collections;
using UnityEngine;

public enum GameState
{
    Menu,
    Ready,
    Playing,
    Paused,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; }

    public NoteManager noteManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(GameStart());
    }

    private void Update()
    {

    }

    IEnumerator GameStart()
    {
        string json = ResourceManager.Instance.Load<TextAsset>("Charts/PrimitiveTide").text;
        ChartData chartData = JsonUtility.FromJson<ChartData>(json);

        // 等待 NoteManager 載入完成
        noteManager.SpawnNotes(chartData.notes,  chartData.speed);

        AudioClip audio = AudioManager.Instance.LoadClip(chartData.path);
        AudioManager.Instance.LoadBGM(audio);

        yield return new WaitForSeconds(1f);

        SetState(GameState.Playing);
        AudioManager.Instance.PlayBGM(chartData.start_time / 1000.0);
    }

    public void SetState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.Menu:
                // 停止音樂、清除Note等
                break;
            case GameState.Ready:
                // 倒數、準備開始
                break;
            case GameState.Playing:
                Debug.Log("Game Start!");
                break;
            case GameState.Paused:
                // 暫停音樂
                break;
            case GameState.Result:
                // 顯示分數
                break;
        }
    }
}
