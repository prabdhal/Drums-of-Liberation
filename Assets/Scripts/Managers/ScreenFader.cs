using UnityEngine;
using UnityEngine.Playables;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject blackScreen;


    [SerializeField] PlayableDirector director;
    [SerializeField] float timeBeforeEndOfTimeline = 4f;

    [SerializeField] EndSceneTrigger endSceneTrigger;

    [SerializeField] bool hasDirector;



    private void Start()
    {
        anim = GetComponent<Animator>();
        if (hasDirector)
            director = FindObjectOfType<PlayableDirector>();

        blackScreen.SetActive(true);
        FadeToScreen();
    }

    private void Update()
    {
        if (hasDirector)
        {
            Debug.Log("duration: " + director.duration);
            Debug.Log("time: " + director.time);
            if (director.duration - director.time <= timeBeforeEndOfTimeline)
                FadeToBlack();
            if (director.duration - director.time <= 0.1f)
            {
                Debug.Log("Get next scene");
                endSceneTrigger.EndScene();
            }

        }
    }

    public void FadeToBlack()
    {
        anim.Play(StringData.BlackScreen);
    }

    public void FadeToScreen()
    {
        anim.Play(StringData.ClearScreen);
    }

}
