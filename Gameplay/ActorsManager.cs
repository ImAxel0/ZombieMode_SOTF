using Sons.Ai.Vail;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using RedLoader;
using HarmonyLib;
using UnityEngine.UI;
using ZombieMode.Core;

namespace ZombieMode.Gameplay;

public class ActorsManager
{
    public static void RemoveActors()
    {
        VailActorManager.ClearAll();
    }

    public static void PreventSpawn()
    {
        foreach (var spawner in VailWorldSimulation._instance._vailSpawners)
        {
            VailWorldSimulation._instance.RemoveActorsFromSpawner(spawner, true);
        }
        VailWorldSimulation._instance._vailSpawners.Clear();
        RemoveActors();
    }

    public static IEnumerator RemoveDeadBodies(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
        VailActorManager.RemoveDeadBodies();
    }

    public static int GetEnemiesAlive()
    {
        return VailActorManager.GetActiveActors().Where(actor => !actor.IsDead()).Count();
    }

    [HarmonyPatch(typeof(VailWorldSimulation), nameof(VailWorldSimulation.OnActorDied))]
    [HarmonyPrefix]
    public static void OnDeath(ref WorldSimActor deadActor, ref bool killedByPlayer)
    {
        if (Consumables.ScoreBuffer >= Consumables.DropEveryScore && Consumables.DropsBuffer < Consumables.MaxRoundDrops)
        {
            Consumables.SpawnRandConsumable(deadActor).RunCoro();
        }
    }

    [HarmonyPatch(typeof(VailActor), nameof(VailActor.DamageActor))]
    [HarmonyPrefix]
    public static void OnDamaged(ref float damage, ref VailActor __instance)
    {
        if (!__instance.IsDead())
        {
            var go = UnityEngine.Object.Instantiate(DamageTextGo.DamageText,
                __instance._visionHeadJoint.position,
                Quaternion.identity).AddComponent<AnimationAutoDestroy>();

            go.transform.LookAt(LocalPlayer.MainCamTr);
            go.GetComponentInChildren<Text>().text = damage.ToString("0");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class AnimationAutoDestroy : MonoBehaviour
    {
        void Start()
        {
            Destroy(gameObject, GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class Ignite : MonoBehaviour
    {
        static List<VailActor> _ignites = new();
        static float _explosionRadius = 2f;
        static float _explosionDamage = 20;

        public static void SpawnIgnite(VailActor spawnedActor)
        {
            spawnedActor.name = "Ignite";
            spawnedActor._stateFx.UpdateFireStatusFx(true);
            spawnedActor._stateFx._statusEffects.RemoveRange(1, spawnedActor._stateFx._statusEffects.Count - 1); // prevent fire from being stuck on floor and keep fire anim
            spawnedActor.gameObject.AddComponent<Ignite>();
            spawnedActor.gameObject.AddComponent<BoxCollider>().isTrigger = true;
            spawnedActor.gameObject.GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);
            _ignites.Add(spawnedActor);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //AudioController.PlaySound("TheIgniteExplosion", AudioController.SoundType.Sfx);
                LocalPlayer.HitReactions.enableExplodeShake(2, 0);
                VailActorManager._instance.KillActorsInRadius(other.gameObject.transform.position, _explosionRadius, VailActorClassId.Cannibal);
                VailActorManager._instance.KillActorsInRadius(other.gameObject.transform.position, _explosionRadius, VailActorClassId.Creepy);
                VailActorManager._instance.DismemberActorsInRadius(other.gameObject.transform.position, _explosionRadius);
                LocalPlayer.Stats.Hit(_explosionDamage, false, ImpactType.Explosion);
            }
        }

        public void Update()
        {
            foreach (var ignite in _ignites.ToList())
            {
                if (ignite.IsDead())
                {
                    Instantiate(IgniteExplosionFx.IgniteExplosion, ignite.transform.position + Vector3.up * 2, Quaternion.identity)
                        .GetComponent<ParticleSystem>()
                        .Play();

                    ignite.gameObject.GetComponent<BoxCollider>().enabled = false;
                    _ignites.Remove(ignite);
                }
            }
        }
    }
}
