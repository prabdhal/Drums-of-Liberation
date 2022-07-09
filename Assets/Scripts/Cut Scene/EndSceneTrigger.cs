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
        Debug.Log(isSceneComplete);
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

            // save player from scene position
            PlayerDataManager.FromSceneSpawnPosition = (int)fromScene;
            PlayerDataManager.CurrentScene = (int)nextScene;
            Debug.Log("Saving Current Scene as: " + PlayerDataManager.CurrentScene);

            PlayerDataManager.Instance.SaveProgress(PlayerDataManager.Instance);

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
        if (GameManager.Instance.enemies.Count <= 5)
            return true;

        Debug.Log("Enemies left: " + GameManager.Instance.Enemies.Count);
        return false;
    }
}
