using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text textDisplay;
    public Dialogue dialogue;
    public float typingSpeed = 0.05f;

    private string fullText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private int index = 0;
    private Dictionary<string, string> customTags = new() 
    {
        { "orange", "#FFA500" },
        { "red", "#FF0000" },
        { "blue", "#0000FF" },
        { "green", "#006405" }
    };

    void Start()
    {
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
            GoToNextDialogue();
        }
    }
    private void GoToNextDialogue() 
    {
        if (dialogue.descriptionTexts.Count > index) 
        {
            TypeText(dialogue.descriptionTexts[index]);
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
