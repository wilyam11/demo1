using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class NoteData
{
    public float s;     // Start
    public float e;     // End
    public int t;       // Track
    public int i;       // Type : 0-Press, 1-Hold
}

[Serializable]
public class ChartData
{
    public string title;
    public string artist;
    public string path;
    public int track;
    public float bpm;
    public NoteData[] notes;
}

