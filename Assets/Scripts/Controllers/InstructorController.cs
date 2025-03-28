using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InstructorController : MonoBehaviour
{
    public Sprite[] arms;
    public Sprite[] eyes;
    public Image armImage, eyesImage;

     // Variáveis de controle
    private int armCallCount = 0;
    private int currentArmIndex = 0;
    private bool isBlinking = false;

     // Configurações de tempo
    private float 
    blinkDuration = 0.1f, 
    minTimeBetweenBlinks = 2f, 
    maxTimeBetweenBlinks = 5f;

    private void Start()
    {
        // Inicializa com o primeiro conjunto de braços e olhos abertos
        SetArmSprites(currentArmIndex);
        SetEyesSprites(0); // 0 = olhos abertos
        
        // Inicia a rotina de piscar os olhos
        StartCoroutine(BlinkRoutine());
    }

    // Método público para trocar os braços
    public void ChangeArmAnimation()
    {
        armCallCount++;
        
        // A cada 3 chamadas, troca os braços
        if (armCallCount >= 3)
        {
            armCallCount = 0;
            int newArmIndex;
            do {
                newArmIndex = Random.Range(0, arms.Length);
            } while (newArmIndex == currentArmIndex && arms.Length > 1);
            
            currentArmIndex = newArmIndex;
            SetArmSprites(currentArmIndex);
        }
    }

     // Define os sprites dos braços
    private void SetArmSprites(int index)
    {
        armImage.sprite = arms[index];
    }

    // Define os sprites dos olhos
    private void SetEyesSprites(int index)
    {
        eyesImage.sprite = eyes[index];
    }

    // Rotina para piscar os olhos
    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // Tempo aleatório entre piscadas (como um humano)
            float waitTime = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
            yield return new WaitForSeconds(waitTime);
            
            // Piscar
            yield return StartCoroutine(Blink());
        }
    }

    // Animação de piscar
    private IEnumerator Blink()
    {
        if (isBlinking) yield break;
        
        isBlinking = true;
        
        // Fecha os olhos
        SetEyesSprites(1); // 1 = piscando
        
        // Mantém por um curto período
        yield return new WaitForSeconds(blinkDuration);
        
        // Reabre os olhos
        SetEyesSprites(0); // 0 = abertos
        
        isBlinking = false;
    }
}