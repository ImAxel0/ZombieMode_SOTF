using UnityEngine;
using ZombieMode.Core;
using ZombieMode.Gameplay;
using ZombieMode.Libs;

namespace ZombieMode.UI;

public class UiManager
{
    public static Font Headliner45 = ResourcesLoader.LoadEmbeddedFont("HeadlinerNo.45");

    public static void CreateAllUi()
    {
        ZMainMenu.UiCreate();
        Settings.UiCreate();
        Loading.UiCreate();
        InstallerUi.UiCreate();
        Overlays.UiCreate();
        HUD.UiCreate();
        Scoreboard.UiCreate();
        ZombieConsoleUi.CreateZombieConsole();
    }
}
