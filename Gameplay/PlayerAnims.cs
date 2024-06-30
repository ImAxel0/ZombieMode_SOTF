using TheForest.Utils;

namespace ZombieMode.Gameplay;

public class PlayerAnims
{
    public static void PlayWakeup()
    {
        LocalPlayer.AnimControl.StandUpFromCrash(0);
    }
}
