using System;
using UnityEngine;

public class NailManager : MonoBehaviour
{
    [SerializeField] private NailTarget[] nails;
    private int cutCount = 0;
    private Action onAllCut;

    public void StartCutting(Action onAllCut)
    {
        this.onAllCut = onAllCut;
        cutCount = 0;
        foreach (var nail in nails) nail.Init(OnNailCut);
    }

    private void OnNailCut()
    {
        cutCount++;
        if (cutCount >= nails.Length) onAllCut?.Invoke();
    }
}