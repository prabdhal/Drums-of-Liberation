[System.Serializable]
public class PlayerData
{
    public bool tutorialFinished;
    public bool resetGame;

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

        fromSceneSpawnPosition = PlayerDataManager.FromSceneSpawnPosition;
        currentScene = PlayerDataManager.CurrentScene;
    }
}
