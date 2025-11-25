using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    private UIDocument m_Document;

    void Start()
    {
        m_Document = GetComponent<UIDocument>();

        var mainArea = m_Document.rootVisualElement.Q<VisualElement>("MainArea");

        var startButton = mainArea.Q<Button>("StartBtn");
        startButton.clicked += OnStartClicked;

        var settingButton = mainArea.Q<Button>("SettingBtn");
        settingButton.clicked += OnSettingClicked;
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene("GamePlay");
    }

    private void OnSettingClicked()
    {

    }
}
