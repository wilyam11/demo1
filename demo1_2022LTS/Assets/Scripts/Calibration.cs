using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    [Header("¸`©ç½u")]
    public GameObject BeatLinePref;

    [Header("­µ²Å")]
    public GameObject NoteTapPref;
    public GameObject NoteContainer;
    public float SpawnRadius = 15f;


    public LevelState State { get; private set; } = LevelState.None;


    // Start is called before the first frame update
    void Start()
    {
        AudioClip audio = AudioManager.Instance.LoadClip("beat_128");
        AudioManager.Instance.LoadBGM(audio);
        AudioManager.Instance.PlayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene("GamePlay");
        }
    }
}
