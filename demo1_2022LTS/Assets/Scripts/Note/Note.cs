using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public enum NoteState
{
    Waiting,
    Hit,
    Finish,
    Death
}

public enum NoteType
{
    Tap = 0,
    Hold = 1
}

public class Note
{
    public float CurrentTimeDis { get; private set; } = 999f;
    public float StartTime { get { return targetTime + travelingTime; } }
    public float TargetTime { get { return targetTime - OffsetTime; }}
    public float TravelingTime { get { return travelingTime; } }
    public float HoldTime { get; set; }
    public float OffsetTime { get; set; } = 0f;
    public int TrackIndex { get; private set; }
    public NoteState State { get; set; } = NoteState.Waiting;
    public INoteObject NoteObject { get; set; }
    public KeyCode HitKey { get; set; }

    private float targetTime;
    private float travelingTime;
    

    public Note(float targetTime, float travelingTime, int trackIndex, float holdTime = 0f)
    {
        this.targetTime = targetTime;
        this.travelingTime = travelingTime;
        
        HoldTime = holdTime;
        TrackIndex = trackIndex;

        UpdateTime(0f);
    }

    public void UpdateTime(float dsp)
    {
        CurrentTimeDis = TargetTime - dsp;
    }

    public void UpdateLogic()
    {
        switch (State)
        {
            case NoteState.Waiting:
                NoteObject.Waiting();
                break;
            case NoteState.Hit:
                NoteObject.Hit();
                break;
            case NoteState.Finish:
                NoteObject.Finish();
                break;
            case NoteState.Death:
                NoteObject.Death();
                break;
        }
    }
}