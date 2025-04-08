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
        if (filter == Constants.LearnTag.All.ToString())
        {
            return allLearnData.OrderBy(data => data.title).ToList();
        }

        Constants.LearnTag selectedTag = (Constants.LearnTag)System.Enum.Parse(typeof(Constants.LearnTag), filter);
        return allLearnData
            .Where(data => data.tag == selectedTag)
            .OrderBy(data => data.title)
            .ToList();
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