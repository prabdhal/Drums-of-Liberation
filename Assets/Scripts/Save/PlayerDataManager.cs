using UnityEngine;

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

    [Header("Main")]
    public static bool TutorialFinished;

    [Header("Player Stats")]
    public static int PlayerLevel;
    public static float CurrentHealth;
    public static float CurrentMana;
    public static float CurrentStamina;
    public static float PlayerXP;

    [Header("Player Inventory")]
    public static float PlayerGold;
    public static int FullRestorePotionCount;

    [Header("Scene Information")]
    public static int FromSceneSpawnPosition;
    public static int CurrentScene;



    public void SaveProgress(PlayerDataManager data)
    {
        SaveSystem.SaveData(data);
    }

    public void LoadProgress()
    {
        PlayerData data = SaveSystem.LoadPlayerData();

        TutorialFinished = data.tutorialFinished;

        PlayerLevel = data.playerLevel;
        CurrentHealth = data.currentHealth;
        CurrentMana = data.currentMana;
        CurrentStamina = data.currentStamina;

        FromSceneSpawnPosition = data.fromSceneSpawnPosition;
        CurrentScene = data.currentScene;
    }

    public void ResetProgressButton()
    {
        SaveSystem.DeletePlayerData();

        TutorialFinished = false;

        PlayerLevel = 0;
        CurrentHealth = 500f;
        CurrentMana = 100f;
        CurrentStamina = 100f;

        SaveProgress(this);
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