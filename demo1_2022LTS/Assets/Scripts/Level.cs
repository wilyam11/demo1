using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    [Header("節拍線")]
    public GameObject BeatLinePref;

    [Header("音符")]
    public GameObject NoteTapPref;
    public GameObject NoteHoldPref;
    public GameObject NoteContainer;
    public float SpawnRadius = 15f;

    [Header("判定線")]
    public List<GameObject> DetectLines = new List<GameObject>();
    public float Radius = 1f;

    [Header("UI Control")]
    public GamePlay GamePlay;

    public static Level Instance { get; private set; }
    public float Score { get; private set; } = 0;
    public int Combo { get; private set; } = 0;
    public float Perfect { get; private set; } = 10f;
    public float Good { get; private set; } = 7f;
    public int MouseCurrentTrack { get; private set; }
    public JudgementData JudgementData { get; private set; }
    public LevelState State { get; private set; } = LevelState.None;

    public const int TrackCount = 6;

    private ChartData chartData;

    private List<Note>[] notes = new List<Note>[TrackCount];
    private List<Note> finishNotes = new List<Note>();

    private float cusTimeOffset = 0f;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        float angleRad = 2 * Mathf.PI / TrackCount;
        float detectLineLength = Radius * angleRad + 0.1f;

        foreach (var d in DetectLines)
        {
            var dl = d.GetComponent<DetectLine>();
            if (dl != null)
            {
                dl.SetRadius(Radius);
                dl.SetLength(detectLineLength);
            }
        }

        for (int i = 0; i < TrackCount; i++)
        {
            notes[i] = new List<Note>();
        }

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

        MouseCurrentTrack = (int)((angleRad / (2 * Mathf.PI)) * TrackCount);
        ColoReset(MouseCurrentTrack);


        if(Input.GetKeyDown(KeyCode.F2))
        {
            IsShowMask = !IsShowMask;
            ShowMask(IsShowMask);
        }

        if(Input.GetKeyDown(KeyCode.F3))
        {
            State = LevelState.None;
            GameReStart();
        }

        // Finish Note 狀態更新
        foreach (var note in finishNotes)
        {
            note.UpdateTime((float)AudioManager.Instance.BgmProgressDSP);
            note.UpdateLogic();

            if(note.State == NoteState.Death)
            {
                note.NoteObject.DestroySelf();
            }
        }
        finishNotes = finishNotes.FindAll(n => n.State != NoteState.Death);

        // Note 狀態更新
        for (int i = 0; i < TrackCount; i++)
        {
            if(notes[i].Count == 0)
            {
                continue;
            }

            foreach (var note in notes[i])
            {
                note.UpdateTime((float)AudioManager.Instance.BgmProgressDSP);
            }

            Note _note = notes[i][0];
            _note.UpdateLogic();

            if (_note.State == NoteState.Finish)
            {
                notes[i].RemoveAt(0);
                finishNotes.Add(_note);
            }
        }
    }

    public void Load(string chartName)
    {
        string _json = ResourceManager.Load<TextAsset>(Path.Combine("Charts", chartName)).text;
        ChartData _chartData = JsonUtility.FromJson<ChartData>(_json);
        this.chartData = _chartData;

        // Note
        SpawnNotes(_chartData.notes);

        // 判定資料
        JudgementData = new JudgementData(_chartData.bpm);

        // 載入音樂
        AudioClip audio = AudioManager.Instance.LoadClip(_chartData.path);
        AudioManager.Instance.LoadBGM(audio);
    }

    public void GameStart()
    {
        GamePlay.UpdateScore(0);
        GamePlay.UpdateCombo(0);
        GamePlay.ShowHitOffset(0f);
        AudioManager.Instance.PlayBGM();
        State = LevelState.Playing;
    }

    public void GameReStart()
    {
        Score = 0;
        Combo = 0;
        AudioManager.Instance.StopBGM();
        ClearAllNotes();
        SpawnNotes(chartData.notes);
        GameStart();
    }

    public void Pause()
    {
        AudioManager.Instance.PauseBGM();
        State = LevelState.Pause;
    }

    public void UnPause()
    {
        State = LevelState.Playing;
        AudioManager.Instance.ResumeBgm();
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

    public void Hit(Judgment judgment, float delay)
    {
        switch(judgment)
        {
            case Judgment.Perfect:
                Combo++;
                GamePlay.UpdateCombo(Combo);
                Score += Perfect;
                GamePlay.UpdateScore((int)Score);
                GamePlay.ShowHitOffset(delay);
                break;
            case Judgment.Good:
                Combo++;
                GamePlay.UpdateCombo(Combo);
                Score += Good;
                GamePlay.UpdateScore((int)Score);
                GamePlay.ShowHitOffset(delay);
                break;
            case Judgment.Miss:
                Combo = 0;
                GamePlay.UpdateCombo(Combo);
                GamePlay.ShowHitOffset(delay);
                break;
        }
    }

    public void SpawnNotes(NoteData[] noteDataList)
    {
        float _trackAngle = 360f / TrackCount;
        float _travelingTime = 12f;

        foreach (var noteData in noteDataList)
        {
            int _index = noteData.t;
            float _angleRad = (90f + _trackAngle * (noteData.t + 0.5f)) * Mathf.Deg2Rad;
            Vector2 _dir = new Vector2(Mathf.Cos(_angleRad), Mathf.Sin(_angleRad));
            Vector3 _spawnPos = _dir * SpawnRadius;

            // 基底 Note 類別
            Note _note = new Note(noteData.s - 0.25f, _travelingTime, _index);
            notes[_index].Add(_note);

            // 功能 Note 物件 (Mono)
            GameObject _noteObj = null;
            NoteType _type = (NoteType)noteData.i;
            if (_type == NoteType.Tap)
            {
                // 物件實例化
                _noteObj = Instantiate(NoteTapPref, _spawnPos, Quaternion.identity, this.transform);
                _noteObj.transform.rotation = Quaternion.Euler(0f, 0f, _angleRad * Mathf.Rad2Deg);
                _noteObj.transform.parent = NoteContainer.transform;

                // 組件程式初始化
                NoteTap _noteTap = _noteObj.GetComponent<NoteTap>();
                _noteTap.Initialize(_note, _dir, SpawnRadius, Radius);
                _note.NoteObject = _noteTap;
            }
            else if(_type == NoteType.Hold)
            {
                // Hold 專屬設定
                _note.HoldTime = noteData.p;

                // 物件實例化
                _noteObj = Instantiate(NoteHoldPref, _spawnPos, Quaternion.identity, this.transform);
                _noteObj.transform.rotation = Quaternion.Euler(0f, 0f, _angleRad * Mathf.Rad2Deg);
                _noteObj.transform.parent = NoteContainer.transform;

                // 組件程式初始化
                NoteHold _noteHold = _noteObj.GetComponent<NoteHold>();
                _noteHold.Initialize(_note, _dir, SpawnRadius, Radius);
                _note.NoteObject = _noteHold;
            }

            // 處理遮罩層級
            if (_noteObj != null)
            {
                if (noteData.t > 2)
                {
                    _noteObj.GetComponent<SpriteRenderer>().sortingOrder = -2;
                }
                else
                {
                    _noteObj.GetComponent<SpriteRenderer>().sortingOrder = -4;
                }
            }
        }

        // Sort Notes by StartTime (Small -> Big)
        for (int i = 0; i < TrackCount; i++)
        {
            notes[i].Sort((a, b) => a.TargetTime.CompareTo(b.TargetTime));
        }
    }

    public void ClearAllNotes()
    {
        foreach (Transform child in NoteContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < TrackCount; i++)
        {
            notes[i].Clear();
        }

        finishNotes.Clear();
    }

    public DetectLine GetDetectLine(int index)
    {
        if(index <0 || index >= DetectLines.Count)
        {
            Debug.LogError("Index out of range.");
            return null;
        }

        return DetectLines[index].GetComponent<DetectLine>();
    }

    public void ResetAllDetectLineColor(Color color)
    {
        foreach (var d in DetectLines)
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
        if (detectLineIndex < 0 || detectLineIndex >= DetectLines.Count)
        {
            Debug.LogError("detectLineIndex out of range.");
            return;
        }

        ResetAllDetectLineColor(new Color(1f, 0f, 0f, 0.2f));

        var dl = DetectLines[detectLineIndex].GetComponent<DetectLine>();
        if (dl != null)
        {
            dl.SetColor(new Color(0f, 0.5f, 1f, 1f));
        }
    }

    public void SetTimeOffset(float offset)
    {
        if(offset < -3f || offset > 3f)
        {
            return;
        }

        cusTimeOffset = offset;

        // 立即更新所有 Note
        foreach (var trackNote in notes)
        {
            foreach (var note in trackNote)
            {
                note.OffsetTime = cusTimeOffset;
            }
        }
    }

    public float GetTimeOffset()
    {
        return cusTimeOffset;   
    }
}