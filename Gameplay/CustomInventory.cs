using RedLoader;
using TheForest.Utils;
using UnityEngine;
using static ZombieMode.Helpers.ItemsIdManager;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class CustomInventory : MonoBehaviour
{
    public static CustomInventory Instance { get; set; }
    private List<int> _mainItems = new()
    {
        { (int)ItemsId.CompactPistol },
        { (int)ItemsId.CombatKnife },
    };
    public List<int> MainItems { get { return _mainItems; } }

    private void Start()
    {
        Instance = this;
    }

    public void SetMainItem(int index, ItemsId itemId, ItemsId toRemove)
    {
        LocalPlayer.Inventory.RemoveItem((int)toRemove, 1, false, false);
        _mainItems[index] = (int)itemId;
    }

    public void SetMainItem(int index, ItemsId itemId)
    {
        _mainItems[index] = (int)itemId;
    }

    public int GetEquippedIndex()
    {
        if (LocalPlayer.Inventory.RightHandItem == null)
        {
            return -1;
        }
        return _mainItems.IndexOf(LocalPlayer.Inventory.RightHandItem._itemID);
    }

    public bool EquipItem(int index)
    {
        return LocalPlayer.Inventory.TryEquip(_mainItems[index], false);
    }

    public static void GiveAmmoFor(int itemId)
    {
        switch ((ItemsId)itemId)
        {
            case ItemsId.CompactPistol:
                LocalPlayer.Inventory.AddItem((int)ItemsId.PistolAmmo, 140);
                break;
            case ItemsId.Revolver:
                LocalPlayer.Inventory.AddItem((int)ItemsId.PistolAmmo, 140);
                break;
            case ItemsId.ShotgunPumpAction:
                LocalPlayer.Inventory.AddItem((int)ItemsId.BuckshotAmmo, 60);
                break;
            case ItemsId.Rifle:
                LocalPlayer.Inventory.AddItem((int)ItemsId.RifleAmmo, 40);
                break;
            case ItemsId.Crossbow:
                LocalPlayer.Inventory.AddItem((int)ItemsId.CrossbowAmmoBolt, 20);
                break;
            case ItemsId.TaserStick:
                LocalPlayer.Inventory.AddItem((int)ItemsId.Batteries, 1);
                break;
            case ItemsId.StunGun:
                LocalPlayer.Inventory.AddItem((int)ItemsId.StunGunAmmo, 60);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LocalPlayer.Inventory.TryEquip((int)ItemsId.Grenade, false);
        }
    }
}
