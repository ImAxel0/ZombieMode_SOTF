using Sons.Ai.Vail;
using Sons.Characters;
using static Sons.Characters.VailSpawner;
using TheForest.Utils;
using UnityEngine;
using ZombieMode.Helpers;
using RedLoader;
using System.Collections;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class SpawnSystem : MonoBehaviour
{
    struct HordeData
    {
        public static int EnemiesToSpawn;
        public static float SpawnDelayBetweenEnemies = 2f;
    };

    public enum SpawnPoints
    {
        GymSpawn1, GymSpawn2, GymSpawn3, GymSpawn4, GymSpawn5,
        GymPool1, GymPool2,
        SpaSpawn1, SpaSpawn2,
        StairsHall1, StairsHall2, StairsHall3,
        PoolSpawn1, PoolSpawn2, PoolSpawn3,
        BarSpawn1, BarSpawn2, BarSpawn3,
    }

    // identifier, <pos., sphere>
    public static Dictionary<SpawnPoints, Tuple<Vector3, float>> SpawnPointsDict = new()
    {
        //{ SpawnPoints.GymSpawn1, Tuple.Create(new Vector3(-1131, 59, 15), 4f) },
        //{ SpawnPoints.GymSpawn2, Tuple.Create(new Vector3(-1149, 59, 1), 4f) },
        //{ SpawnPoints.GymSpawn3, Tuple.Create(new Vector3(-1154, 59, 21), 4f) },
        //{ SpawnPoints.GymSpawn4, Tuple.Create(new Vector3(-1172, 59, 35), 4f) },
        //{ SpawnPoints.GymSpawn5, Tuple.Create(new Vector3(-1155, 59, 46), 4f) },
        { SpawnPoints.SpaSpawn1, Tuple.Create(new Vector3(-1110, 59, 28), 4f) },
        { SpawnPoints.SpaSpawn2, Tuple.Create(new Vector3(-1118, 59, -20), 4f) }
    };

    public static Dictionary<SpawnPoints, Tuple<Vector3, float>> GymSpawns = new()
    {
        { SpawnPoints.GymSpawn1, Tuple.Create(new Vector3(-1131, 59, 15), 4f) },
        { SpawnPoints.GymSpawn2, Tuple.Create(new Vector3(-1149, 59, 1), 4f) },
        { SpawnPoints.GymSpawn3, Tuple.Create(new Vector3(-1154, 59, 21), 4f) },
        { SpawnPoints.GymSpawn4, Tuple.Create(new Vector3(-1172, 59, 35), 4f) },
        { SpawnPoints.GymSpawn5, Tuple.Create(new Vector3(-1155, 59, 46), 4f) },
    };

    public static Dictionary<SpawnPoints, Tuple<Vector3, float>> GymPoolSpawns = new()
    {
        { SpawnPoints.GymPool1, Tuple.Create(new Vector3(-1181, 59, 18), 4f) },
        { SpawnPoints.GymPool2, Tuple.Create(new Vector3(-1211, 59, 23), 4f) }
    };

    public static Dictionary<SpawnPoints, Tuple<Vector3, float>> StairsHallSpawns = new()
    {
        { SpawnPoints.StairsHall1, Tuple.Create(new Vector3(-1096, 59, -16), 4f) },
        { SpawnPoints.StairsHall2, Tuple.Create(new Vector3(-1091, 59, 16), 4f) },
        { SpawnPoints.StairsHall3, Tuple.Create(new Vector3(-1079, 59, -22), 4f) }
    };

    public static Dictionary<SpawnPoints, Tuple<Vector3, float>> PoolSpawns = new()
    {
        { SpawnPoints.PoolSpawn1, Tuple.Create(new Vector3(-1100, 67, 14), 4f) },
        { SpawnPoints.PoolSpawn2, Tuple.Create(new Vector3(-1104, 67, -9), 4f) },
        { SpawnPoints.PoolSpawn3, Tuple.Create(new Vector3(-1145, 67, -6), 4f) }
    };

    public static Dictionary<SpawnPoints, Tuple<Vector3, float>> BarSpawns = new()
    {
        { SpawnPoints.BarSpawn1, Tuple.Create(new Vector3(-1168, 66, 20), 4f) },
        { SpawnPoints.BarSpawn2, Tuple.Create(new Vector3(-1178, 66, 41), 4f) },
        { SpawnPoints.BarSpawn3, Tuple.Create(new Vector3(-1192, 66, 34), 4f) },
    };

    public static List<Vector3> SpawnPointsPos
    {
        get
        {
            List<Vector3> points = new();
            foreach (var spawnpoint in SpawnPointsDict)
            {
                points.Add(spawnpoint.Value.Item1);
            }
            return points;
        }
    }

    static int maxEnemiesOnMap = 24;
    static float initRoundsCorrection = 4f;
    public static bool canSpawnEnemies;
    public static bool LockSpawn;

    public static VailSpawner CannibalSpawner;
    static Func<int, bool> _shouldRemoveBodies = round => round % 5 == 0;

    public static void CreateActorSpawners()
    {
        var customVailSpawnerObject = new GameObject("Custom Vail Spawner");
        UnityEngine.Object.DontDestroyOnLoad(customVailSpawnerObject);
        customVailSpawnerObject.hideFlags = HideFlags.HideAndDontSave;

        CannibalSpawner = customVailSpawnerObject.AddComponent<VailSpawner>();
        CannibalSpawner.name = "CannibalSpawner";
        CannibalSpawner._characterDefinition = CharacterManager.Instance.GetDefinition(EnemiesIdentifiers.Misspuffy);
        CannibalSpawner._cave = GameObject.Find("BunkerD-Entertainment").GetComponent<CreepyCave>();
        CannibalSpawner._spawnOnTerrain = false;
    }

    public static bool IsMultipleOf(int multiple, int dividend)
    {
        return multiple % dividend == 0;
    }

    public static void EnableSpawnPoint(Dictionary<SpawnPoints, Tuple<Vector3, float>> spawnDict)
    {
        foreach (var spawnpoint in spawnDict)
        {
            SpawnPointsDict.Add(spawnpoint.Key, spawnpoint.Value);
        }
    }

    private static void NewHordeData(int currentRound)
    {
        HordeData.EnemiesToSpawn = (int)(0.0842f * Math.Pow(currentRound, 2) + 0.1954f * currentRound + 22.05f);

        if (currentRound < 10)
        {
            int count = (int)(HordeData.EnemiesToSpawn / initRoundsCorrection);
            HordeData.EnemiesToSpawn = count;
            initRoundsCorrection *= 0.85f;
        }

        if (currentRound > 1 && HordeData.SpawnDelayBetweenEnemies! <= 0.08f)
            HordeData.SpawnDelayBetweenEnemies *= 0.95f;

        Game.Enemies.Value = HordeData.EnemiesToSpawn;
    }

    public static IEnumerator BeginSpawnEnemies(float spawnDelayBetweenRound = 15f, bool clearActors = false)
    {
        if (_shouldRemoveBodies(Game.Round.Value))
            ActorsManager.RemoveDeadBodies().RunCoro();

        NewHordeData(Game.Round.Value);
        canSpawnEnemies = false;

        yield return new WaitForSeconds(spawnDelayBetweenRound);
        if (clearActors) ActorsManager.RemoveActors();

        for (int i = 0; i < HordeData.EnemiesToSpawn; i++)
        {
            while (Game.Enemies.Value >= maxEnemiesOnMap || LockSpawn)
            {
                yield return null;
            }

            if (i != 0 && Game.Enemies.Value == 0 && !LockSpawn)
            {
                // e.g nuke
                yield return new WaitForSeconds(10f);
            }

            List<float> distances = new();
            foreach (var _spawnPoint in SpawnPointsDict)
            {
                distances.Add(Vector3.Distance(LocalPlayer.Transform.position, _spawnPoint.Value.Item1));
            }

            int idx = distances.IndexOf(distances.Min()); // SpawnPoints sp = (SpawnPoints)
            SpawnPointsDict.TryGetValue(SpawnPointsDict.ElementAt(idx).Key, out var spawnPoint);
            var point = new SpawnPoint() { Position = spawnPoint.Item1 + UnityEngine.Random.insideUnitSphere * spawnPoint.Item2 };
            var spawnedActor = CannibalSpawner.SpawnWorldSimActor(point.Position, 0, 1);
            var spawnedVail = VailWorldSimulation.Instance().ConvertToRealActor(spawnedActor);       

            yield return new WaitForSeconds(HordeData.SpawnDelayBetweenEnemies);
        }
        yield return new WaitForSeconds(2f);
        canSpawnEnemies = true;
    }

    private void OnDestroy()
    {
        HordeData.EnemiesToSpawn = 0;
        HordeData.SpawnDelayBetweenEnemies = 2f;
        initRoundsCorrection = 4f;
        canSpawnEnemies = false;
        LockSpawn = false;

        // resetting spawn points on death
        foreach (var _spawnPoint in GymSpawns.Keys)
        {
            SpawnPointsDict.Remove(_spawnPoint);
        }
        foreach (var _spawnPoint in GymPoolSpawns.Keys)
        {
            SpawnPointsDict.Remove(_spawnPoint);
        }
        foreach (var _spawnPoint in StairsHallSpawns.Keys)
        {
            SpawnPointsDict.Remove(_spawnPoint);
        }
        foreach (var _spawnPoint in PoolSpawns.Keys)
        {
            SpawnPointsDict.Remove(_spawnPoint);
        }
        foreach (var _spawnPoint in BarSpawns.Keys)
        {
            SpawnPointsDict.Remove(_spawnPoint);
        }
    }
}
