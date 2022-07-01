using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneTrigger : MonoBehaviour
{
    [SerializeField] SceneNames nextScene;
    [SerializeField] SceneNames fromScene;

    private delegate bool IsSceneComplete();
    private IsSceneComplete isSceneComplete;
    [SerializeField] float startSceneLoadTimer = 2f;
    private float curSceneLoadTimer;
    private bool initiateNextScene = false;

    private void Start()
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

            PlayerDataManager.Instance.SaveProgress(PlayerDataManager.Instance);

            EndScene();
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
