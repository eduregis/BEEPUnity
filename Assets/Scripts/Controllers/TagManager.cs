using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TagManager : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private RectTransform boxCentral;
    [SerializeField] private List<LearnTag> tags = new();
    [SerializeField] private Image line;
    
    [Header("Otimização")]
    [SerializeField] private float clickCooldown = 0.3f;
    
    private int currentSelectedIndex = -1;
    private bool isProcessing = false;

    private void Start()
    {
        for (int i = 0; i < tags.Count; i++)
        {
            int index = i;
            tags[i].button.onClick.AddListener(() => OnTagClicked(index));
        }

        LearnUIManager.Instance.ShowFilteredItems(
            LearnDataManager.Instance.GetFilteredLearnData(Constants.LearnTag.All.ToString())
        );

        line.color = tags[0].color;
    }

    private void OnTagClicked(int index)
    {
        if (isProcessing || currentSelectedIndex == index) return;
        
        LearnUIManager.Instance.ShowFilteredItems(
            LearnDataManager.Instance.GetFilteredLearnData(tags[index].learnTag.ToString())
        );

        line.color = tags[index].color;

        StartCoroutine(ProcessTagSelection(index));
    }

    private System.Collections.IEnumerator ProcessTagSelection(int index)
    {
        isProcessing = true;
        
        // Seleciona a nova tag
        currentSelectedIndex = index;
        
        // Ajusta a ordem hierárquica
        UpdateHierarchyOrder(index);

        AudioManager.Instance.Play("chooseLevel");
        
        yield return new WaitForSeconds(clickCooldown);
        isProcessing = false;
    }

    private void UpdateHierarchyOrder(int selectedIndex)
    {
        // Ordem de renderização:
        // 1. Tags não selecionadas
        // 2. Box Central
        // 3. Tag selecionada
        
        // Primeiro coloca todas as tags atrás
        foreach (var tag in tags)
        {
            tag.transform.SetSiblingIndex(boxCentral.GetSiblingIndex() - 1);
        }
        
        // Depois traz a tag selecionada para frente
        tags[selectedIndex].transform.SetAsLastSibling();
        
        // Garante que a box fique atrás da tag selecionada
        boxCentral.SetSiblingIndex(tags[selectedIndex].transform.GetSiblingIndex() - 1);
    }
}