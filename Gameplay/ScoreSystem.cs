using UnityEngine;
using RedLoader;
using SUI;
using Sons.Ai.Vail;
using TheForest.Utils;
using ZombieMode.UI;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class ScoreSystem : MonoBehaviour
{
    public static Observable<int> Score = new(10000);
    public static Observable<string> StringScore = new(string.Empty);
    public static int ScoreMultiplier { get; set; } = 1;
    public static float SaleMultiplier { get; set; } = 1f;

    public static int PlayerKills;
    public static Observable<string> StringPlayerKills = new(string.Empty);

    public static void AddScore(int amount)
    {
        amount *= ScoreMultiplier;
        Consumables.ScoreBuffer += amount;
        Score.Value += amount;
    }

    public static void DecScore(int amount)
    {
        Score.Value -= (int)(amount * SaleMultiplier);
    }

    public static void DamageToScore(ref MonoBehaviourStimuli sourceStimuli, ref float damage)
    {
        if (sourceStimuli._transform.gameObject == LocalPlayer.GameObject)
        {
            AddScore((int)Mathf.Clamp(damage, 0, 120));
        }
    }

    public void Update()
    {
        StringScore.Set($"Score <color=yellow>{Score.Value} $</color>");
        StringPlayerKills.Set($"Kills <color=red>{PlayerKills}</color>");

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Scoreboard.Show(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            Scoreboard.Show(false);
        }
    }

    private void OnDestroy()
    {
        Score.Set(500);
        StringScore.Set(string.Empty);
        ScoreMultiplier = 1;
        SaleMultiplier = 1;
        PlayerKills = 0;
        StringPlayerKills.Set(string.Empty);
    }
}
