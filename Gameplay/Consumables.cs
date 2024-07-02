using UnityEngine;
using RedLoader;
using Sons.Ai.Vail;
using System.Collections;
using System.Diagnostics;
using System.Data;
using ZombieMode.Core;
using SUI;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class Consumables : MonoBehaviour
{
    public static Dictionary<GameObject, Action> ConsumablesPair = new();
    static GameObject _spawnedConsumable;
    static float _consumablesDuration = 30f;
    static float _timeToFade = 15f;
    static bool _wasPickedUp;

    public static int DropEveryScore = 2000;
    public static int MaxRoundDrops = 4;
    public static int ScoreBuffer;
    public static int DropsBuffer;

    public static Observable<bool> IsDoubleScore = new(false);
    public static Observable<bool> IsFireSale = new(false);
    public static Observable<bool> IsImperceptible = new(false);
    public static Observable<bool> IsLockThemUp = new(false);
    public static Observable<bool> IsSlowThemDown = new(false);

    public void Start()
    {
        ConsumablesPair.Clear();
        ConsumablesPair.Add(NukeConsumableGo.NukeConsumable, ConsumablesController.DoNuke);
        ConsumablesPair.Add(FireSaleConsumableGo.FireSaleConsumable, () => ConsumablesController.DoFireSale().RunCoro());
        ConsumablesPair.Add(ImperceptibleConsumableGo.ImperceptibleConsumable, () => ConsumablesController.DoImperceptible().RunCoro());
        ConsumablesPair.Add(LockThemUpConsumableGo.LockThemUpConsumable, () => ConsumablesController.DoLockThemUp().RunCoro());
        ConsumablesPair.Add(DoubleScoreConsumableGo.DoubleScoreConsumable, () => ConsumablesController.DoDoubleScore().RunCoro());
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

        public static IEnumerator DoDoubleScore()
        {
            IsDoubleScore.Value = true;
            ScoreSystem.ScoreMultiplier = 2;
            yield return new WaitForSeconds(_consumablesDuration);
            ScoreSystem.ScoreMultiplier = 1;
            IsDoubleScore.Value = false;
        }

        public static IEnumerator DoFireSale(float salePerc = 0.5f)
        {
            IsFireSale.Value = true;
            ScoreSystem.SaleMultiplier = salePerc;
            yield return new WaitForSeconds(_consumablesDuration);
            ScoreSystem.SaleMultiplier = 1f;
            IsFireSale.Value = false;
        }

        public static IEnumerator DoLockThemUp()
        {
            IsLockThemUp.Value = true;
            VailWorldSimulation.SetPaused(true);
            yield return new WaitForSeconds(_consumablesDuration);
            VailWorldSimulation.SetPaused(false);
            IsLockThemUp.Value = false;
        }

        public static IEnumerator DoImperceptible()
        {
            IsImperceptible.Value = true;
            VailActorManager.SetGhostPlayer(true);
            yield return new WaitForSeconds(_consumablesDuration);
            VailActorManager.SetGhostPlayer(false);
            IsImperceptible.Value = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _wasPickedUp = true;
                switch (_spawnedConsumable.name)
                {
                    case "NukeConsumable(Clone)":
                        ConsumablesPair.TryGetValue(NukeConsumableGo.NukeConsumable, out var _NukeConsumable);
                        ScoreSystem.AddScore(ActorsManager.GetEnemiesAlive() * 100);
                        _NukeConsumable.Invoke();
                        break;
                    case "FireSaleConsumable(Clone)":
                        ConsumablesPair.TryGetValue(FireSaleConsumableGo.FireSaleConsumable, out var _FireSaleConsumable);
                        _FireSaleConsumable.Invoke();
                        break;
                    case "ImperceptibleConsumable(Clone)":
                        ConsumablesPair.TryGetValue(ImperceptibleConsumableGo.ImperceptibleConsumable, out var _ImperceptibleConsumable);
                        _ImperceptibleConsumable.Invoke();
                        break;
                    case "LockThemUpConsumable(Clone)":
                        ConsumablesPair.TryGetValue(LockThemUpConsumableGo.LockThemUpConsumable, out var _LockThemUpConsumable);
                        _LockThemUpConsumable.Invoke();
                        break;
                    case "DoubleScoreConsumable(Clone)":
                        ConsumablesPair.TryGetValue(DoubleScoreConsumableGo.DoubleScoreConsumable, out var _DoubleScoreConsumable);
                        _DoubleScoreConsumable.Invoke();
                        break;
                }
            }
        }
    }
}
