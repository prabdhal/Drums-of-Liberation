using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject optionsPanel;

    [SerializeField] Light light;
    private bool shadowOn = true;

    public bool ignore = false;

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
        if (ignore) return;
        CloseAllMenus();
        shadowOn = true;
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

    public void OpenOptionsButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void BackToGameMenuButton()
    {
        CloseAllMenus();

        menuPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.LoadScene(StringData.MainMenuScene);
    }

    public void ShadowButton()
    {
        shadowOn = !shadowOn;

        if (shadowOn)
            light.shadows = LightShadows.Soft;
        else
            light.shadows = LightShadows.None;
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
