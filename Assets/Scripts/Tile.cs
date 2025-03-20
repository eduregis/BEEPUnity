using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Image lightCenter; // Imagem central do tile (opcional)
    public Image lightTop;    // Imagem decorativa para o topo
    public Image lightDown;   // Imagem decorativa para a base
    public Image lightLeft;   // Imagem decorativa para a esquerda
    public Image lightRight;  // Imagem decorativa para a direita

    private int[,] mapMatrix; // Referência para a matriz do mapa
    private int x, y;         // Posição do tile na matriz

    // Cores para a pulsação
    private Color color1 = new Color(0.89f, 0.20f, 0.67f); // #e332aa
    private Color color2 = new Color(0.08f, 0.47f, 0.85f); // #1479da

    private float pulseSpeed = 2f; // Velocidade da pulsação
    private float timeOffset;      // Offset de tempo para variar o efeito entre os tiles

    // Método para inicializar o tile com a matriz e sua posição
    public void Initialize(int[,] map, int posX, int posY)
    {
        mapMatrix = map;
        x = posX;
        y = posY;

        // Define um offset de tempo aleatório para variar o efeito de pulsação
        timeOffset = Random.Range(0f, 2f * Mathf.PI);

        ApplyDecorativeRules();
    }

    // Aplica as regras decorativas com base nos vizinhos
    private void ApplyDecorativeRules()
    {
        int mapHeight = mapMatrix.GetLength(0);
        int mapWidth = mapMatrix.GetLength(1);

        // Verifica o vizinho acima (i, j - 1)
        if (y > 0 && mapMatrix[y - 1, x] == 1)
        {
            lightTop.gameObject.SetActive(true);
        }
        else
        {
            lightTop.gameObject.SetActive(false);
        }

        // Verifica o vizinho abaixo (i, j + 1)
        if (y < mapHeight - 1 && mapMatrix[y + 1, x] == 1)
        {
            lightDown.gameObject.SetActive(true);
        }
        else
        {
            lightDown.gameObject.SetActive(false);
        }

        // Verifica o vizinho à esquerda (i - 1, j)
        if (x > 0 && mapMatrix[y, x - 1] == 1)
        {
            lightLeft.gameObject.SetActive(true);
        }
        else
        {
            lightLeft.gameObject.SetActive(false);
        }

        // Verifica o vizinho à direita (i + 1, j)
        if (x < mapWidth - 1 && mapMatrix[y, x + 1] == 1)
        {
            lightRight.gameObject.SetActive(true);
        }
        else
        {
            lightRight.gameObject.SetActive(false);
        }
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        // Calcula a pulsação usando uma função senoidal
        float t = Mathf.Sin(Time.time * pulseSpeed + timeOffset) * 0.5f + 0.5f;
        Color currentColor = Color.Lerp(color1, color2, t);

        // Aplica a cor aos assets decorativos ativos
        if (lightCenter.gameObject.activeSelf)
            lightCenter.color = currentColor;
        if (lightTop.gameObject.activeSelf)
            lightTop.color = currentColor;
        if (lightDown.gameObject.activeSelf)
            lightDown.color = currentColor;
        if (lightLeft.gameObject.activeSelf)
            lightLeft.color = currentColor;
        if (lightRight.gameObject.activeSelf)
            lightRight.color = currentColor;
    }
}