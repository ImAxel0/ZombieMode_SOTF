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

    private void Start()
    {
        Instance = this;
    }

    public void SetMainItem(int index, ItemsId itemId)
    {
        _mainItems[index] = itemId;
    }

    public int GetEquippedIndex()
    {
        return _mainItems.IndexOf((ItemsId)LocalPlayer.Inventory.RightHandItem._itemID);
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
    }
}
