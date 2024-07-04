using FMODUnity;
using Sons.Weapon;
using SonsSdk;
using UnityEngine;
using ZombieMode.Helpers;
using HarmonyLib;
using Endnight.Utilities;
using ZombieMode.Libs;
using ZombieMode.Core;
using RedLoader;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class WeaponsUpgrade : MonoBehaviour
{
    [HarmonyPatch(typeof(RangedWeaponController), nameof(RangedWeaponController.Update))]
    [HarmonyPrefix]
    public static void RangedWeaponControllerUpdate(ref RangedWeaponController __instance)
    {
        switch (__instance.ItemId)
        {
            case (int)ItemsIdManager.ItemsId.CompactPistol:
                PistolUpgrade.Update(__instance);
                break;

            case (int)ItemsIdManager.ItemsId.ShotgunPumpAction:
                ShotgunUpgrade.Update(__instance);
                break;
        }
    }

    public static bool IsWeaponUpgraded(ItemsIdManager.ItemsId itemId)
    {
        switch (itemId)
        {
            case ItemsIdManager.ItemsId.CompactPistol:
                return PistolUpgrade.IsUpgraded;
            case ItemsIdManager.ItemsId.ShotgunPumpAction:
                return ShotgunUpgrade.IsUpgraded;
            default:
                return false;
        }
    }

    private void OnDestroy()
    {
        PistolUpgrade.IsUpgraded = false;
        ShotgunUpgrade.IsUpgraded = false;
    }

    public class PistolUpgrade
    {
        public static bool IsUpgraded;
        static SonsFMODEventEmitter _fmodEmitter;

        public static void OnTriggerShotFiredAudio(ref CompactPistolWeaponController __instance)
        {
            if (IsUpgraded)
            {
                AudioController.PlayBSound(_fmodEmitter, "event:/Pistol/UpgradeShoot", AudioController.SoundType.Sfx);

                UnityEngine.Object.Instantiate(PistolUpgradeImpactFx.PistolUpgradeImpact,
                    __instance._rangedWeapon._impactTargetInstance.transform.position,
                    Quaternion.identity).GetComponent<ParticleSystem>().Play();
            }
        }

        public static void UpgradeFx(RangedWeaponController __instance, bool onoff)
        {
            if (onoff)
            {
                __instance._gunShotAudioEvent = "";
                __instance._muzzleFlashInstance?.gameObject?.SetActive(false);
                __instance._rangedWeapon.ShowImpactLocation(true);
                __instance._rangedWeapon._impactTargetInstance?.transform.GetChild(0).gameObject.SetActive(false);
                var light = __instance.gameObject.GetOrAddComponent<Light>();
                light.intensity = 50;
                light.color = new Color(1, 0, 0);
                TrailRedFx.TrailRed.SetParent(__instance._rangedWeapon.bulletPrefab.transform);
                return;
            }
            __instance._gunShotAudioEvent = "event:/SotF Events/player sounds/Weapons/PistolTactical/PistolTacticalFire";
            __instance._muzzleFlashInstance?.gameObject?.SetActive(true);
        }

        public static void Update(RangedWeaponController __instance)
        {
            if (__instance.GetComponent<SonsFMODEventEmitter>() == null)
                _fmodEmitter = __instance.gameObject.AddComponent<SonsFMODEventEmitter>();

            UpgradeFx(__instance, IsUpgraded);
            if (IsUpgraded)
            {
                __instance.GetItemData().AmmoDamageMult = 2f;
                _fmodEmitter.instance.set3DAttributes(__instance.gameObject.transform.position.To3DAttributes());
            }
        }
    }

    public class ShotgunUpgrade
    {
        public static bool IsUpgraded;
        static SonsFMODEventEmitter _fmodEmitter;

        public static void OnTriggerShotFiredAudio(ref ShotgunWeaponController __instance)
        {
            if (IsUpgraded)
            {
                AudioController.PlayBSound(_fmodEmitter, "event:/Shotgun/UpgradeShoot", AudioController.SoundType.Sfx);

                UnityEngine.Object.Instantiate(ShotgunUpgradeImpactFx.ShotgunUpgradeImpact,
                    __instance._rangedWeapon._impactTargetInstance.transform.position,
                    Quaternion.identity).GetComponent<ParticleSystem>().Play();
            }
        }

        public static void UpgradeFx(RangedWeaponController __instance, bool onoff)
        {
            if (onoff)
            {
                __instance._gunShotAudioEvent = "";
                __instance._muzzleFlashInstance?.gameObject?.SetActive(false);
                __instance._rangedWeapon.ShowImpactLocation(true);
                var light = __instance.gameObject.GetOrAddComponent<Light>();
                light.intensity = 50;
                light.color = new Color(0, 0.825f, 1);
                __instance.gameObject.AddComponent<Light>();
                TrailWaterFx.TrailWater.SetParent(__instance._rangedWeapon.bulletPrefab.transform);
                return;
            }
            __instance._gunShotAudioEvent = "event:/SotF Events/player sounds/shotgun_combat";
            __instance._muzzleFlashInstance?.gameObject?.SetActive(true);
        }

        public static void Update(RangedWeaponController __instance)
        {
            if (__instance.GetComponent<SonsFMODEventEmitter>() == null)
                _fmodEmitter = __instance.gameObject.AddComponent<SonsFMODEventEmitter>();

            UpgradeFx(__instance, IsUpgraded);
            if (IsUpgraded)
            {
                __instance.GetItemData().AmmoDamageMult = 2f;
                _fmodEmitter.instance.set3DAttributes(__instance.gameObject.transform.position.To3DAttributes());
            }
        }
    }
}
