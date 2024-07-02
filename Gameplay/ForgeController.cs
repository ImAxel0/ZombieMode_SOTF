using TheForest.Utils;
using UnityEngine;
using ZombieMode.Helpers;
using RedLoader;
using Sons.Gui.Input;
using ZombieMode.Core;
using ZombieMode.Libs;
using SonsSdk;
using Sons.Input;
using TheForest.Items.Utils;
using Sons.Items.Core;
using System.Collections;
using static ZombieMode.Helpers.ItemsIdManager;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class ForgeController : MonoBehaviour
{
    public static GameObject ForgeObject;
    static LinkUiElement _uiElement;
    public static int NeededScore = 4000;
    static Animator _controller;
    static bool _isUpgrading;

    static List<ItemsId> _forgeableItems = new()
    {
        { ItemsId.CompactPistol },
        { ItemsId.ShotgunPumpAction },
    };

    private void Start()
    {
        ForgeObject = Instantiate(ForgeGo.Forge,
            new Vector3(-1160.706f, 58.7f, 37.696f),
            Quaternion.Euler(0, 9, 0));

        ForgeObject.GetComponentInChildren<Light>().intensity = 67899;
        _uiElement = Interactable.Create(ForgeObject, 2f, Interactable.InteractableType.Open, ResourcesLoader.ResourceToTex("WeaponUpgradeIcon"));
        _controller = ForgeObject.GetComponentInChildren<Animator>();
    }

    private IEnumerator UpgradeWeapon(int itemId)
    {
        _isUpgrading = true;
        _uiElement.gameObject.SetActive(false);
        var itemData = ItemDatabaseManager.ItemById(itemId);
        LocalPlayer.Inventory.RemoveItem(itemId);
        var spawnedItem = Instantiate(itemData.PickupPrefab, ForgeObject.transform.Find("WeaponPos").position, Quaternion.Euler(0, 90, 270));
        _controller.Play(string.Empty);

        while (_controller.GetCurrentAnimatorStateInfo(0).IsName(string.Empty))
        {
            yield return null;
        }

        Destroy(spawnedItem);
        ItemUtils.SpawnItemPickup(itemData, ForgeObject.transform.Find("WeaponPos").position, Quaternion.Euler(0, 90, 270), 
            Vector3.zero, false, false, false, false, null, null, 1, new(Vector3.zero), new(Vector3.zero));

        switch (itemId)
        {
            case (int)ItemsIdManager.ItemsId.CompactPistol:
                WeaponsUpgrade.PistolUpgrade.IsUpgraded = true;
                break;
            case (int)ItemsIdManager.ItemsId.ShotgunPumpAction:
                WeaponsUpgrade.ShotgunUpgrade.IsUpgraded = true;
                break;
        }
        _uiElement.gameObject.SetActive(true);
        _isUpgrading = false;
    }

    private static bool CanForge()
    {
        if (_uiElement.IsActive && !_isUpgrading)
            return true;

        return false;
    }

    private void Update()
    {
        if (InputSystem.InputMapping.@default.Use.triggered && CanForge())
        {
            var itemId = LocalPlayer.Inventory.RightHandItem?._itemID;
            if (_forgeableItems.Contains((ItemsId)itemId))
            {
                if (WeaponsUpgrade.IsWeaponUpgraded((ItemsId)itemId))
                {
                    LocalPlayer.Sfx.PlayRemove();
                    SonsTools.ShowMessage($"Weapon is already upgraded");
                }
                else if (ScoreSystem.Score.Value >= NeededScore)
                {
                    ScoreSystem.DecScore(NeededScore);
                    UpgradeWeapon(LocalPlayer.Inventory.RightHandItem._itemID).RunCoro();
                }
                else
                {
                    LocalPlayer.Sfx.PlayRemove();
                    SonsTools.ShowMessage($"Need <color=yellow>{NeededScore}</color> score to upgrade the weapon");
                }
            }
            else
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage($"Equipped item can't be upgraded");
            }
        }
    }
}
