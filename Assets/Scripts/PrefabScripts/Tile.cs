using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Image fittingBox;  // Imagem do encaixe da caixa
    public Image lightCenter; // Imagem central do tile (opcional)
    public Image lightTop;    // Imagem decorativa para o topo
    public Image lightDown;   // Imagem decorativa para a base
    public Image lightLeft;   // Imagem decorativa para a esquerda
    public Image lightRight;  // Imagem decorativa para a direita

    private int[,] mapMatrix; // Referência para a matriz do mapa
    public int x { get; private set; }
    public int y { get; private set; }

    public bool concluded = false;

    // Cores para a pulsação
    private Color color1 = HexColorUtility.HexToColor("#e332aa");
    private Color color2 = HexColorUtility.HexToColor("#1479da");
    private Color colorConcluded = HexColorUtility.HexToColor("#aedb16");

    private float pulseSpeed = 2f; // Velocidade da pulsação
    private float timeOffset;      // Offset de tempo para variar o efeito entre os tiles

    // Método para inicializar o tile com a matriz e sua posição
    public void Initialize(int[,] map, int posX, int posY, bool hasFittingBox = false)
    {
        mapMatrix = map;
        x = posX;
        y = posY;
        fittingBox.gameObject.SetActive(hasFittingBox);

        Vector2Int position = new Vector2Int(x, y);
        Utils.ApplyIsoDepth(gameObject, position, 0);

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
        if (y > 0 && mapMatrix[y - 1, x] != (int)Constants.TileType.Empty)
        {
            lightTop.gameObject.SetActive(true);
        }
        else
        {
            lightTop.gameObject.SetActive(false);
        }

        // Verifica o vizinho abaixo (i, j + 1)
        if (y < mapHeight - 1 && mapMatrix[y + 1, x] != (int)Constants.TileType.Empty)
        {
            lightDown.gameObject.SetActive(true);
        }
        else
        {
            lightDown.gameObject.SetActive(false);
        }

        // Verifica o vizinho à esquerda (i - 1, j)
        if (x > 0 && mapMatrix[y, x - 1] != (int)Constants.TileType.Empty)
        {
            lightLeft.gameObject.SetActive(true);
        }
        else
        {
            lightLeft.gameObject.SetActive(false);
        }

        // Verifica o vizinho à direita (i + 1, j)
        if (x < mapWidth - 1 && mapMatrix[y, x + 1] != (int)Constants.TileType.Empty)
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
        Color currentColor = colorConcluded;
        if (!concluded)
        {
            float t = Mathf.Sin(Time.time * pulseSpeed + timeOffset) * 0.5f + 0.5f;
            currentColor = Color.Lerp(color1, color2, t);
        }

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