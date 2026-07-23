using UnityEngine;

public class SmoothDelete : MonoBehaviour
{
    [SerializeField] private SpriteRenderer deleteGraphic;
    [SerializeField] private int textureSize = 512;
    [SerializeField] private float brushRadius = 0.35f;
    [SerializeField] private bool isErase = true;

    private Texture2D maskTex;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        maskTex = new Texture2D(textureSize, textureSize, TextureFormat.Alpha8, false);
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i].a = isErase ? 1f : 0f;
        maskTex.SetPixels(pixels);
        maskTex.Apply();

        deleteGraphic.material.SetTexture("_MaskTex", maskTex);
    }

    public void PerformAtWorldPos(Vector3 pos)
    {
        PerformAtScreenPos(mainCamera.WorldToScreenPoint(pos));
    }

    private void PerformAtScreenPos(Vector2 screenPos)
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Bounds bounds = deleteGraphic.bounds;

        if (!bounds.Contains(new Vector3(worldPos.x, worldPos.y, bounds.center.z))) return;

        float u = (worldPos.x - bounds.min.x) / bounds.size.x;
        float v = (worldPos.y - bounds.min.y) / bounds.size.y;

        PainAlpha(u, v);
    }

    private void PainAlpha(float u, float v)
    {
        int cx = Mathf.RoundToInt(u * textureSize);
        int cy = Mathf.RoundToInt(v * textureSize);
        int r = Mathf.RoundToInt(brushRadius * textureSize);

        for (int x = cx - r; x < cx + r; x++)
        {
            for (int y = cy - r; y < cy + r; y++)
            {
                if (x < 0 || x >= textureSize || y < 0 || y >= textureSize) continue;

                float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                if (dist > r) continue;

                float t = dist / r;
                float gradient = Mathf.SmoothStep(0f,1f, t);
                float current = maskTex.GetPixel(x, y).a;
                float newVal = isErase ? Mathf.Min(current, gradient) : Mathf.Max(current, gradient);
                maskTex.SetPixel(x, y, new Color(0f, 0f, 0f, newVal));
            }
        }
    }

    public void FlushApply() => maskTex.Apply();

    public void SetEraseMode(bool erase)
    {
        isErase = erase;
    }

    public void SetActive(bool active)
    {
        enabled = active;
    }

    public float GetCoverage()
    {
        if (maskTex == null) return 0f;
        Color[] pixels = maskTex.GetPixels();
        float total = pixels.Length;
        float changed = 0f;
        foreach (var p in pixels)
        {
            if (isErase && p.a < 0.1f) changed++;
            else if (!isErase && p.a > 0.9f) changed++;
        }
        return changed / total;
    }

    public void AutoFillComplete()
    {
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i].a = 1f;
        maskTex.SetPixels(pixels);
        maskTex.Apply();
    }

    public void AutoEraseComplete()
    {
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i].a = 0f;
        maskTex.SetPixels(pixels);
        maskTex.Apply();
    }
}
