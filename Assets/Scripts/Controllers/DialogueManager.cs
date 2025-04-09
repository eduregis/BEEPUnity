using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text textDisplay;
    public InstructorController instructorController;
    public float typingSpeed = 0.05f;

    private string fullText;
    private Dialogue dialogue;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    [SerializeField] private PlayerProgressSO playerProgress;

    private int index = 0;
    private Dictionary<string, string> customTags = new() 
    {
        { "magenta", "#e332aa" },
        { "yellow", "#fe8305" },
        { "green", "#aedb16"}
    };
    private string[] voices = {"instructorVoice1", "instructorVoice2"};

    void Start()
    {
        StartCoroutine(InitializeDialogue());
    }

    private IEnumerator InitializeDialogue()
    {
        // Espera um frame para garantir que tudo foi inicializado
        yield return new WaitForSeconds(0.5f);
        
        if (string.IsNullOrEmpty(AppSettings.DialogueName))
        {
            Debug.LogError("Nome do diálogo não definido!");
            yield break;
        }

        dialogue = Resources.Load<Dialogue>($"Dialogues/Dialogue_{AppSettings.DialogueName}");
        if (dialogue == null)
        {
            Debug.LogError($"Diálogo não encontrado: Dialogue_{AppSettings.DialogueName}");
            yield break;
        }

        foreach (string learnId in dialogue.learnIdentifiers)
        {
            playerProgress.UnlockLearnContent(learnId);
        }

        playerProgress.SaveProgress();

        GoToNextDialogue();
    }

    public void TypeText(string text) 
    {
        // Processar as tags personalizadas antes de começar a digitação
        fullText = ReplaceCustomTags(text);
        isTyping = true;
        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeRoutine(fullText));
    }

    public void CompleteTyping() {
        if (isTyping) {
            StopCoroutine(typingCoroutine);
            textDisplay.text = fullText; // Exibir o texto completo com formatação
            isTyping = false;
            typingCoroutine = null;
        } else {
            index++;
            instructorController.ChangeArmAnimation();
            GoToNextDialogue();
        }
    }
    private void GoToNextDialogue() 
    {
        if (dialogue == null || dialogue.descriptionTexts == null || dialogue.descriptionTexts.Count == 0)
        {
            Debug.LogError("Diálogo inválido ou sem textos!");
            CanvasFadeController.Instance.HideCanvas();
            return;
        }

        if (dialogue.descriptionTexts.Count > index) 
        {
            TypeText(dialogue.descriptionTexts[index]);
        }
        else 
        {
            CanvasFadeController.Instance.HideCanvas();
        }
    }

    private IEnumerator TypeRoutine(string text) 
    {
        textDisplay.text = "";
        int charIndex = 0;
        bool insideTag = false;

        while (charIndex < text.Length) {
            // Detectar início e fim de tags
            if (text[charIndex] == '<') insideTag = true;
            if (text[charIndex] == '>') insideTag = false;

            // Apenas exibir o texto visível, incluindo tags completas
            textDisplay.text += text[charIndex];
            charIndex++;

            if (text[charIndex - 1] == '\n') {
                yield return new WaitForSeconds(0.25f);
            }

            if (!insideTag) {
                int voiceIndex = Random.Range(0, voices.Length);
                AudioManager.Instance.Play(voices[voiceIndex]);
                yield return new WaitForSeconds(typingSpeed);  // Aguarda o tempo de digitação para o próximo caractere
            }
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private string ReplaceCustomTags(string text) {
        string processedText = text;

        foreach (var tag in customTags) {
            // Substituir abertura da tag personalizada
            string openingTag = $"<{tag.Key}>";
            string richOpeningTag = $"<color={tag.Value}>";
            processedText = processedText.Replace(openingTag, richOpeningTag);

            // Substituir fechamento da tag personalizada
            string closingTag = $"</{tag.Key}>";
            string richClosingTag = "</color>";
            processedText = processedText.Replace(closingTag, richClosingTag);
        }

        return processedText;
    }
}
