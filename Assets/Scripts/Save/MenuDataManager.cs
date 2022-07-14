using UnityEngine;

public class MenuDataManager : MonoBehaviour
{
    #region Singleton
    public static MenuDataManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one PlayerDataManager in scene");
            return;
        }
        Instance = this;
    }
    #endregion

    [Header("Audio")]
    public static float SoundVolume;
    public static float MusicVolume;
    public static bool Mute;

    [Header("Graphics")]
    public static int Shadow;



    public void SaveProgress(MenuDataManager data)
    {
        SaveSystem.SaveData(data);
    }

    public void LoadProgress()
    {
        MenuData data = SaveSystem.LoadMenuData();

        SoundVolume = data.soundValue;
        MusicVolume = data.musicValue;
        Mute = data.mute;
        Shadow = data.shadow;
    }

    public void ResetProgressButton()
    {
        SaveSystem.DeleteMenuData();

        MusicVolume = 100f;
        SoundVolume = 100f;
        Mute = false;
        //Shadow = 2;

        SaveProgress(this);
    }
}