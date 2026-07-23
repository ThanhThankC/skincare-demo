using System;
using System.Collections;
using UnityEngine;

public class NailTarget : MonoBehaviour
{
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float slideDistance = 1f;
    [SerializeField] private Animator scissorAnimator;

    private SpriteRenderer sr;
    private Action onCut;
    private bool isCut = false;

    public void Init(Action onCut)
    {
        this.onCut = onCut;
        isCut = false;
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCut) return;
        if (!other.CompareTag("ScissorTip")) return;

        isCut = true;
        scissorAnimator?.SetTrigger("Cut");
        StartCoroutine(SlideAndFade());
        onCut?.Invoke();
    }

    private IEnumerator SlideAndFade()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * slideDistance;
        Color startColor = sr.color;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }

        gameObject.SetActive(false);
    }
}