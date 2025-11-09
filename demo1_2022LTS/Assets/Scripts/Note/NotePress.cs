using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NotePress : MonoBehaviour
{
    private enum NoteState
    {
        Waiting,
        ActivateF,
        ActivateB,
        Death,
        Finish
    }

    public Vector2 UnitDirectionVector;
    public float StartPointRadius;
    public float TargetPointRadius;
    public float DurationMoveTime;     // 每秒走幾拍的距離
    public float DeathMoveTime;
    public float TargetTime;
    public int TargetTrackIndex;


    private NoteState state;

    private void Start()
    {
        DeathMoveTime = DurationMoveTime * TargetPointRadius / (StartPointRadius - TargetPointRadius);
        state = NoteState.Waiting;
    }

    void Update()
    {
        // Check
        if (AudioManager.Instance.BgmProgressDSP <= 0)
        {
            return;
        }

        // Baisc Movement Calculation
        //float _curTimeClamp = Mathf.Clamp01(1f - ((float)AudioManager.Instance.BgmProgressDSP - (TargetTime - DurationMoveTime)) / DurationMoveTime);
        //float _curTimeDis = _curTimeClamp * DurationMoveTime;
        //float _curDis = TargetPointRadius + (StartPointRadius - TargetPointRadius)* _curTimeClamp;

        float tStart = TargetTime - DurationMoveTime;
        float tEnd = TargetTime + DeathMoveTime;
        float tNow = (float)AudioManager.Instance.BgmProgressDSP;
        float _curTimeClamp = Mathf.Clamp01(1f - (tNow - tStart) / (tEnd - tStart));
        float _curTimeDis = _curTimeClamp * (DurationMoveTime + DeathMoveTime) - DeathMoveTime;
        float _curDis = StartPointRadius * _curTimeClamp;


        // State Machine
        bool repeat = true;
        while (repeat)
        {
            repeat = false;
            switch (state)
            {
                case NoteState.Waiting:
                    if (_curTimeDis < Level.Instance.Beat_1_4)
                    {
                        GetComponent<SpriteRenderer>().color = Color.yellow;
                        state = NoteState.ActivateF;
                        repeat = true;
                    }
                    break;
                case NoteState.ActivateF:
                    if (_curTimeDis < 0)
                    {
                        state = NoteState.ActivateB;
                        DetectLine _dl =  Level.Instance.GetDetectLine(TargetTrackIndex);
                        if(_dl != null)
                        {
                            _dl.StartCoroutine(_dl.Grow(new Vector2(1.1f, 1.5f), 0.3f));
                        }
                    }

                    // Hit
                    if (Input.anyKeyDown & Level.Instance.MouseCurrentTrack == TargetTrackIndex)
                    {
                        Level.Instance.HitPerfect();
                        FindFirstObjectByType<Cross>().PlayScaleEffect();
                        Level.Instance.GamePlay.ShowOffset(_curTimeDis);
                        state = NoteState.Finish;
                    }
                    break;
                case NoteState.ActivateB:
                    // Miss
                    if(_curTimeDis <= -Level.Instance.Beat_1_8)
                    {
                        GetComponent<SpriteRenderer>().color = Color.red;
                        state = NoteState.Death;
                    }

                    // Hit
                    if (Input.anyKeyDown & Level.Instance.MouseCurrentTrack == TargetTrackIndex)
                    {
                        Level.Instance.HitPerfect();
                        FindFirstObjectByType<Cross>().PlayScaleEffect();
                        Level.Instance.GamePlay.ShowOffset(_curTimeDis);
                        state = NoteState.Finish;
                    }
                    break;
                case NoteState.Death:
                    if (_curTimeDis <= TargetPointRadius / (TargetPointRadius - StartPointRadius))
                    {
                        state = NoteState.Finish;
                        Level.Instance.GamePlay.ShowOffset(_curTimeDis);
                    }
                    break;
                case NoteState.Finish:
                    Color _c = GetComponent<SpriteRenderer>().color;
                    _c.a *= 0.95f;
                    if (_c.a < 0.1f)
                    {
                        Destroy(gameObject);
                    }
                    GetComponent<SpriteRenderer>().color = _c;
                    break;
            }
        }

        // Move
        //Vector2 _newPos = UnitDirectionVector * _curDis;
        //transform.position = new Vector3(_newPos.x, _newPos.y, 0f);

        // 讓前端而非中心對齊判定點
        float noteLength = GetComponent<SpriteRenderer>().bounds.size.x;
        Vector2 _newPos = UnitDirectionVector * (_curDis + noteLength * 0.5f);
        transform.position = new Vector3(_newPos.x, _newPos.y, 0f);

    }
}
