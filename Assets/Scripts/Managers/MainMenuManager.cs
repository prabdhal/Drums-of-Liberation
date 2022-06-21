using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
  [SerializeField]
  private GameObject mainMenuPanel;
  [SerializeField]
  private GameObject optionsPanel;

  private void Start()
  {
    mainMenuPanel.SetActive(true);
    optionsPanel.SetActive(false);
  }

  public void NewGameButton()
  {
    GameManager.Instance.LoadScene(StringData.CutSceneOne);
  }

  public void ContinueGameButton()
  {
    GameManager.Instance.LoadScene(StringData.CutSceneOne);
  }

  public void OptionsButton()
  {
    optionsPanel.SetActive(true);
    mainMenuPanel.SetActive(false);
  }

  public void BackButton()
  {
    mainMenuPanel.SetActive(true);
    optionsPanel.SetActive(false);
  }

  public void QuitButton()
  {
    Application.Quit();
  }
}
