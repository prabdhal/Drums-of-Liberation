[System.Serializable]
public class PlayerData
{
    public bool tutorialFinished;
    public bool resetGame;

    public int playerLevel;
    public float currentHealth;
    public float currentMana;
    public float currentStamina;

    public int fromSceneSpawnPosition;
    public int currentScene;

    public float soundValue;
    public float musicValue;


    public PlayerData(PlayerDataManager playerDataManager)
    {
        tutorialFinished = PlayerDataManager.TutorialFinished;
        resetGame = PlayerDataManager.ResetGame;

        soundValue = PlayerDataManager.SoundValue;
        musicValue = PlayerDataManager.MusicValue;

        playerLevel = PlayerDataManager.PlayerLevel;
        currentHealth = PlayerDataManager.CurrentHealth;
        currentMana = PlayerDataManager.CurrentMana;
        currentStamina = PlayerDataManager.CurrentStamina;

        fromSceneSpawnPosition = PlayerDataManager.FromSceneSpawnPosition;
        currentScene = PlayerDataManager.CurrentScene;
    }
}
