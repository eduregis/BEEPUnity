using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LearnDataManager : MonoBehaviour
{
    public static LearnDataManager Instance { get; private set; }

    [SerializeField] private List<LearnData> allLearnData;
    [SerializeField] private PlayerProgressSO playerProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Opcional: DontDestroyOnLoad(gameObject) se precisar persistir entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<LearnData> GetFilteredLearnData(string filter)
    {
        // Filtra por conteúdos desbloqueados
        var filteredData = allLearnData.Where(data => playerProgress.IsContentUnlocked(data.id));

        // Aplica filtro adicional se não for "All"
        if (filter != Constants.LearnTag.All.ToString())
        {
            Constants.LearnTag selectedTag = (Constants.LearnTag)System.Enum.Parse(typeof(Constants.LearnTag), filter);
            filteredData = filteredData.Where(data => data.tag == selectedTag);
        }

        return filteredData.OrderBy(data => data.title).ToList();
    }

     // Método para debug (opcional)
    public void LogUnlockedContents()
    {
        Debug.Log("Conteúdos desbloqueados:");
        foreach (var id in playerProgress.GetUnlockedContents())
        {
            Debug.Log($"- {id}");
        }
    }

    public List<string> GetAllFilterOptions()
    {
        List<string> options = new List<string> { Constants.LearnTag.All.ToString() };
        options.AddRange(System.Enum.GetNames(typeof(Constants.LearnTag)));
        return options;
    }

    // Método para buscar dados específicos (útil para outros managers)
    public LearnData GetLearnDataById(string id)
    {
        return allLearnData.FirstOrDefault(data => data.id == id);
    }
}