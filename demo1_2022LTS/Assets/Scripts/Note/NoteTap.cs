using UnityEngine;


public class NoteTap : MonoBehaviour, INoteObject
{
    private Vector2 unitDirVector;
    private Note note;

    //private float startRadius;
    private float targetRadius;
    private float slope;
    private float posOffset;

    public void Initialize(Note note, Vector2 dir, float startRadius, float targetRadius)
    {
        this.note = note;
        this.unitDirVector = dir.normalized;
        //this.startRadius = startRadius;
        this.targetRadius = targetRadius;
        this.slope = (startRadius - targetRadius) / note.TravelingTime;

        //posOffset = transform.localScale.x * 0.5f;
        //posOffset = 0f;

        UpdatePos();
    }

    private void Update()
    {
        if (note == null)
        {
            return;
        }
        if(Level.Instance.State != LevelState.Playing)
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
            Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            return;
        }

        if (_isKeyDown)
        {
            if (_jdg == Judgment.Perfect)
            {
                note.State = NoteState.Hit;
                GetComponent<SpriteRenderer>().color = Color.blue;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }
            else if (_jdg == Judgment.Good)
            {
                note.State = NoteState.Hit;
                GetComponent<SpriteRenderer>().color = Color.green;
                Level.Instance.Hit(_jdg, note.CurrentTimeDis);
            }
        }
    }

    public void Hit()
    {
        FindAnyObjectByType<Cross>().PlayScaleEffect();
        note.State = NoteState.Finish;
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