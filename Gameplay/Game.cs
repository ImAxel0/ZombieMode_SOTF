using TheForest.Utils;
using ZombieMode.Helpers;
using Sons.Items.Core;
using TheForest;
using UnityEngine;
using Sons.Ai.Vail;
using SUI;
using RedLoader;
using HarmonyLib;
using Sons.Weapon;
using ZombieMode.Libs;
using ZombieMode.UI;
using ZombieMode.Core;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class Game : MonoBehaviour
{
    public enum GameStates
    {
        Menu,
        Loading,
        InGame,
        DeadInGame
    }

    public static GameStates GameState = GameStates.Menu;
    public static Observable<int> Round = new(1);
    public static Observable<int> Enemies = new(0);

    static SonsFMODEventEmitter _fmodEmitter = new();

    public static void InitGame()
    {
        GameState = GameStates.InGame;
        Loading.LoadingInfo.Set("Initializing game...");
        LocalPlayer._instance.gameObject.AddComponent<Player>();
        LocalPlayer._instance.gameObject.AddComponent<CustomInventory>();
        LocalPlayer._instance.gameObject.AddComponent<Game>();
        LocalPlayer._instance.gameObject.AddComponent<ScoreSystem>();
        LocalPlayer._instance.gameObject.AddComponent<MysteryBoxController>();
        LocalPlayer._instance.gameObject.AddComponent<ForgeController>();
        LocalPlayer._instance.gameObject.AddComponent<DoorsManager>();
        LocalPlayer._instance.gameObject.AddComponent<WallItems>();
        LocalPlayer._instance.gameObject.AddComponent<VendingMachines>();
        LocalPlayer._instance.gameObject.AddComponent<Consumables>();
        LocalPlayer._instance.gameObject.AddComponent<SceneMaterialsSwap>();
        LocalPlayer._instance.gameObject.AddComponent<HUD>();
        LocalPlayer._instance.gameObject.AddComponent<Overlays>();
        ZombieConsole.SilentInput().RunCoro();

        ZombieMode.HarmonyInst.Patch(AccessTools.Method(typeof(VailActor), nameof(VailActor.UpdateDamageTakenStats)),
            new HarmonyMethod(typeof(ScoreSystem), nameof(ScoreSystem.DamageToScore)));

        ZombieMode.HarmonyInst.Patch(AccessTools.Method(typeof(HeldControllerBase), nameof(HeldControllerBase.SetupFirstTimeEquip)),
            new HarmonyMethod(typeof(Player), nameof(Player.PreventPickupAnim)));

        ZombieMode.HarmonyInst.Patch(AccessTools.Method(typeof(CompactPistolWeaponController), nameof(CompactPistolWeaponController.TriggerShotFiredAudio)),
            new HarmonyMethod(typeof(WeaponsUpgrade.PistolUpgrade), nameof(WeaponsUpgrade.PistolUpgrade.OnTriggerShotFiredAudio)));

        ZombieMode.HarmonyInst.Patch(AccessTools.Method(typeof(ShotgunWeaponController), nameof(ShotgunWeaponController.TriggerShotFiredAudio)),
            new HarmonyMethod(typeof(WeaponsUpgrade.ShotgunUpgrade), nameof(WeaponsUpgrade.ShotgunUpgrade.OnTriggerShotFiredAudio)));

        //ZombieMode.HarmonyInst.Patch(AccessTools.Method(typeof(RangedWeaponController), nameof(RangedWeaponController.Update)),
        //new HarmonyMethod(typeof(WeaponsUpgrade), nameof(WeaponsUpgrade.RangedWeaponControllerUpdate)));

        ZombieMode.HarmonyInst.Patch(AccessTools.Method(typeof(RangedWeapon), nameof(RangedWeapon.LateUpdate)),
            new HarmonyMethod(typeof(Player), nameof(Player.UpdateAmmoCounter)));
    }

    public static void OnSpawn()
    {
        InitGame();
        DebugConsole.Instance._lockTimeOfDay("00");
        //DebugConsole.Instance._godmode("on");
        GameObject.Find("SpaMusicGroup").SetActive(false);
        GameObject.Find("GymMusicGroup").SetActive(false);
        GameObject.Find("BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/ParlorPalmALOD0").SetActive(false);
        GameObject.Find("BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/ParlorPalmALOD0 (2)").SetActive(false);
        GameObject.Find("BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/MonsteraPlantALOD0").SetActive(false);
        ActorsManager.PreventSpawn();
        LocalPlayer.Inventory.UnequipItemAtSlot(EquipmentSlot.RightHand, false, true, false);
        LocalPlayer.Inventory.UnequipItemAtSlot(EquipmentSlot.LeftHand, false, true, false);
        foreach (ItemData itemData in ItemDatabaseManager.Items)
        {
            if (itemData.Id != (int)ItemsIdManager.ItemsId.CombatKnife)
            {
                LocalPlayer.Inventory.RemoveItem(itemData.Id);
            }
        }
        LocalPlayer.Inventory.AddItem((int)ItemsIdManager.ItemsId.Backpack);
        LocalPlayer.Inventory.AddItem((int)ItemsIdManager.ItemsId.CompactPistol);
        LocalPlayer.Inventory.AddItem((int)ItemsIdManager.ItemsId.PistolAmmo, 120, true);

        PlayerAnims.PlayWakeup();
        SpawnSystem.CreateActorSpawners();
        SpawnSystem.BeginSpawnEnemies(25, true).RunCoro();
        SoundManager.musicEmitter.Stop();
        AudioController.PlayBSound(SoundManager.musicEmitter, "event:/Ambient/wind", AudioController.SoundType.Sfx, 2);
        HUD.DisableGameHud();
        Loading.ToggleLoading();
        HUD.HudPanel.Active(true);
    }

    private static void IncRound()
    {
        Round.Value++;
    }

    private static void AdvanceRound()
    {
        HUD.RoundCrossFade().RunCoro();
        IncRound();
        Consumables.UpdateNewRound();
        SpawnSystem.BeginSpawnEnemies().RunCoro();
        AudioController.PlayBSound(_fmodEmitter, "event:/Game/start", AudioController.SoundType.Sfx);
    }

    public void Update()
    {
        Enemies.Value = ActorsManager.GetEnemiesAlive();

        if (Enemies.Value == 0 && SpawnSystem.canSpawnEnemies)
        {
            AdvanceRound();
        }
    }
}
