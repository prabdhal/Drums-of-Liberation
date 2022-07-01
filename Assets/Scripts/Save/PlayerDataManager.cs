using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    #region Singleton
    public static PlayerDataManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one PlayerDataManager in scene");
            return;
        }
        Instance = this;
    }
    #endregion

    MainMenuManager main;

    public static bool TutorialFinished;
    public static bool ResetGame;

    [Header("Player Stats")]
    public static int PlayerLevel;
    public static float CurrentHealth;
    public static float CurrentMana;
    public static float CurrentStamina;

    [Header("Scene Information")]
    public static int FromSceneSpawnPosition;
    public static int CurrentScene;

    [Header("Audio")]
    [Range(0, 1)]
    public static float SoundValue;
    [Range(0, 1)]
    public static float MusicValue;


    void Start()
    {
        main = MainMenuManager.Instance;
    }

    public void SaveProgress(PlayerDataManager data)
    {
        SaveSystem.SaveData(data);
    }

    public void LoadProgress()
    {
        PlayerData data = SaveSystem.LoadData();

        TutorialFinished = data.tutorialFinished;
        ResetGame = data.resetGame;

        SoundValue = data.soundValue;
        MusicValue = data.musicValue;

        PlayerLevel = data.playerLevel;
        CurrentHealth = data.currentHealth;
        CurrentMana = data.currentMana;
        CurrentStamina = data.currentStamina;

        FromSceneSpawnPosition = data.fromSceneSpawnPosition;
        CurrentScene = data.currentScene;
    }

    public void ResetProgressButton()
    {
        SaveSystem.DeleteData();

        TutorialFinished = false;

        SoundValue = 1f;
        MusicValue = 1f;

        PlayerLevel = 0;
        CurrentHealth = 100f;
        CurrentMana = 100f;
        CurrentStamina = 100f;
        Debug.Log("Game has been reset: Current HP: " + CurrentHealth);

        SaveProgress(this);
    }

    public void UpdateCurrency()
    {
    }

    public void OnApplicationPause(bool pause)
    {
        //if (pause)
            //SaveProgress(this);
    }

    public void OnApplicationQuit()
    {
        //SaveProgress(this);
    }
}