using UnityEngine;


public class Progress
{
    public float progress = 0f;
    public float duration;
    private float startTime;
    private float currentTime;
    private bool running = false;
    
    public delegate void OnUpdate();
    public event OnUpdate OnUpdateEvent;
    public delegate void OnComplete();
    public event OnComplete OnCompleteEvent;

    public void Start()
    {
        running = true;
        progress = 0f;
        startTime = Time.time;
    }
    public void Stop()
    {
        running = false;
    }

    public void Update()
    {
        if (!running)
            return;
        OnUpdateEvent?.Invoke();

        if (IsComplete())
        {
            running = false;
            OnCompleteEvent?.Invoke();
        }
    }

    public bool IsComplete()
    {
        if (running)
        {
            currentTime = Time.time;
            progress = (currentTime - startTime) / duration;
            if (progress >= 1f)
            {
                running = false;
            }
        }

        return progress >= 1f;
    }
}