using UnityEngine;
using RedLoader;
using SUI;
using Sons.Ai.Vail;
using TheForest.Utils;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class ScoreSystem : MonoBehaviour
{
    public static Observable<int> Score = new(10000);

    public static void AddScore(int amount)
    {
        Consumables.ScoreBuffer += amount;
        Score.Value += amount;
    }

    public static void DecScore(int amount)
    {
        Score.Value -= amount;
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

    }
}
