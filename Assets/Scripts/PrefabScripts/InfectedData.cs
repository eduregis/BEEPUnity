using UnityEngine;

public class InfectedData : MonoBehaviour
{
    private Animator animator;
    public Vector2Int gridPosition;
    public bool isInfected = true;

    private void Awake()
    {
        // Obtém o componente Animator no Awake para garantir que ele seja atribuído corretamente
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no objeto!");
        }
    }
    public void Initialize(Vector2Int position)
    {
        gridPosition = position;

        int depth = Utils.CalculateIsoDepth(position, 1);
        transform.SetSiblingIndex(depth);

        GetComponent<RectTransform>().anchoredPosition =
        IsometricMapGenerator.Instance.GetTileRect(position)?.anchoredPosition +
        Vector3.up * 40f ?? Vector2.zero;
    }

    public void RecoveringData()
    {
        isInfected = false;
        animator.SetBool("isInfected", false);

        // Força atualização imediata (opcional, mas recomendado para mudança brusca de estado)
        animator.Update(0);
    }
}