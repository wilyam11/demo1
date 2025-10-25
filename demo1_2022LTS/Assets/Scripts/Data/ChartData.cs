using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class NoteData
{
    public float x;
    public float y;
}

[Serializable]
public class ChartData
{
    public string title;
    public string artist;
    public string path;
    public int speed;
    public float bpm;
    public float start_time;
    public float delay;
    public NoteData[] notes;
}

