using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteHold : MonoBehaviour, INoteObject
{
    private Vector2 unitDirVector;
    private Note note;

    //private float startRadius;
    private float targetRadius;
    private float slope;
    private float posOffset;

    private SpriteRenderer holdPartRender;
    private Transform holdPartTransform;

    public void Initialize(Note note, Vector2 dir, float startRadius, float targetRadius)
    {
        this.note = note;
        unitDirVector = dir.normalized;
        //this.startRadius = startRadius;
        this.targetRadius = targetRadius;
        slope = (startRadius - targetRadius) / note.TravelingTime;

        //posOffset = transform.localScale.x * 0.5f;
        posOffset = 0f;

        float _holdScale = slope * note.HoldTime / transform.localScale.x;
        float _holdPosOffset = 0.5f + _holdScale / 2f;
        holdPartRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        holdPartTransform = transform.GetChild(0);
        holdPartTransform.localScale = new Vector3(_holdScale, 1f, 1f);
        holdPartTransform.localPosition = new Vector3(_holdPosOffset, 0f, 0f);

        UpdatePos();
    }

    private void Update()
    {
        if (note == null)
        {
            return;
        }
        if (Level.Instance.State != LevelState.Playing)
        {
            return;
        }

        UpdatePos();
    }

    public void UpdatePos()
    {
        float _curDistanceDis = targetRadius + slope * note.CurrentTimeDis;
        Vector2 _newPos = unitDirVector * (_curDistanceDis + posOffset);
        transform.position = new Vector3(_newPos.x, _newPos.y, 0f);
    }

    public void Waiting()
    {
        KeyCode[] _curHitKeys = InputManager.Instance.GetCurrentHitKey();
        bool _isKeyDown = (_curHitKeys.Length > 0) && Level.Instance.MouseCurrentTrack == note.TrackIndex;

        Judgment _jdg = Level.Instance.JudgementData.Judge(note.CurrentTimeDis);
        if (_jdg == Judgment.Miss)
        {
            note.State = NoteState.Finish;
            GetComponent<SpriteRenderer>().color = Color.red;
            holdPartRender.color += Color.red;
            Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            return;
        }

        if (_isKeyDown)
        {
            if (_jdg == Judgment.Perfect)
            {
                note.State = NoteState.Hit;
                note.HitKey = _curHitKeys[0];
                GetComponent<SpriteRenderer>().color = Color.blue;
                holdPartRender.color += Color.yellow;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }
            else if (_jdg == Judgment.Good)
            {
                note.State = NoteState.Hit;
                note.HitKey = _curHitKeys[0];
                GetComponent<SpriteRenderer>().color = Color.green;
                holdPartRender.color += Color.yellow;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }
        }
    }

    public void Hit()
    {
        // 提早 or 晚一點放
        if (!InputManager.Instance.IsKeyDown(note.HitKey))
        {
            Judgment _jdg = Level.Instance.JudgementData.Judge(note.CurrentTimeDis + note.HoldTime);
            if (_jdg == Judgment.Perfect)
            {
                FindAnyObjectByType<Cross>().PlayScaleEffect();
                note.State = NoteState.Finish;
                holdPartRender.color += Color.blue;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }
            else if (_jdg == Judgment.Good)
            {
                FindAnyObjectByType<Cross>().PlayScaleEffect();
                note.State = NoteState.Finish;
                holdPartRender.color += Color.green;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }
            else
            {
                FindAnyObjectByType<Cross>().PlayScaleEffect();
                note.State = NoteState.Finish;
                holdPartRender.color += Color.red;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }

            return;
        }

        // 壓到最後
        if(note.CurrentTimeDis <= -note.HoldTime)
        {
            FindAnyObjectByType<Cross>().PlayScaleEffect();
            note.State = NoteState.Finish;
            holdPartRender.color += Color.blue;
            Level.Instance.Hit(Judgment.Perfect, note.CurrentTimeDis);
        }
    }

    public void Finish()
    {
        note.State = NoteState.Death;
    }

    public void Death()
    {

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
