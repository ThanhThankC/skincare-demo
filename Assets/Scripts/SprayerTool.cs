using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayerTool : DragTool
{
    [SerializeField] private SmoothDelete smooth;
    [SerializeField] private CoverageTracker coverageTracker;
    [SerializeField] private GameObject sprayFX;
    [SerializeField] private float duration = 0.5f;

    private Coroutine sprayCoroutine;
    private bool isSpraying;

    protected override void Start()
    {
        base.Start();
        HideVFX();
    }

    protected override void Update()
    {
        base.Update();
        if (!isDragging) return;

        if (Input.GetMouseButton(1))
            SprayWater();
    }

    private void SprayWater()
    {
        if (isSpraying) return;
        isSpraying = true;
        if (sprayCoroutine != null)
            StopCoroutine(sprayCoroutine);
        sprayCoroutine = StartCoroutine(SprayApply());
    }

    private IEnumerator SprayApply()
    {
        smooth.PerformAtWorldPos(tipPoint.position);
        smooth.FlushApply();
        coverageTracker.Tracking();
        sprayFX.SetActive(true);
        yield return new WaitForSeconds(duration);
        HideVFX();
        yield return new WaitForSeconds(0.3f);
        isSpraying = false;
    }

    private void HideVFX()
    {
        sprayFX.SetActive(false);
    }
}
