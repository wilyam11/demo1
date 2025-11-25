using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "RhythmGame/Song Level")]
public class SongLevelSO : ScriptableObject
{
    [Header("Metadata")]
    public string songTitle;
    public string artistName;
    public Sprite coverArt;
    public float bpm;

    [Header("Audio")]
    public AudioClip fullTrack;
    public AudioClip previewClip; // Short 10s loop for the menu

    [Header("Difficulty")]
    [Range(1, 10)] public int difficultyRating;
    public int highScore = 0; // e.g., "009500"
    public string rank = "-";      // e.g., "S", "A", "B"

    // --- HELPER FUNCTION FOR EXISTING ASSETS ---
    // Add this if you want to right-click an existing song and reset it.
    // [ContextMenu("Reset Song Data")]
    // public void ResetToDefaults()
    // {
    //     highScore = 0;
    //     rank = "-";
    //     // We usually don't reset Title/Artist as you might have typed those already
    //     // Debug.Log($"{name} data has been reset.");
    // }
}
