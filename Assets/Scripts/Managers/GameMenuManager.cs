using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject graphicsPanel;

    [SerializeField] Light light;
    private bool shadowOn = true;

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

    public void GameOptionsButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
    }

    public void GameGraphicsButton()
    {
        CloseAllMenus();

        graphicsPanel.SetActive(true);
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
        graphicsPanel.SetActive(false);
        menuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
