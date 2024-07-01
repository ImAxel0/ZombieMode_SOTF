using SonsSdk;
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

public class ZombieConsoleUi
{
    public static STextboxOptions ConsoleInput {  get; set; }
    public const string ZOMBIE_CONSOLE_ID = "ZombieConsole";

    public static void CreateZombieConsole()
    {
        var survivalConsole = RegisterNewPanel(ZOMBIE_CONSOLE_ID, true)
            .Dock(EDockType.Fill).Margin(0, 0, 0, 0).Background(Color.black.WithAlpha(0.6f), EBackground.None).Vertical(5, "EC").OverrideSorting(999);

        var commandLine = SContainer.Background(new Color(0.06f, 0.06f, 0.06f, 0.9f), EBackground.None).Horizontal(0, "EE").Padding(10).PHeight(70);
        commandLine.Add(SLabel.Text("ZOMBIE CONSOLE").Dock(EDockType.Fill).Alignment(TextAlignmentOptions.Left).FontSize(40).FontColor(Color.red));
        ConsoleInput = STextbox.Text("<color=yellow>Command</color> (enter to execute)").Dock(EDockType.Fill).Placeholder("insert command here...")
            .FontAutoSize(true).Bind(ZombieConsole.Command).Background(EBackground.None).HOffset(20, -20);
        commandLine.Add(ConsoleInput);
        survivalConsole.Add(commandLine);

        var columnsContainer = SContainer.Background(Color.black.WithAlpha(0), EBackground.None).Horizontal(0, "EC").PHeight(1000);
        List<SContainerOptions> columns = new();
        for (int i = 0; i < 5; i++)
        {
            var column = SContainer.Background(Color.black.WithAlpha(0), EBackground.None).Vertical(10, "EC").PHeight(1000).PWidth(350).Padding(10);
            columns.Add(column);
            columnsContainer.Add(column);
        }

        var sortedCommands = ZombieConsole.GetAvailableCommands().OrderBy(n => n);
        int commandIndex = 0;
        int columnIndex = 0;
        foreach (var command in sortedCommands)
        {
            if (commandIndex != 0 && SpawnSystem.IsMultipleOf(commandIndex, 26))
                columnIndex++;

            columns[columnIndex].Add(SLabel.Text(command).Alignment(TextAlignmentOptions.Left));
            commandIndex++;
        }
        survivalConsole.Add(columnsContainer);
        survivalConsole.Active(false);
    }
}
