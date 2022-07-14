using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject menuPanelFirstObject;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject volumePanelFirstObject;
    [SerializeField] GameObject tutorialScrollViewPanel;

    [SerializeField] Light light;
    private bool shadowOn = true;

    public bool ignore = false;
    public bool isTutorial = false;

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
        if (ignore) {
            EventSystem.current.SetSelectedGameObject(menuPanelFirstObject);
            return;
        }

        CloseAllMenus();
        shadowOn = true;

        if (eventSystem == null)
            eventSystem = FindObjectOfType<EventSystem>();
        if (tutorialScrollViewPanel == null)
            tutorialScrollViewPanel = GameObject.FindGameObjectWithTag(StringData.TutorialScrollView).GetComponent<Image>().gameObject;
        if (!isTutorial)
            tutorialScrollViewPanel.SetActive(false);
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuPanelFirstObject);
        Time.timeScale = 0f;
    }

    public void OpenOptionsButton()
    {
        CloseAllMenus();

        optionsPanel.SetActive(true);
        backgroundPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(volumePanelFirstObject);
    }

    public void BackToGameMenuButton()
    {
        CloseAllMenus();

        menuPanel.SetActive(true);
        backgroundPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuPanelFirstObject);
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
