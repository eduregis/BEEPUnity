using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LearnDataManager : MonoBehaviour
{
    public static LearnDataManager Instance { get; private set; }

    [SerializeField] private LearnData[] allLearnData;

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
        if (filter == "All")
        {
            return allLearnData.OrderBy(data => data.title).ToList();
        }

        LearnData.Tag selectedTag = (LearnData.Tag)System.Enum.Parse(typeof(LearnData.Tag), filter);
        return allLearnData
            .Where(data => data.tag == selectedTag)
            .OrderBy(data => data.title)
            .ToList();
    }

    public List<string> GetAllFilterOptions()
    {
        List<string> options = new List<string> { "All" };
        options.AddRange(System.Enum.GetNames(typeof(LearnData.Tag)));
        return options;
    }

    // Método para buscar dados específicos (útil para outros managers)
    public LearnData GetLearnDataById(string id)
    {
        return allLearnData.FirstOrDefault(data => data.id == id);
    }
}