using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GamePlay : MonoBehaviour
{
    private UIDocument m_Document;
    
    private Label scoreLab;
    private Label hitOffsetLab;
    private Label timeOffsetLab;
    private Label comboLab;
    private Button pauseBtn;
    private Button timeOffsetDownBtn;
    private Button timeOffsetUpBtn;

    private VisualElement pauseWindow;
    private Button backBtn;

    private void Start()
    {
        m_Document = GetComponent<UIDocument>();

        var mainArea = m_Document.rootVisualElement.Q<VisualElement>("Root");
        var centerArea = m_Document.rootVisualElement.Q<VisualElement>("CenterArea");
        var leftArea = m_Document.rootVisualElement.Q<VisualElement>("LeftArea");
        var rightArea = m_Document.rootVisualElement.Q<VisualElement>("RightArea");
        var btnArea = leftArea.Q<VisualElement>("BtnArea");

        // 左邊數據
        pauseBtn = leftArea.Q<Button>("PauseBtn");
        pauseBtn.clicked += OnPauseClicked;
        scoreLab = leftArea.Q<Label>("ScoreLab");
        hitOffsetLab = leftArea.Q<Label>("HitOffsetLab");
        ShowHitOffset(0f);
        timeOffsetLab = leftArea.Q<Label>("TimeOffsetLab");
        ShowTimeOffset();
        timeOffsetDownBtn = btnArea.Q<Button>("TimeOffsetDownBtn");
        timeOffsetDownBtn.clicked += OnTimeOffsetDownClicked;
        timeOffsetUpBtn = btnArea.Q<Button>("TimeOffsetUpBtn");
        timeOffsetUpBtn.clicked += OnTimeOffsetUpClicked;

        // 中間數據
        comboLab = centerArea.Q<Label>("ComboLab");

        // 暫停畫面
        pauseWindow = m_Document.rootVisualElement.Q<VisualElement>("PauseWindow");
        pauseWindow.visible = false;
        backBtn = pauseWindow.Q<Button>("BackBtn");
        backBtn.clicked += OnBackClicked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseWindow.visible == false)
            {
                OnPauseClicked();
            }
            else
            {
                OnBackClicked();
            }
        }
    }

    public void UpdateCombo(int combo)
    {
        comboLab.text = combo.ToString() + "\nCombo";
    }

    public void UpdateScore(int score)
    {
        scoreLab.text = "Score : " + score.ToString();
    }

    public void ShowHitOffset(float offset)
    {
        hitOffsetLab.text = "Hit Offset : " + offset.ToString("F2");
    }

    private void OnPauseClicked()
    {
        Level.Instance.Pause();
        pauseWindow.visible = true;
        pauseWindow.BringToFront();
    }

    private void OnBackClicked()
    {
        pauseWindow.visible = false;
        Level.Instance.UnPause();
    }

    private void OnTimeOffsetDownClicked()
    {
        Level.Instance.SetTimeOffset(Level.Instance.GetTimeOffset() - 0.05f);
        ShowTimeOffset();
    }

    private void OnTimeOffsetUpClicked()
    {
        Level.Instance.SetTimeOffset(Level.Instance.GetTimeOffset() + 0.05f);
        ShowTimeOffset();
    }

    private void ShowTimeOffset()
    {
        timeOffsetLab.text = "Time Offset : " + Level.Instance.GetTimeOffset().ToString("F2");
    }
}
