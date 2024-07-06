using Sons.Settings;

namespace ZombieMode.Core;

public class SoundManager
{
    public static SonsFMODEventEmitter musicEmitter;
    public static float MasterLvl = 1;
    public static float MusicLvl = 1;
    public static float SfxLvl = 1;

    public static void Init()
    {
        musicEmitter = new();
    }

    public static void OnMasterLvlChange(float value)
    {
        Config.MasterLvl.Value = value;
        if (musicEmitter.instance.isValid())
        {
            musicEmitter.instance.setVolume(value * 0.5f * AudioSettings._masterVolume * AudioSettings._musicVolume * MusicLvl);
        }
        MasterLvl = value;
    }

    public static void OnMusicLvlChange(float value)
    {
        Config.MusicLvl.Value = value;
        if (musicEmitter.instance.isValid())
        {
            musicEmitter.instance.setVolume(value * 0.5f * AudioSettings._musicVolume * AudioSettings._masterVolume * MasterLvl);
        }
        MusicLvl = value;
    }

    public static void OnSfxLvlChange(float value)
    {
        Config.SfxLvl.Value = value;
        SfxLvl = value;
    }
}
