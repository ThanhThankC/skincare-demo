using UnityEngine;

public class BrushTool : DragTool
{
    [SerializeField] private SmoothDelete waterLayer;
    [SerializeField] private SmoothDelete dirtyLayer;
    [SerializeField] private CoverageTracker waterTracker;
    [SerializeField] private CoverageTracker dirtyTracker;

    private int frameCounter = 0;
    private const int CHECK_INTERVAL = 10;

    protected override void Update()
    {
        base.Update();
        if (!isDragging) return;
        waterLayer.PerformAtWorldPos(tipPoint.position);
        dirtyLayer.PerformAtWorldPos(tipPoint.position);

        waterLayer.FlushApply();
        dirtyLayer.FlushApply();

        frameCounter++;
        if (frameCounter >= CHECK_INTERVAL)
        {
            frameCounter = 0;
            waterTracker.Tracking();
            dirtyTracker.Tracking();
        }
    }
}
