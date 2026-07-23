using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoisturizeTool : DragTool
{
    [SerializeField] private SmoothDelete smooth;
    [SerializeField] private CoverageTracker coverageTracker;


    protected override void Update()
    {
        base.Update();
        if (!isDragging) return;

        smooth.PerformAtWorldPos(tipPoint.position);
        smooth.FlushApply();
        coverageTracker.Tracking();
    }

}
