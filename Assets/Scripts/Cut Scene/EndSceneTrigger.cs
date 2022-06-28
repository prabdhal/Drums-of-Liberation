using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneTrigger : MonoBehaviour
{
    [SerializeField] SceneNames nextScene;

    private delegate bool IsSceneComplete();
    private IsSceneComplete isSceneComplete;

    private void Start()
    {
        isSceneComplete += SceneCompletionConditions;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            EndScene();
        }
    }

    public void EndScene()
    {
        if (isSceneComplete())
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
