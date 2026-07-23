using System;
using UnityEngine;

public class CoverageTracker : MonoBehaviour
{
    [SerializeField] private float coverageThreshold = 0.60f;

    private SmoothDelete smoothDelete;
    private Action onComplete;
    private bool isTracking;
    private bool isCompleted; 

    public void StartTracking(Action onComplete)
    {
        smoothDelete = GetComponent<SmoothDelete>();
        this.onComplete = onComplete;
        isTracking = true;
        isCompleted = false;
    }

    public void StopTracking()
    {
        isTracking = false;
    }

    public void Tracking()
    {
        if (!isTracking || isCompleted) return;
        float coverage = smoothDelete.GetCoverage();
        if (coverage >= coverageThreshold)
        {
            isCompleted = true;
            isTracking = false;
            onComplete?.Invoke();
        }
    }
}