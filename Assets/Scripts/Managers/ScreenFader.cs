using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject blackScreen;


    #region Singleton
    public static ScreenFader Instance;
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
        anim = GetComponent<Animator>();
        blackScreen.SetActive(true);
        FadeToScreen();
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
