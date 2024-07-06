using RedLoader;
using SonsSdk.Attributes;
using ZombieMode.Core;

namespace ZombieMode;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static ConfigEntry<bool> HideMenuPanel { get; private set; }

    [SettingsUiIgnore]
    public static ConfigEntry<float> MasterLvl { get; private set; }
    [SettingsUiIgnore]
    public static ConfigEntry<float> MusicLvl { get; private set; }
    [SettingsUiIgnore]
    public static ConfigEntry<float> SfxLvl { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("ZombieMode", "ZombieMode", "ZombieMode.cfg");

        HideMenuPanel = Category.CreateEntry("HideMenuPanel", false, "Hide menu panel", "Hides the main menu top right panel");
        MasterLvl = Category.CreateEntry("MasterLvl", 1f, "Master level", "Global volume control of zombie mode external sounds");
        MusicLvl = Category.CreateEntry("MusicLvl", 1f, "Music level", "Music volume control of zombie mode external sounds");
        SfxLvl = Category.CreateEntry("SfxLvl", 1f, "Sfx level", "Sfx volume control of zombie mode external sounds");
    }

    public static void ApplySavedSettings()
    {
        SoundManager.OnMasterLvlChange(MasterLvl.Value);
        SoundManager.OnMusicLvlChange(MusicLvl.Value);
        SoundManager.OnSfxLvlChange(SfxLvl.Value);
    }

    public static void OnUiClose()
    {
        ZMainMenu.SonsPanel.Active(!HideMenuPanel.Value);
    }
}