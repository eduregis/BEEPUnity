using UnityEngine;
using System.Collections.Generic;

public class TagManager : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private RectTransform boxCentral;
    [SerializeField] private List<LearnTag> tags = new List<LearnTag>();
    
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
            LearnDataManager.Instance.GetFilteredLearnData("All")
        );
    }

    private void OnTagClicked(int index)
    {
        if (isProcessing || currentSelectedIndex == index) return;
        
        StartCoroutine(ProcessTagSelection(index));
    }

    private System.Collections.IEnumerator ProcessTagSelection(int index)
    {
        isProcessing = true;
        
        // Seleciona a nova tag
        currentSelectedIndex = index;
        
        // Ajusta a ordem hierárquica
        UpdateHierarchyOrder(index);
        
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