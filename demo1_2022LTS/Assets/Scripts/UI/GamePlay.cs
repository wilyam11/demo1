using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GamePlay : MonoBehaviour
{
    private UIDocument m_Document;

    private VisualElement Root;

    private Label scoreLab;
    private Label hitOffsetLab;
    private Label timeOffsetLab;
    private Label comboLab;
    private Button pauseBtn;
    private Button timeOffsetDownBtn;
    private Button timeOffsetUpBtn;

    private VisualElement pauseWindow;
    private Button backBtn;

    // 結算畫面
    private VisualElement CompleteWindow;
    private Button MenuBtn;
    private Button RetryBtn;
    private Label PerfectLab;
    private Label GreatLab;
    private Label GoodLab;
    private Label MissLab;
    private Label ScoreLab;
    private Label HighScoreLab;
    private Label HighComboLab;

    private void Start()
    {
        m_Document = GetComponent<UIDocument>();

        Root = m_Document.rootVisualElement.Q<VisualElement>("Root");

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

        // 結算畫面
        CompleteWindow = m_Document.rootVisualElement.Q<VisualElement>("CompleteWindow");
        CompleteWindow.visible = false;
        MenuBtn = CompleteWindow.Q<Button>("MenuBtn");
        MenuBtn.clicked += OnComfirmCkicked;
        RetryBtn = CompleteWindow.Q<Button>("RetryBtn");
        RetryBtn.clicked += OnRetryClicked;
        PerfectLab = CompleteWindow.Q<Label>("PerfectLab");
        GreatLab = CompleteWindow.Q<Label>("GreatLab");
        GoodLab = CompleteWindow.Q<Label>("GoodLab");
        MissLab = CompleteWindow.Q<Label>("MissLab");
        ScoreLab = CompleteWindow.Q<Label>("ScoreLab");
        HighScoreLab = Root.Q<Label>("HighScoreLab");
        HighComboLab = Root.Q<Label>("HighComboLab");
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

    public void ShowCompleteWindow(int score, int combo, ScoreData scoreData)
    {
        CompleteWindow.visible = true;
        PerfectLab.text = scoreData.Get(Judgment.Perfect).ToString();
        GreatLab.text = "0";
        GoodLab.text = scoreData.Get(Judgment.Good).ToString();
        MissLab.text = scoreData.Get(Judgment.Miss).ToString();
        ScoreLab.text = score.ToString();
        HighScoreLab.text = score.ToString();
        HighComboLab.text = combo.ToString();
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

    private void OnComfirmCkicked()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnRetryClicked()
    {
        Action action = () =>
        {
            CompleteWindow.visible = false;
            Level.Instance.GameReStart();
            Root.style.opacity = 1f;
        };

        StartCoroutine(FadeOut(action));
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

    private IEnumerator FadeOut(Action finishAction)
    {
        float duration = 1f;
        float elapsed = 0f;
        float startOpacity = Root.resolvedStyle.opacity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Root.style.opacity = Mathf.Lerp(startOpacity, 0f, t);
            yield return null;
        }
        Root.style.opacity = 0f;

        finishAction?.Invoke();
    }
}
