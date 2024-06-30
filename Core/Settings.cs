using SUI;
using TMPro;
using UnityEngine;
using static ZombieMode.UI.AXSUI;

namespace ZombieMode.Core;

public class Settings
{
    public static SContainerOptions SettingsPanel;
    static readonly string _dividerColor = "#9A0002";

    public static void UiCreate()
    {
        SettingsPanel = AxCreateSidePanel("SettingsPanel", false, "Settings", Side.Right, 800, new Color32(22, 22, 22, 255), EBackground.None, false)
            .OverrideSorting(250).Active(false);

        AxGetMenuTitle((SPanelOptions)SettingsPanel).FontColor(new Color(0.51f, 0.03f, 0.03f)).FontStyle(FontStyles.UpperCase);

        var main = AxGetMainContainer((SPanelOptions)SettingsPanel).Background(new Color(0.04f, 0.04f, 0.04f), EBackground.RoundSmall);

        main.Add(AxDivider(DividerText("Audio")));
        main.Add(AxMenuSliderFloat("Master", LabelPosition.Top, 0, 2, 1, 0.1f, SoundManager.OnMasterLvlChange));
        main.Add(AxMenuSliderFloat("Music", LabelPosition.Top, 0, 2, 1, 0.1f, SoundManager.OnMusicLvlChange));
        main.Add(AxMenuSliderFloat("Sfx", LabelPosition.Top, 0, 2, 1, 0.1f, SoundManager.OnSfxLvlChange));
    }

    public static void ShowSettings(bool onoff)
    {
        SettingsPanel.Active(onoff);
    }

    private static string DividerText(string text)
    {
        return $"<color={_dividerColor}>{text}</color>";
    }
}
