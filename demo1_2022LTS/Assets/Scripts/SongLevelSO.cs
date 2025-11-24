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
    public string highScore; // e.g., "009500"
    public string rank;      // e.g., "S", "A", "B"
}
