using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private SmoothDelete waterSprayDelete; 
    [SerializeField] private SmoothDelete dirtyDelete;     
    [SerializeField] private SmoothDelete medicineDelete;     
    [SerializeField] private CoverageTracker waterCoverage;
    [SerializeField] private CoverageTracker dirtyCoverage;
    [SerializeField] private CoverageTracker medicineCoverage;

    [Header("Tools & Props")]
    [SerializeField] private GameObject handPointer;
    [SerializeField] private GameObject sprayer;
    [SerializeField] private GameObject brushTool;
    [SerializeField] private GameObject scissorTool;
    [SerializeField] private GameObject moisturizeTool;
    [SerializeField] private GameObject baseBox;
    [SerializeField] private GameObject towel;
    [SerializeField] private GameObject oldNail;
    [SerializeField] private NailManager nailManager;

    [Header("UI")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private GameObject blinkFx;
    [SerializeField] private ParticleSystem resultParticle1;
    [SerializeField] private ParticleSystem resultParticle2;

    [Header("Settings")]
    [SerializeField] private float towelWaitDuration = 2f;
    [SerializeField] private Vector3 handPointerOffset = new Vector3(0.5f, -0.5f, 0f);

    private int currentStep = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() => GoToStep(0);

    public void GoToStep(int step)
    {
        currentStep = step;
        StopAllCoroutines();

        switch (step)
        {
            case 0: StartCoroutine(Step0_Start()); break;
            case 1: StartCoroutine(Step1_ShowSprayer()); break;
            case 2: StartCoroutine(Step2_SprayWater()); break;
            case 4: StartCoroutine(Step4_BrushClean()); break;
            case 6: StartCoroutine(Step6_ShowScissor()); break;
            case 7: StartCoroutine(Step7_CutNails()); break;
            case 8: StartCoroutine(Step8_ShowMoisturize()); break;
            case 9: StartCoroutine(Step9_Moisturize()); break;
            case 10: StartCoroutine(Step10_Towel()); break;
            case 12: StartCoroutine(Step12_Result()); break;
        }
    }

    public void NextStep() => GoToStep(currentStep + 1);

    IEnumerator Step0_Start()
    {
        SetAllToolsInactive();
        startScreen.SetActive(true);
        yield break;
    }

    IEnumerator Step1_ShowSprayer()
    {
        startScreen.SetActive(false);
        sprayer.SetActive(true);
        handPointer.SetActive(true);
        handPointer.transform.position = GetHandTargetPosition(sprayer);
        yield break;
    }

    IEnumerator Step2_SprayWater()
    {
        handPointer.SetActive(false);

        waterSprayDelete.SetEraseMode(false);
        waterCoverage.StartTracking(onComplete: () =>
        {
            waterSprayDelete.AutoFillComplete();
            sprayer.SetActive(false);
            StartCoroutine(PepareToNextStep(4));
        });
        yield break;
    }

    // STEP 3: Coverage check (handled by CoverageTracker callback)

    IEnumerator Step4_BrushClean()
    {
        brushTool.SetActive(true);
        handPointer.SetActive(true);
        handPointer.transform.position = GetHandTargetPosition(brushTool);

        waterSprayDelete.SetEraseMode(true);
        dirtyDelete.SetEraseMode(true);
        dirtyDelete.SetActive(true);

        dirtyCoverage.StartTracking(onComplete: () =>
        {
            dirtyDelete.AutoEraseComplete();
            waterSprayDelete.AutoEraseComplete();
            brushTool.SetActive(false);
            StartCoroutine(PepareToNextStep(6));
        });
        yield break;
    }

    // STEP 5: Coverage check (handled by callbacks)

    IEnumerator Step6_ShowScissor()
    {
        scissorTool.SetActive(true);
        handPointer.SetActive(true);
        handPointer.transform.position = GetHandTargetPosition(scissorTool);
        yield break;
    }

    IEnumerator Step7_CutNails()
    {
        handPointer.SetActive(false);
        nailManager.StartCutting(onAllCut: () => StartCoroutine(PepareToNextStep(8)));
        yield break;
    }

    IEnumerator Step8_ShowMoisturize()
    {
        scissorTool.SetActive(false);
        moisturizeTool.SetActive(true);
        baseBox.SetActive(true);
        handPointer.SetActive(true);
        handPointer.transform.position = GetHandTargetPosition(moisturizeTool);
        yield break;
    }

    IEnumerator Step9_Moisturize()
    {
        handPointer.SetActive(false);
        medicineDelete.SetEraseMode(false);
        medicineCoverage.StartTracking(onComplete: () =>
        {
            medicineDelete.AutoFillComplete();
            moisturizeTool.SetActive(false);
            baseBox.SetActive(false);
            StartCoroutine(PepareToNextStep(10));
        });
        yield break;
    }

    IEnumerator Step10_Towel()
    {
        moisturizeTool.SetActive(false);
        towel.SetActive(true);
        yield return new WaitForSeconds(towelWaitDuration);
        GoToStep(12);
    }

    IEnumerator Step12_Result()
    {
        medicineDelete.gameObject.SetActive(false);
        towel.SetActive(false);
        blinkFx.SetActive(true);
        oldNail.SetActive(false);
        resultParticle1.Play();
        resultParticle2.Play();
        yield return new WaitForSeconds(3f);
        resultScreen.SetActive(true);
    }

    private IEnumerator PepareToNextStep(int step, float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        GoToStep(step);
    }

    public void OnToolPickedUp(DragTool tool, float delay = 0.5f)
    {
        handPointer.SetActive(false);
    }

    public void OnToolReleased(DragTool tool, float delay = 0.5f)
    {
        if (currentStep == 1 || currentStep == 6 || currentStep == 8)
            Invoke(nameof(EnableHandPointer), delay);
    }

    private void EnableHandPointer()
    {
        handPointer.SetActive(true);
    }

    private void SetAllToolsInactive()
    {
        sprayer.SetActive(false);
        handPointer.SetActive(false);
        brushTool.SetActive(false);
        scissorTool.SetActive(false);
        moisturizeTool.SetActive(false);
        baseBox.SetActive(false);
        towel.SetActive(false);
        blinkFx.SetActive(false);
    }

    private Vector3 GetHandTargetPosition(GameObject target)
    {
        return target.transform.position + handPointerOffset;
    }
}