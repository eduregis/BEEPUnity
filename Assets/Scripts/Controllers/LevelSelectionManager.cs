using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonState
    {
        public Sprite unavailableSprite;
        public Sprite availableSprite;
        public Sprite concludedSprite;
    }

    [SerializeField] private ButtonState buttonStates;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private int totalLevels = 12;

    private void Start()
    {
        if (AppSettings.HighestUnlockedLevel == 0)
        {
            CanvasFadeController.Instance.ShowDialogue("Intro");
            AppSettings.HighestUnlockedLevel = 1;
        }
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonsContainer);
            
            // Certifique-se de pegar o componente TMP_Text corretamente
            TMP_Text levelText = buttonObj.GetComponentInChildren<TMP_Text>(true);
            
            if (levelText != null)
            {
                levelText.text = i.ToString();
            }
            else
            {
                Debug.LogError($"Nenhum componente TMP_Text encontrado no botão {i}");
            }

            Button button = buttonObj.GetComponent<Button>();
            LevelState state = GetLevelState(i);
            SetupButtonAppearance(button, levelText, state, i);
        }
    }

    private LevelState GetLevelState(int levelIndex)
    {
        // Lógica para determinar o estado do nível
        int highestUnlocked = AppSettings.HighestUnlockedLevel;

        if (levelIndex > highestUnlocked) return LevelState.Unavailable;
        if (levelIndex == highestUnlocked) return LevelState.Available;
        return LevelState.Concluded;
    }

    private void SetupButtonAppearance(Button button, TMP_Text text, LevelState state, int levelIndex)
    {
        switch (state)
        {
            case LevelState.Unavailable:
                button.image.sprite = buttonStates.unavailableSprite;
                button.interactable = false;
                break;
            case LevelState.Available:
                button.image.sprite = buttonStates.availableSprite;
                button.interactable = true;
                break;
            case LevelState.Concluded:
                button.image.sprite = buttonStates.concludedSprite;
                button.interactable = true;
                break;
        }

        button.onClick.AddListener(() => OnLevelSelected(levelIndex));
    }

    private void OnLevelSelected(int levelIndex)
    {
        AudioManager.Instance.Play("chooseLevel");
        AppSettings.CurrentLevel = levelIndex;
        SceneManager.LoadScene("GameplayScene");
    }

    private enum LevelState
    {
        Unavailable,
        Available,
        Concluded
    }
}