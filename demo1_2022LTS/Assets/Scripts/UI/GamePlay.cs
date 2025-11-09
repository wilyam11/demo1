using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GamePlay : MonoBehaviour
{
    private UIDocument m_Document;
    
    private Label scoreLabel;
    private Label offsetLab;

    void Start()
    {
        m_Document = GetComponent<UIDocument>();

        var mainArea = m_Document.rootVisualElement.Q<VisualElement>("Root");
        var centerArea = m_Document.rootVisualElement.Q<VisualElement>("CenterArea");
        var leftArea = m_Document.rootVisualElement.Q<VisualElement>("LeftArea");
        var rightArea = m_Document.rootVisualElement.Q<VisualElement>("RightArea");

        scoreLabel = centerArea.Q<Label>("ScoreLab");
        if (scoreLabel == null)
        {
            Debug.LogError("ScoreLab not found in UI Document.");
        }

        offsetLab = leftArea.Q<Label>("OffsetLab");
        if (offsetLab == null)
        {
            Debug.LogError("OffsetLab not found in UI Document.");
        }
    }

    public void UpdateScore(int score)
    {
        scoreLabel.text = score.ToString();
    }

    public void ShowOffset(float offset)
    {
        offsetLab.text = offset.ToString("F2");
    }
}
