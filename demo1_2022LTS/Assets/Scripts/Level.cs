using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum LevelState
{
    Playing,
    Pause,
    Result,
    None
}

public class Level : MonoBehaviour
{
    [Header("畫面遮罩")]
    public SpriteMask Mask;
    public Vector2 MinMaskSize = new Vector2(10f, 10f);
    public Vector2 MaxMaskSize = new Vector2(25f, 20f);
    public bool IsShowMask = true;

    [Header("音符")]
    public GameObject NotePref;

    [Header("軌道")]
    public GameObject TrackLinePref;

    [Header("判定線")]
    public GameObject DetectLinePref;
    public float Scale = 1f;
    public float Radius = 1f;

    [Header("UI Control")]
    public GamePlay GamePlay;

    public static Level Instance { get; private set; }
    public float Score { get; private set; } = 0;
    public float Perfect { get; private set; } = 10f;
    public float Great { get; private set; } = 7f;
    public float Beat_1_10 { get; private set; }
    public float Beat_1_8 { get; private set; }
    public float Beat_1_4 { get; private set; }
    public float Beat_1_3 { get; private set; }
    public int Track { get; private set; }
    public int MouseCurrentTrack { get; private set; }
    public LevelState State { get; private set; } = LevelState.None;

    private ChartData _chartData;

    private List<GameObject> _tracks = new List<GameObject>();

    private List<GameObject> _detects = new List<GameObject>();


    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        GameManager.Instance.GameStart();
    }

    public void Update()
    {
        if(State != LevelState.Playing)
        {
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
);


        float angleRad = -Mathf.Atan2(mouseWorldPos.x, mouseWorldPos.y);
        if (angleRad < 0) angleRad += 2 * Mathf.PI;

        MouseCurrentTrack = (int)((angleRad / (2 * Mathf.PI)) * Track);
        ColoReset(MouseCurrentTrack);


        if(Input.GetKeyDown(KeyCode.F2))
        {
            IsShowMask = !IsShowMask;
            ShowMask(IsShowMask);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            State = LevelState.None;
            GameReStart();
        }
    }

    public void ShowMask(bool show)
    {
        if (show)
        {
            Mask.transform.localScale = new Vector3(MinMaskSize.x, MinMaskSize.y, 1f);
        }
        else
        {
            Mask.transform.localScale = new Vector3(MaxMaskSize.x, MaxMaskSize.y, 1f);
        }
    }

    public void HitPerfect()
    {
        Score += Perfect;
        GamePlay.UpdateScore((int)Score);
    }

    public void HitGreat()
    {
        Score += Great;
        GamePlay.UpdateScore((int)Score);
    }

    public void SpawnNotes(NoteData[] notes)
    {
        float spawnRadius = 15f;

        foreach (var noteData in notes)
        {
            float angleRad = (90f + 60f * (noteData.t + 0.5f)) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector3 spawnPosition = dir * spawnRadius;

            if(noteData.i == 0)
            {

            }

            GameObject noteObj = Instantiate(NotePref, spawnPosition, Quaternion.identity, this.transform);
            noteObj.transform.rotation = Quaternion.Euler(0f, 0f, angleRad * Mathf.Rad2Deg);

            var noteScript = noteObj.GetComponent<NotePress>();
            if (noteScript != null)
            {
                noteScript.UnitDirectionVector = dir;
                noteScript.StartPointRadius = spawnRadius;
                noteScript.TargetPointRadius = Radius;
                noteScript.DurationMoveTime = 8f;
                noteScript.DeathMoveTime = 1f;
                noteScript.TargetTime = noteData.s - 0.16f;
                noteScript.TargetTrackIndex = noteData.t;
                //noteScript.TargetTrackIndex = 0;        // 測試用
            }
        }
    }

    public void Load(string chartName)
    {
        string _json = ResourceManager.Load<TextAsset>(Path.Combine("Charts", chartName)).text;
        ChartData _chartData = JsonUtility.FromJson<ChartData>(_json);
        this._chartData = _chartData;

        // 基本數值
        Track = _chartData.track;
        Beat_1_10 = 6f / _chartData.bpm;
        Beat_1_8 = 7.5f / _chartData.bpm;
        Beat_1_4 = 15f / _chartData.bpm;
        Beat_1_3 = 20f / _chartData.bpm;

        // Note
        SpawnNotes(_chartData.notes);

        // 軌道
        GenerateTrackLine(_chartData.track, 0f);

        // 判定線
        GenerateDetectLine(_chartData.track, 0f);

        // 載入音樂
        AudioClip audio = AudioManager.Instance.LoadClip(_chartData.path);
        AudioManager.Instance.LoadBGM(audio);
    }

    public void GameStart()
    {
        GamePlay.UpdateScore(0);
        AudioManager.Instance.PlayBGM();
        State = LevelState.Playing;
    }

    public void GameReStart()
    {
        Score = 0;
        AudioManager.Instance.ResumeBgm();
        ClearAllNotes();
        SpawnNotes(_chartData.notes);
        GameStart();
    }

    public DetectLine GetDetectLine(int index)
    {
        if(index <0 || index >= _detects.Count)
        {
            Debug.LogError("Index out of range.");
            return null;
        }

        return _detects[index].GetComponent<DetectLine>();
    }

    public void ClearAllNotes()
    {
        // 只清除有 Note 組件的子物件
        foreach (Transform child in transform)
        {
            if (child.GetComponent<NotePress>() != null)
            {
                Destroy(child.gameObject);
            }
        }
    }


    public void ResetAllDetectLineColor(Color color)
    {
        foreach (var d in _detects)
        {
            var dl = d.GetComponent<DetectLine>();
            if (dl != null)
            {
                dl.SetColor(color);
            }
        }
    }

    public void ColoReset(int detectLineIndex)
    {
        if (detectLineIndex < 0 || detectLineIndex >= _detects.Count)
        {
            Debug.LogError("detectLineIndex out of range.");
            return;
        }

        ResetAllDetectLineColor(new Color(1f, 0f, 0f, 0.2f));

        var dl = _detects[detectLineIndex].GetComponent<DetectLine>();
        if (dl != null)
        {
            dl.SetColor(new Color(0f, 0.5f, 1f, 1f));
        }
    }

    public void ResetTrackLine()
    {
        foreach (var t in _tracks)
        {
            Destroy(t);
        }
    }

    public void ResetDetectLine()
    {
        foreach (var d in _detects)
        {
            Destroy(d);
        }
    }

    public void GenerateTrackLine(int count, float off_r)
    {
        if (TrackLinePref == null)
        {
            Debug.LogError("TrackLinePref shouldn't be null.");
            return;
        }

        if (count < 3)
        {
            Debug.LogError("Count must be >= 3");
            return;
        }

        float _rotationStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            GameObject _trackLine = GameObject.Instantiate(TrackLinePref, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, off_r + 90f + i * _rotationStep));
            _trackLine.name = "track_" + i.ToString();
            _tracks.Add(_trackLine);
        }
    }

    public void GenerateDetectLine(int count, float off_r)
    {
        if (DetectLinePref == null)
        {
            Debug.LogError("DetectLinePref shouldn't be null.");
            return;
        }

        if (count < 3)
        {
            Debug.LogError("Count must be >= 3");
            return;
        }

        float _rotationStep = 360f / count;
        float _length = 2f * Radius * Mathf.Tan(Mathf.PI / count) * Scale;
        for (int i = 0; i < count; i++)
        {
            GameObject _detectLine = GameObject.Instantiate(DetectLinePref, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, off_r + 90f + (i + 0.5f) * _rotationStep));
            _detectLine.name = "detect_" + i.ToString();

            DetectLine _dl = _detectLine.GetComponent<DetectLine>();
            _dl.SetLength(_length);
            _dl.SetRadius(Radius);
            _detects.Add(_detectLine);
        }
    }
}
