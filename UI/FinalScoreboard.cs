using SonsSdk;
using Steamworks;
using SUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using ZombieMode.Gameplay;
using ZombieMode.Helpers;
using static SUI.SUI;

namespace ZombieMode.UI;

public class FinalScoreboard
{
    public const string GAME_FINAL_SCOREBOARD = "ZombieModeFinalScoreboard";

    public static void UiDestroy()
    {
        RemovePanel(GAME_FINAL_SCOREBOARD);
    }

    public static void UiCreate()
    {
        var panel = RegisterNewPanel(GAME_FINAL_SCOREBOARD)
            .Dock(EDockType.Fill).Margin(150, 150, 150, 150).Background(new Color(0.1f, 0.1f, 0.1f, 0.9f), EBackground.RoundedStandard).Vertical(10, "EC").Padding(2)
            .OverrideSorting(999);

        var titleContainer = SContainer.Background(new Color(0.06f, 0.06f, 0.06f, 0.8f), EBackground.RoundedStandard).PaddingVertical(10).PHeight(70);
        titleContainer.Add(SLabel.Text("Scoreboard").Dock(EDockType.Fill).FontSize(60).FontSpacing(8.5f).FontColor(new Color(0.72f, 0.25f, 0.25f)));
        panel.Add(titleContainer);

        var scrollbar = SDiv.FlexHeight(1);
        scrollbar.SetParent(panel);

        var scroll = SScrollContainer
        .Dock(EDockType.Fill)
        .Background(Color.black.WithAlpha(0), EBackground.RoundedStandard)
        .Size(-20, -20)
        .As<SScrollContainerOptions>();
        scroll.ContainerObject.Spacing(10);
        scroll.ContainerObject.PaddingHorizontal(10);
        scroll.SetParent(scrollbar);

        scroll.Add(SLabel.RichText($"Rounds survived <color=red>{Game.Round.Value}</color>").FontSize(60).Alignment(TextAlignmentOptions.Center).Margin(10, 0, 0, 0));

        scroll.Add(Divider("Score"));
        scroll.Add(SLabel.RichText($"{"Total score"} <color=orange>{ScoreSystem.Score.Value}</color>$").Alignment(TextAlignmentOptions.Left).Margin(10, 0, 0, 0));

        scroll.Add(Divider("Kills"));
        scroll.Add(SLabel.RichText($"{"Player kills"} <color=#00ffffff>{ScoreSystem.PlayerKills}</color>").Alignment(TextAlignmentOptions.Left).Margin(10, 0, 0, 0));

        panel.Active(false);
    }

    private static SLabelDividerOptions Divider(string text)
    {
        return SLabelDivider.Text(text);
    }
}
