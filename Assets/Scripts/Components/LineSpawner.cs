using UnityEngine;
using System.Collections.Generic;

public class LineSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private int maxLines = 20;
    [SerializeField] private float lineWidth = 800f;
    
    [Header("Position Settings")]
    [SerializeField] private float startX = 0f;
    [SerializeField] private float startY = -200f;
    
    private Canvas canvas;
    private List<GameObject> activeLines = new List<GameObject>();
    private float timer;
    
    private void Start()
    {
        // Busca o Canvas no pai ou na cena
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            // Usando o novo método recomendado
            canvas = FindAnyObjectByType<Canvas>();
            
            if (canvas == null)
            {
                Debug.LogError("Nenhum Canvas encontrado na cena!");
                enabled = false;
                return;
            }
        }
        
        timer = spawnInterval; // Spawna a primeira linha imediatamente
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval && activeLines.Count < maxLines)
        {
            SpawnLine();
            timer = 0f;
        }
        
        // Remove linhas destruídas da lista
        activeLines.RemoveAll(item => item == null);
    }
    
    private void SpawnLine()
    {
        GameObject newLine = Instantiate(linePrefab, canvas.transform);
        RectTransform rt = newLine.GetComponent<RectTransform>();
        
        rt.anchoredPosition = new Vector2(startX, startY);
        rt.sizeDelta = new Vector2(lineWidth, rt.sizeDelta.y);
        
        // Configura a posição inicial corretamente
        MovingLine movingLine = newLine.GetComponent<MovingLine>();
        if (movingLine != null)
        {
            movingLine.SetInitialPosition(rt.anchoredPosition);
        }
        
        activeLines.Add(newLine);
    }
}