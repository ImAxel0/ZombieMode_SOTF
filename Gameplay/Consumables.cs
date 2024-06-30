using UnityEngine;
using RedLoader;
using Sons.Ai.Vail;
using System.Collections;
using System.Diagnostics;
using System.Data;
using ZombieMode.Core;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class Consumables : MonoBehaviour
{
    public static Dictionary<GameObject, Action> ConsumablesPair = new();
    static GameObject _spawnedConsumable;
    static float _timeToFade = 15f;
    static bool _wasPickedUp;

    public static int DropEveryScore = 2000;
    public static int MaxRoundDrops = 4;
    public static int ScoreBuffer;
    public static int DropsBuffer;

    public void Start()
    {
        ConsumablesPair.Clear();
        ConsumablesPair.Add(NukeConsumableGo.NukeConsumable, ConsumablesController.DoNuke);
    }

    private static void UpdateStats()
    {
        _wasPickedUp = false;
        ScoreBuffer = 0;
        DropsBuffer++;
    }

    public static void UpdateNewRound()
    {
        DropsBuffer = 0;
        DropEveryScore = (int)(DropEveryScore * 1.14f);
    }

    public static IEnumerator SpawnRandConsumable(WorldSimActor deadActor)
    {
        UpdateStats();

        _spawnedConsumable = Instantiate(ConsumablesPair.ElementAt(UnityEngine.Random.Range(0, ConsumablesPair.Count)).Key,
                deadActor.Position() + Vector3.up * 1f,
                Quaternion.identity);

        _spawnedConsumable.AddComponent<ConsumablesController>();

        Stopwatch sw = new();
        sw.Start();

        while (sw.Elapsed.Seconds <= _timeToFade)
        {
            if (_wasPickedUp) break;
            yield return null;
        }
        Destroy(_spawnedConsumable);
    }

    [RegisterTypeInIl2Cpp]
    public class ConsumablesController : MonoBehaviour
    {
        public static void DoNuke()
        {
            VailActorManager.GetActiveActors().Where(actor => !actor.IsDead()).ToList().ForEach(actor =>
            {
                actor.DismemberRandomPart();
                actor.ForceDeath();
                actor.IgniteSelf();
            });
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _wasPickedUp = true;
                if (ConsumablesPair.TryGetValue(ConsumablesPair.Keys.FirstOrDefault(x => x.tag == _spawnedConsumable.tag), out var Action))
                {
                    Action.Invoke();
                }
            }
        }
    }
}
