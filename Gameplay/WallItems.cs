using Sons.Gui.Input;
using UnityEngine;
using RedLoader;
using Sons.Input;
using TheForest.Utils;
using UnityEngine.UI;
using static ZombieMode.Helpers.ItemsIdManager;
using Sons.Items.Core;
using SonsSdk;
using ZombieMode.Libs;
using ZombieMode.Core;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class WallItems : MonoBehaviour
{
    static SonsFMODEventEmitter _fmodEmitter;
    static Dictionary<ItemsId, Tuple<LinkUiElement, int>> ItemCostPair = new();
    static Vector4 _color = new Vector4(1, 1, 1, 0.3f);
    static float _ammoDiscount = 0.3f;

    public struct Katana
    {
        public static LinkUiElement Ui;
        public static int Cost = 5000;
    }

    public struct ModernAxe
    {
        public static LinkUiElement Ui;
        public static int Cost = 2500;
    }

    public struct Shotgun
    {
        public static LinkUiElement Ui;
        public static int Cost = 6000;
    }

    public struct PistolAmmo
    {
        public static LinkUiElement Ui;
        public static int Cost = 1250;
    }

    public struct Grenade
    {
        public static LinkUiElement Ui;
        public static int Cost = 1500;
        public static int Amount = 3;
    }

    // -1159,488 60 0,5509 | -0 280 0 // gym left
    // -1164,199 60,1 49,9547 | -0 97,6236 0 // gym right corner
    // -1071,25 60,0899 -4,3558 | -0 189,0877 0 // stairs room glass

    public void Start()
    {
        _fmodEmitter = new SonsFMODEventEmitter();

        var katanaWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1112, 60, 12), Quaternion.Euler(0, 280, 0));
        katanaWall.GetComponentInChildren<Text>().text = $"Katana: {Katana.Cost}";
        katanaWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.Katana).UiData._icon;
        katanaWall.GetComponentInChildren<RawImage>().color = _color;
        Katana.Ui = Interactable.Create(katanaWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.Katana).UiData._icon);
        ItemCostPair.Add(ItemsId.Katana, new(Katana.Ui, Katana.Cost));

        var modernAxeWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1115, 60, -4.35f), Quaternion.Euler(0, 100, 0));
        modernAxeWall.GetComponentInChildren<Text>().text = $"Modern Axe: {ModernAxe.Cost}";
        modernAxeWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.ModernAxe).UiData._icon;
        modernAxeWall.GetComponentInChildren<RawImage>().color = _color;
        ModernAxe.Ui = Interactable.Create(modernAxeWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.ModernAxe).UiData._icon);
        ItemCostPair.Add(ItemsId.ModernAxe, new(ModernAxe.Ui, ModernAxe.Cost));

        var pistolAmmoWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1159.488f, 60, 0.5509f), Quaternion.Euler(0, 282, 0));
        pistolAmmoWall.GetComponentInChildren<Text>().text = $"Pistol ammo x{140}: {PistolAmmo.Cost}";
        pistolAmmoWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.PistolAmmo).UiData._icon;
        pistolAmmoWall.GetComponentInChildren<RawImage>().color = _color;
        PistolAmmo.Ui = Interactable.Create(pistolAmmoWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.PistolAmmo).UiData._icon);
        ItemCostPair.Add(ItemsId.PistolAmmo, new(PistolAmmo.Ui, PistolAmmo.Cost));

        var grenadeWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1164.199f, 60.1f, 49.9547f), Quaternion.Euler(0, 98, 0));
        grenadeWall.GetComponentInChildren<Text>().text = $"Grenade x3: {Grenade.Cost}";
        grenadeWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.Grenade).UiData._icon;
        grenadeWall.GetComponentInChildren<RawImage>().color = _color;
        Grenade.Ui = Interactable.Create(grenadeWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.Grenade).UiData._icon);
        ItemCostPair.Add(ItemsId.Grenade, new(Grenade.Ui, Grenade.Cost));

        var shotgunWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1071.25f, 60.089f, -4.35f), Quaternion.Euler(0, 190, 0));
        shotgunWall.GetComponentInChildren<Text>().text = $"Shotgun: {Shotgun.Cost}";
        shotgunWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.ShotgunPumpAction).UiData._icon;
        shotgunWall.GetComponentInChildren<RawImage>().color = _color;
        Shotgun.Ui = Interactable.Create(shotgunWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.ShotgunPumpAction).UiData._icon);
        ItemCostPair.Add(ItemsId.ShotgunPumpAction, new(Shotgun.Ui, Shotgun.Cost));
    }

    private static void BuyItem(ItemsId id, int cost)
    {
        switch (id)
        {
            case ItemsId.PistolAmmo:
                CustomInventory.GiveAmmoFor((int)ItemsId.CompactPistol);
                break;
            case ItemsId.Grenade:
                LocalPlayer.Inventory.AddItem((int)id, Grenade.Amount, true);
                break;
            case ItemsId.ShotgunPumpAction:
                CustomInventory.Instance.SetMainItem(CustomInventory.Instance.GetEquippedIndex(), (ItemsId)LocalPlayer.Inventory.RightHandItem._itemID);
                CustomInventory.GiveAmmoFor((int)ItemsId.ShotgunPumpAction);
                LocalPlayer.Inventory.AddItem((int)id);
                LocalPlayer.Inventory.TryEquip((int)id, false);               
                break;
            default:
                CustomInventory.Instance.SetMainItem(CustomInventory.Instance.GetEquippedIndex(), (ItemsId)LocalPlayer.Inventory.RightHandItem._itemID);
                LocalPlayer.Inventory.AddItem((int)id);
                LocalPlayer.Inventory.TryEquip((int)id, false);
                break;
        }
        ScoreSystem.DecScore(cost);
        AudioController.PlayBSound(_fmodEmitter, "event:/Buying/wall-item", AudioController.SoundType.Sfx);
    }

    public void Update()
    {
        if (InputSystem.InputMapping.@default.Use.triggered)
        {
            foreach (var pair in ItemCostPair)
            {
                if (pair.Value.Item1.IsActive && LocalPlayer.Inventory.Owns((int)pair.Key) && pair.Key != ItemsId.PistolAmmo && pair.Key != ItemsId.Grenade)
                {
                    if (ItemDatabaseManager.ItemById((int)pair.Key).Ammo != null)
                    {
                        // give ammo for weapon type but decrease score price
                        if (LocalPlayer.Inventory.Owns((int)pair.Key) && ScoreSystem.Score.Value >= pair.Value.Item2 * _ammoDiscount)
                        {
                            CustomInventory.GiveAmmoFor((int)pair.Key);
                            ScoreSystem.DecScore((int)(pair.Value.Item2 * _ammoDiscount));
                            AudioController.PlayBSound(_fmodEmitter, "event:/Buying/wall-item", AudioController.SoundType.Sfx);
                        }
                        else
                        {
                            LocalPlayer.Sfx.PlayRemove();
                            SonsTools.ShowMessage($"Needs <color=yellow>{(int)(pair.Value.Item2 * _ammoDiscount)}</color> to buy");
                        }
                    }
                    else
                    {
                        LocalPlayer.Sfx.PlayRemove();
                        SonsTools.ShowMessage("You already own this item");
                    }             
                }
                else if (pair.Value.Item1.IsActive && ScoreSystem.Score.Value >= pair.Value.Item2)
                {
                    if (LocalPlayer.Inventory.RightHandItem == null || !CustomInventory.Instance.MainItems.Contains(LocalPlayer.Inventory.RightHandItem._itemID))
                    {
                        LocalPlayer.Sfx.PlayRemove();
                        SonsTools.ShowMessage("A primary item must be equipped to buy");
                        break;
                    }
                    BuyItem(pair.Key, pair.Value.Item2);
                }
                else if (pair.Value.Item1.IsActive)
                {
                    LocalPlayer.Sfx.PlayRemove();
                    SonsTools.ShowMessage($"Needs <color=yellow>{pair.Value.Item2}</color> to buy");
                }                  
            }
        }
    }

    private void OnDestroy()
    {
        ItemCostPair.Clear();
    }
}
