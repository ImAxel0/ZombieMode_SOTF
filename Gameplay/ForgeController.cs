using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForest.Utils;
using UnityEngine;
using ZombieMode.Helpers;
using RedLoader;
using Sons.Gui.Input;
using ZombieMode.Core;
using UnityEngine.InputSystem.XR;
using ZombieMode.Libs;
using SonsSdk;
using Sons.Input;
using TheForest.Items.Utils;
using Sons.Items.Core;
using System.Collections;
using System.Diagnostics;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class ForgeController : MonoBehaviour
{
    public static GameObject ForgeObject;
    static LinkUiElement _uiElement;
    public static int NeededScore = 4000;
    static Animator _controller;

    private void Start()
    {
        ForgeObject = Instantiate(ForgeGo.Forge,
            new Vector3(-1100.2f, 59, 20),
            Quaternion.Euler(0, 0, 0));

        _uiElement = Interactable.Create(ForgeObject, 2f, Interactable.InteractableType.Open, ResourcesLoader.ResourceToTex("MysteryBoxInteract"));
        _controller = ForgeObject.GetComponentInChildren<Animator>();
    }

    private IEnumerator UpgradeWeapon(int itemId)
    {
        _uiElement.gameObject.SetActive(false);
        var itemData = ItemDatabaseManager.ItemById(itemId);
        LocalPlayer.Inventory.RemoveItem(itemId);
        var spawnedItem = Instantiate(itemData.PropPrefab, ForgeObject.transform.Find("WeaponPos").position, Quaternion.Euler(0, 90, 270));
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
    }

    private static bool CanForge()
    {
        if (_uiElement.IsActive)
            return true;

        return false;
    }

    private void Update()
    {
        if (InputSystem.InputMapping.@default.Use.triggered && CanForge())
        {
            if (ScoreSystem.Score.Value >= NeededScore)
            {
                ScoreSystem.DecScore(NeededScore);
                UpgradeWeapon(LocalPlayer.Inventory.RightHandItem._itemID).RunCoro();
            }
            else
            {
                SonsTools.ShowMessage($"Need <color=yellow>{NeededScore}</color> score to upgrade the weapon");
            }
        }
    }
}
