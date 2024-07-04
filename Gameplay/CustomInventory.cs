using RedLoader;
using TheForest.Utils;
using UnityEngine;
using static ZombieMode.Helpers.ItemsIdManager;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class CustomInventory : MonoBehaviour
{
    public static CustomInventory Instance { get; set; }
    private List<ItemsId> _mainItems = new()
    {
        { ItemsId.CompactPistol },
        { ItemsId.CombatKnife },
    };
    public List<ItemsId> MainItems { get { return _mainItems; } }

    private void Start()
    {
        Instance = this;
    }

    public void SetMainItem(int index, ItemsId itemId)
    {
        LocalPlayer.Inventory.RemoveItem(GetEquippedIndex());
        _mainItems[index] = itemId;
    }

    public int GetEquippedIndex()
    {
        int? index = _mainItems.IndexOf((ItemsId)LocalPlayer.Inventory.RightHandItem._itemID);
        if (index != null)
        {
            return index.Value;
        }
        return 0; // if no item is equipped select slot 0
    }

    public bool EquipItem(int index)
    {
        return LocalPlayer.Inventory.TryEquip((int)_mainItems[index], false);
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
