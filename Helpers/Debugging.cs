using RedLoader;
using UnityEngine;
using ZombieMode.Core;
using ZombieMode.Gameplay;

namespace ZombieMode.Helpers;

public class Debugging
{
    public static void DebugUI()
    {
        if (Game.GameState == Game.GameStates.Menu)
        {
            if (GUI.Button(new Rect(1700, 150, 100, 50), "Start game"))
            {
                Loading.LoadIntoGame().RunCoro();
            }
        }
        else if (Game.GameState == Game.GameStates.Loading)
        {
            GUI.Label(new Rect(50, 25, 100, 50), $"Loading Survival Mode... {Loading.LoadingProgress.Value:N1}");
        }
        else if (Game.GameState == Game.GameStates.InGame)
        {
            GUI.Label(new Rect(10, 5, 150, 50), $"Round: {Game.Round.Value}");
            GUI.Label(new Rect(10, 20, 150, 50), $"Remaining enemies: {Game.Enemies.Value}");
            GUI.Label(new Rect(10, 35, 150, 50), $"Score: {ScoreSystem.Score.Value}");

            GUI.Label(new Rect(10, 1030, 150, 50), $"{Player.RemainingAmmo.Value}/{Player.TotalAmmo.Value}");
        }
    }
}
