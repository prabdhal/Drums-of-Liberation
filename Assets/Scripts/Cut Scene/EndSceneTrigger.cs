using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneTrigger : MonoBehaviour
{
    [SerializeField] SceneNames nextScene;
    [SerializeField] SceneNames fromScene;

    public delegate bool IsSceneComplete();
    public IsSceneComplete isSceneComplete;
    [SerializeField] float startSceneLoadTimer = 2f;
    private float curSceneLoadTimer;
    public bool initiateNextScene = false;

    private void Awake()
    {
        isSceneComplete += SceneCompletionConditions;
        curSceneLoadTimer = startSceneLoadTimer; 
        initiateNextScene = false;
    }

    private void Update()
    {
        if (initiateNextScene)
        {
            if (curSceneLoadTimer <= 0)
            {
                GetNextScene();
                initiateNextScene = false;
            }
            else
                curSceneLoadTimer -= Time.deltaTime;
        }
        Debug.Log(isSceneComplete());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            // save player stats
            PlayerDataManager.PlayerLevel = PlayerManager.Instance.Stats.playerLevel;
            PlayerDataManager.CurrentHealth = PlayerManager.Instance.Stats.CurrentHealth;
            PlayerDataManager.CurrentMana = PlayerManager.Instance.Stats.CurrentMana;
            PlayerDataManager.CurrentStamina = PlayerManager.Instance.Stats.CurrentStamina;
            PlayerDataManager.PlayerXP = PlayerManager.Instance.Stats.playerXP;

            // save player from scene position
            PlayerDataManager.FromSceneSpawnPosition = (int)fromScene;
            PlayerDataManager.CurrentScene = (int)nextScene;
            Debug.Log("Saving Current Scene as: " + PlayerDataManager.CurrentScene);

            // save player inventory
            PlayerDataManager.PlayerGold = PlayerManager.Instance.Gold;
            PlayerDataManager.FullRestorePotionCount = PlayerInventory.Instance.curFullRestoreCount;

            // save sound settings
            MenuDataManager.MusicVolume = AudioManager.Instance.MusicVolume;
            MenuDataManager.SoundVolume = AudioManager.Instance.SoundVolume;
            MenuDataManager.Mute = AudioManager.Instance.Mute;
            //MenuDataManager.Shadow = AudioManager.Instance.Shadow;


            PlayerDataManager.Instance.SaveProgress(PlayerDataManager.Instance);
            MenuDataManager.Instance.SaveProgress(MenuDataManager.Instance);

            EndScene();
            PlayerControls.Instance.FullPlayerControl(false);
        }
    }

    public void EndScene()
    {
        if (isSceneComplete())
        {
            initiateNextScene = true;
            ScreenFader.Instance.FadeToBlack();
        }
    }

    private void GetNextScene()
    {
        SceneManager.LoadScene(nextScene.ToString());
    }

    protected virtual bool SceneCompletionConditions()
    {
        return true;
    }
}
