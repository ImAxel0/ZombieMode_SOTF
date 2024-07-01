using RedLoader;
using Sons.Weapon;
using SUI;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using HarmonyLib;
using TheForest;
using ZombieMode.Libs;
using Sons.Ai.Vail;
using TheForest.Items.Inventory;
using Sons.Items;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class Player : MonoBehaviour
{
    static bool _isRestoringHealth;
    public static bool IsOnLava;
    public static Observable<int> RemainingAmmo = new(0);
    public static Observable<int> TotalAmmo = new(0);
    static SonsFMODEventEmitter _emitter;

    void Start()
    {
        _emitter = new();
        OnLavaDamage().RunCoro();
        RegenHealth().RunCoro();
    }

    public static IEnumerator RegenHealth()
    {
        while (Game.GameState == Game.GameStates.InGame)
        {
            if (LocalPlayer.Vitals.Health._currentValue < 100)
            {
                LocalPlayer.Vitals.Health._currentValue++;
                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
    }

    public static bool PreventPickupAnim()
    {
        return false;
    }

    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.Open))]
    [HarmonyPrefix]
    public static bool PreventInventory()
    {
        return false;
    }

    public static void UpdateAmmoCounter(ref RangedWeapon __instance)
    {
        RemainingAmmo.Value = __instance.GetAmmo().GetRemainingAmmo();
        TotalAmmo.Value = LocalPlayer.Inventory.AmountOf(__instance.GetAmmo()._type);
    }

    [HarmonyPatch(typeof(Vitals), nameof(Vitals.ApplyDamage))]
    [HarmonyPrefix]
    public static void OnDamageTaken()
    {
        Overlays.DoOverlay(Overlays.OverlayTypes.Blood);
    }

    public void Update()
    {

    }

    static IEnumerator OnLavaDamage()
    {
        while (Game.GameState == Game.GameStates.InGame)
        {
            if (IsOnLava)
            {
                DebugConsole.HitLocalPlayer(5);
                AudioController.PlayBSound(_emitter, "event:/Ambient/player-burn", AudioController.SoundType.Sfx);
                Overlays.DoOverlay(Overlays.OverlayTypes.Fire, 0, 0.3f);
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
    }
}
