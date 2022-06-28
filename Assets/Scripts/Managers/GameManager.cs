using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<EnemyManager> enemies = new List<EnemyManager>();
    public Dictionary<string, EnemyManager> Enemies = new Dictionary<string, EnemyManager>();

    [Header("Prefabs")]
    public GameObject damagePopPrefab;

    [SerializeField] ScreenFader screenFader;
    [SerializeField] float startNextSceneTimer = 2f;
    private float curNextSceneTimer = 0f;



    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    #endregion

    public void Start()
    {
        if (screenFader == null)
            screenFader = FindObjectOfType<ScreenFader>();

        foreach (var enemy in enemies)
        {
            Enemies.Add(enemy.name, enemy);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void AddEnemy(EnemyManager manager)
    {
        enemies.Add(manager);
        UpdateDictionary(true);
    }

    public void RemoveEnemy(EnemyManager manager)
    {
        enemies.Remove(manager);
        UpdateDictionary(false);
    }

    private void UpdateDictionary(bool add)
    {
        foreach (var enemy in enemies)
        {
            if (Enemies.ContainsKey(enemy.name)) continue;

            if (add)
                Enemies.Add(enemy.name, enemy);
            else
                Enemies.Remove(enemy.name);
        }
    }

    public void ResetScene()
    {
        screenFader.FadeToBlack();

        if (curNextSceneTimer <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else curNextSceneTimer -= Time.deltaTime;
    }
}