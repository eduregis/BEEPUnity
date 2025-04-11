using UnityEngine;
using System.Collections;

public class InstructorIdleAnimator : MonoBehaviour
{
    public RectTransform head, body, armBack, armFront, neckConectors;
    public GameObject eyesOpen, eyesClosed;

    public float stepInterval = 2f; // tempo entre cada step
    public int[] headPattern = new int[] { 0, 1, 0, -1 }; // pixels de deslocamento
    public int[] bodyPattern = new int[] { 0, 1, 0, 0 };
    public int[] armsPattern = new int[] { 0, 0, 0, 1 };

    public float blinkInterval = 3f;
    public float blinkDuration = 0.1f;
    public Vector2 eyesOffset = new Vector2(0, -2); // ajuste fino dos olhos

    private Vector3 headStart, bodyStart, armBackStart, armFrontStart, neckConectorsStart;
    private int stepIndex = 0;
    private float timer = 0f;
    private float blinkTimer = 0f;

    void Start()
    {
        headStart = head.localPosition;
        bodyStart = body.localPosition;
        armBackStart = armBack.localPosition;
        armFrontStart = armFront.localPosition;
        neckConectorsStart = neckConectors.localPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (timer >= stepInterval)
        {
            StepAnimation();
            timer = 0f;
        }

        if (blinkTimer >= blinkInterval)
        {
            StartCoroutine(Blink());
            blinkTimer = 0f + Random.Range(-0.5f, 0.5f);
        }
    }

    void StepAnimation()
    {
        int headOffset = headPattern[stepIndex % headPattern.Length];
        int bodyOffset = bodyPattern[stepIndex % bodyPattern.Length];
        int armsOffset = armsPattern[stepIndex % armsPattern.Length];

        head.localPosition = headStart + new Vector3(0, headOffset, 0);
        body.localPosition = bodyStart + new Vector3(0, bodyOffset, 0);
        armFront.localPosition = armFrontStart + new Vector3(0, armsOffset, 0);
        armBack.localPosition = armBackStart + new Vector3(0, armsOffset, 0);
        neckConectors.localPosition = headStart + new Vector3(0, headOffset, 0);

        stepIndex++;
    }

    IEnumerator Blink()
    {
        eyesOpen.SetActive(false);
        eyesClosed.SetActive(true);
        yield return new WaitForSeconds(blinkDuration);
        eyesOpen.SetActive(true);
        eyesClosed.SetActive(false);
    }
}
