using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject newGameFirstGameObjectButton;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject volumeSliderFirstGameObject;
    [SerializeField] GameObject optionsPanel;

    [SerializeField] GameObject confirmationPanel;
    [SerializeField] GameObject confirmationPanelFirstGameObject;

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

        BackToMainMenuButton();
    }

    public void NewGameButton()
    {
        SaveMenuSettings();
        ResetGameProgress();
        GameManager.Instance.LoadScene(StringData.CutSceneOne);
    }

    public void OpenConfirmationWindow()
    {
        CloseAllMenus();

        confirmationPanel.SetActive(true);
        backgroundPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(confirmationPanelFirstGameObject);
    }

    public void ContinueGameButton()
    {
        SaveMenuSettings();
        if (!PlayerDataManager.Instance.LoadProgress()) NewGameButton();
        MenuDataManager.Instance.LoadProgress();
        GameManager.Instance.LoadScene(((SceneNames)PlayerDataManager.CurrentScene).ToString());
    }

    public void OpenOptionsPanelButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(volumeSliderFirstGameObject);
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameFirstGameObjectButton);
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
        SaveSystem.DeleteMenuData();
        PlayerDataManager.Instance.ResetProgressButton();
        MenuDataManager.Instance.ResetProgressButton();
    }

    private void SaveMenuSettings()
    {
        MenuDataManager.MusicVolume = AudioManager.Instance.MusicVolume;
        //MenuDataManager.SoundValue = AudioManager.Instance.Sound;
        MenuDataManager.Mute = AudioManager.Instance.Mute;
        //MenuDataManager.Shadow = AudioManager.Instance.Shadow;

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
        confirmationPanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }
}
