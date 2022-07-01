using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject backgroundPanel;
    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private GameObject optionsPanel;

    #region Singleton
    public static MainMenuManager Instance;

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
        Time.timeScale = 1f;

        mainMenuPanel.SetActive(true);
    }

    public void NewGameButton()
    {
        ResetGameProgress();
        GameManager.Instance.LoadScene(StringData.CutSceneOne);
        Debug.Log("New Game Button");
    }

    public void ContinueGameButton()
    {
        SaveSystem.LoadData();
        GameManager.Instance.LoadScene(StringData.CutSceneOne);
    }

    public void MainOptionsButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void BackMainMenuButton()
    {
        CloseAllMenus();

        mainMenuPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    private void ResetGameProgress()
    {
        SaveSystem.DeleteData();
        PlayerDataManager.Instance.ResetProgressButton();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void CloseAllMenus()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }
}
