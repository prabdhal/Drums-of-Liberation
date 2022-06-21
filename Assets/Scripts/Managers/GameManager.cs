using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<EnemyManager> enemies = new List<EnemyManager>();
    public Dictionary<string, EnemyManager> Enemies = new Dictionary<string, EnemyManager>();

    [Header("Prefabs")]
    public GameObject damagePopPrefab;


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
        foreach (var enemy in enemies)
        {
            Enemies.Add(enemy.name, enemy);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}