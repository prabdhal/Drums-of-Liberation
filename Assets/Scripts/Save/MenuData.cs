[System.Serializable]
public class MenuData
{
    public float soundValue;
    public float musicValue;
    public bool mute;
    public int shadow;

    public MenuData(MenuDataManager menuDataManager)
    {
        soundValue = MenuDataManager.SoundVolume;
        musicValue = MenuDataManager.MusicVolume;
        mute = MenuDataManager.Mute;
        shadow = MenuDataManager.Shadow;
    }
}
