using ZombieMode.Core;
using ZombieMode.Gameplay;

namespace ZombieMode.UI;

public class UiManager
{
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
