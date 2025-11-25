using UnityEngine;

public interface INoteObject
{
    public void Waiting();
    public void Hit();
    public void Finish();
    public void Death();
    public void DestroySelf();
}