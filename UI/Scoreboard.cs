using SonsSdk;
using SUI;
using UnityEngine;
using ZombieMode.Gameplay;
using static SUI.SUI;

namespace ZombieMode.UI;

public class Scoreboard
{
    public const string SCOREBOARD_ID = "ZombieModeScoreboard";

    public static void UiCreate()
    {
        var scoreboard = RegisterNewPanel(SCOREBOARD_ID)
                .Dock(EDockType.Center).Size(800, 300).Background(Color.black.WithAlpha(0.98f), EBackground.RoundedStandard).Vertical(0, "EC").Active(false);

        var scoreboardTitleContainer = SContainer.Dock(EDockType.Fill).Background(new Color(0.06f, 0.06f, 0.06f, 0.8f), EBackground.RoundedStandard).PaddingVertical(10).PHeight(50);
        scoreboardTitleContainer.Add(SLabel.Text("Scoreboard").Dock(EDockType.Fill).FontAutoSize(true).FontSpacing(8.5f).FontColor(new Color(0.72f, 0.25f, 0.25f)));
        scoreboard.Add(scoreboardTitleContainer);

        var scoreboardContainer = SContainer.Dock(EDockType.Fill).Background(new Color(0.06f, 0.06f, 0.06f, 0.5f), EBackground.RoundedStandard).Horizontal();
        scoreboardContainer.Add(SLabel.Bind(ScoreSystem.StringScore));
        scoreboardContainer.Add(SLabel.Bind(ScoreSystem.StringPlayerKills));
        scoreboard.Add(scoreboardContainer);
    }

    public static void Show(bool onoff)
    {
        TogglePanel(SCOREBOARD_ID, onoff);
    }
}
