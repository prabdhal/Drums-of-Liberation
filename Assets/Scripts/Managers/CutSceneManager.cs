using UnityEngine;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] float timeBeforeEndOfTimeline = 4f;
    [SerializeField] EndSceneTrigger endSceneTrigger;

    private void Start()
    {
        director = FindObjectOfType<PlayableDirector>();
    }

    void Update()
    {
        if (director.duration - director.time <= timeBeforeEndOfTimeline)
            SkipButton();
    }

    public void SkipButton()
    {
        endSceneTrigger.EndScene();
    }
}
