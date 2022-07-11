using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject optionsPanel;

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
        SaveMenuSettings();
        ResetGameProgress();
        GameManager.Instance.LoadScene(StringData.CutSceneOne);
    }

    public void ContinueGameButton()
    {
        SaveMenuSettings();
        PlayerDataManager.Instance.LoadProgress();
        MenuDataManager.Instance.LoadProgress();
        GameManager.Instance.LoadScene(((SceneNames)PlayerDataManager.CurrentScene).ToString());
    }

    public void OpenOptionsPanelButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void OpenSoundsPanelButton()
    {
        CloseAllMenus();

        backgroundPanel.SetActive(true);
    }

    public void OpenGraphicsPanelButton()
    {
        CloseAllMenus();

        backgroundPanel.SetActive(true);
    }

    public void BackToMainMenuButton()
    {
        CloseAllMenus();

        mainMenuPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void BackToOptionsButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    private void ResetGameProgress()
    {
        SaveSystem.DeletePlayerData();
        PlayerDataManager.Instance.ResetProgressButton();
    }

    private void SaveMenuSettings()
    {
        MenuDataManager.MusicVolume = AudioManager.Instance.Volume;
        //MenuDataManager.SoundValue = AudioManager.Instance.Sound;
        MenuDataManager.Mute = AudioManager.Instance.Mute;
        MenuDataManager.Shadow = AudioManager.Instance.Shadow;

        MenuDataManager.Instance.SaveProgress(MenuDataManager.Instance);
    }

    public void BackToMainMenu()
    {
        SaveMenuSettings();
        GameManager.Instance.LoadScene(SceneNames.MainMenuScene.ToString());
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
