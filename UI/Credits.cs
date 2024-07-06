using SonsSdk;
using SUI;
using System.Reflection;
using TMPro;
using UnityEngine;
using ZombieMode.Libs;
using static SUI.SUI;

namespace ZombieMode.UI;

public class Credits
{
    public const string MAIN_MENU_CREDITS = "ZombieModeCredits";

    public static SContainerOptions UiCreate()
    {
        var creditsPanel = RegisterNewPanel(MAIN_MENU_CREDITS)
                .Dock(EDockType.Fill).Margin(0, 0, 0, 0).Background(new Color(0.1f, 0.1f, 0.1f, 0.995f), EBackground.RoundedStandard).Vertical(10, "EC").Padding(2).OverrideSorting(202);

        var titleContainer = SContainer.Background(new Color(0.06f, 0.06f, 0.06f, 0.8f), EBackground.None).PaddingVertical(10).PHeight(70);
        titleContainer.Add(SLabel.Text("Credits").Dock(EDockType.Fill).FontSize(40).FontSpacing(8.5f).FontColor(new Color(0.72f, 0.25f, 0.25f)));
        creditsPanel.Add(titleContainer);

        var scrollbar = SDiv.FlexHeight(1);
        scrollbar.SetParent(creditsPanel);
        //scrollbar.Visible(false);

        var scroll = SScrollContainer
        .Dock(EDockType.Fill)
        .Background(Color.black.WithAlpha(0), EBackground.RoundedStandard)
        .Size(-20, -20)
        .As<SScrollContainerOptions>();
        scroll.ContainerObject.Spacing(20);
        scroll.ContainerObject.PaddingHorizontal(10);
        scroll.SetParent(scrollbar);

        /*
        foreach (var line in File.ReadLines(@"C:\Users\Alex\Downloads\SurvivalModeThings\Credits.txt"))
        {
            scroll.Add(CreditTextLine(line));
        }*/

        ReadCredits(scroll);

        var bottomContainer = SContainer.Background(new Color(0.06f, 0.06f, 0.06f, 0.8f), EBackground.None).PHeight(50).PaddingVertical(10);
        bottomContainer.Add(SButton.Text("Back").Notify(ToggleCreditsPanel).Dock(EDockType.Fill).FontAutoSize(true));
        creditsPanel.Add(bottomContainer);
        creditsPanel.Active(false);
        creditsPanel.Add(creditsPanel);
        creditsPanel.Active(false);

        return creditsPanel;
    }

    public static void ToggleCreditsPanel()
    {
        TogglePanel(MAIN_MENU_CREDITS);
    }

    private static void ReadCredits(SScrollContainerOptions scroll)
    {
        if (ResourcesLoader.TryGetEmbeddedResourceBytes("Credits.txt", out byte[] bytes))
        {
            byte[] data = bytes;
            string tempFilePath = Path.Combine(Application.persistentDataPath, $"{Guid.NewGuid()}.txt");
            File.WriteAllBytes(tempFilePath, data);

            foreach (var line in File.ReadLines(tempFilePath))
            {
                scroll.Add(CreditTextLine(line));
            }
            File.Delete(tempFilePath);
        }
    }

    private static SLabelOptions CreditTextLine(string text, int fontsize = 34)
    {
        return SLabel.RichText(text).Alignment(TextAlignmentOptions.Center).FontSize(fontsize);
    }
}
