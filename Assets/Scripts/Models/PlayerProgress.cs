// PlayerProgressSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProgress", menuName = "Game/Player Progress")]
public class PlayerProgressSO : ScriptableObject
{
    [SerializeField] private List<string> unlockedLearnIds = new List<string>();

    private const string SAVE_KEY = "PlayerProgress";

    private void OnEnable()
    {
        LoadProgress();
    }

    // Adiciona um ID à lista de desbloqueados (evita duplicatas)
    public void UnlockLearnContent(string id)
    {
        if (!unlockedLearnIds.Contains(id))
        {
            unlockedLearnIds.Add(id);
            Debug.Log($"Conteúdo desbloqueado: {id}");
        }
    }

    // Verifica se um conteúdo está desbloqueado
    public bool IsContentUnlocked(string id)
    {
        return unlockedLearnIds.Contains(id);
    }

    // Retorna todos os IDs desbloqueados (apenas leitura)
    public IReadOnlyList<string> GetUnlockedContents()
    {
        return unlockedLearnIds.AsReadOnly();
    }

    // Limpa o progresso (útil para testes ou reset)
    public void ResetProgress()
    {
        unlockedLearnIds.Clear();
        PlayerPrefs.DeleteKey(SAVE_KEY); // Remove do PlayerPrefs
        PlayerPrefs.Save();
    }

    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(new SaveData(unlockedLearnIds));
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("Progresso salvo");
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            unlockedLearnIds = new List<string>(saveData.unlockedIds);
            Debug.Log("Progresso carregado");
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public List<string> unlockedIds;

        public SaveData(List<string> ids)
        {
            unlockedIds = ids;
        }
    }
}