using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject backgroundPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private GameObject optionsPanel;

    #region Singleton
    public static GameMenuManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #endregion


    private void Start()
    {
        CloseAllMenus();
    }

    public void ResumeGame()
    {
        CloseAllMenus();

        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        CloseAllMenus();

        menuPanel.SetActive(true);
        backgroundPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameOptionsButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void BackGameMenuButton()
    {
        CloseAllMenus();

        menuPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.LoadScene(StringData.MainMenuScene);
    }

    public void CloseAllMenus()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
