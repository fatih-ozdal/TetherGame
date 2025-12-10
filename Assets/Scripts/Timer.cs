using UnityEngine;

public class Timer
{
    private float elapsed;
    private float duration;

    public float Elapsed => elapsed;
    public float Duration => duration;

    public bool IsRunning => elapsed < duration;
    public bool HasExpired => elapsed >= duration;

    public Timer(float duration)
    {
        this.duration = duration;
        elapsed = duration;
    }

    public void Reset()
    {
        elapsed = 0f;
    }

    public void Tick(float deltaTime)
    {
        if (elapsed < duration)
            elapsed += deltaTime;
    }

    public void ForceExpire()
    {
        elapsed = duration;
    }
}
