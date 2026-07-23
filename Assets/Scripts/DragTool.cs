using UnityEngine;

public class DragTool : MonoBehaviour
{
    [SerializeField] protected Transform tipPoint;

    [Header("Snap back when drop (SprayerTool = true, BrushTool = false)")]
    [SerializeField] private bool snapBackOnRelease = false;
    [SerializeField] private float snapSpeed = 8f;
    [SerializeField] private bool triggerNextStepOnFirstDrag = false;

    private Camera mainCamera;
    private Vector3 originPosition;
    protected bool isDragging = false;
    private bool isSnapping = false;
    private bool hasTriggeredNextStep = false;

    public Vector2 TipWorldPosition => tipPoint.position;
    public bool IsDragging => isDragging;

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        originPosition = transform.position;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        isSnapping = false;
        GameFlowManager.Instance.OnToolPickedUp(this);
        if (triggerNextStepOnFirstDrag && !hasTriggeredNextStep)
        {
            hasTriggeredNextStep = true;
            GameFlowManager.Instance.NextStep();
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        if (snapBackOnRelease)
        {
            isSnapping = true;
            GameFlowManager.Instance.OnToolReleased(this);
        }
    }

    protected virtual void Update()
    {
        if (isDragging)
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = transform.position.z;
            transform.position = mouseWorld;
        }

        if (isSnapping)
        {
            transform.position = Vector3.Lerp(transform.position, originPosition, Time.deltaTime * snapSpeed);
            if (Vector3.Distance(transform.position, originPosition) < 0.01f)
            {
                transform.position = originPosition;
                isSnapping = false;
            }
        }
    }
}