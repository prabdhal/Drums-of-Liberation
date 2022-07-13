[System.Serializable]
public class PlayerData
{
    public bool tutorialFinished;

    public int playerLevel;
    public float currentHealth;
    public float currentMana;
    public float currentStamina;
    public float playerXP;

    public float playerGold;
    public int fullRestorePotionCount;

    public int fromSceneSpawnPosition;
    public int currentScene;

    public PlayerData(PlayerDataManager playerDataManager)
    {
        tutorialFinished = PlayerDataManager.TutorialFinished;

        playerLevel = PlayerDataManager.PlayerLevel;
        currentHealth = PlayerDataManager.CurrentHealth;
        currentMana = PlayerDataManager.CurrentMana;
        currentStamina = PlayerDataManager.CurrentStamina;
        playerXP = PlayerDataManager.PlayerXP;

        playerGold = PlayerDataManager.PlayerGold;
        fullRestorePotionCount = PlayerDataManager.FullRestorePotionCount;

        fromSceneSpawnPosition = PlayerDataManager.FromSceneSpawnPosition;
        currentScene = PlayerDataManager.CurrentScene;
    }
}
