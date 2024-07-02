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

    public void Start()
    {
        _fmodEmitter = new SonsFMODEventEmitter();

        var katanaWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1112, 60, 12), Quaternion.Euler(0, 280, 0));
        katanaWall.GetComponentInChildren<Text>().text = $"Katana: {Katana.Cost}";
        katanaWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.Katana).UiData._icon;
        katanaWall.GetComponentInChildren<RawImage>().color = new(1, 1, 1, 0.3f);
        Katana.Ui = Interactable.Create(katanaWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.Katana).UiData._icon);
        ItemCostPair.Add(ItemsId.Katana, new(Katana.Ui, Katana.Cost));

        var modernAxeWall = Instantiate(WallItemBundle.WallItem, new Vector3(-1115, 60, -4.35f), Quaternion.Euler(0, 100, 0));
        modernAxeWall.GetComponentInChildren<Text>().text = $"Modern Axe: {ModernAxe.Cost}";
        modernAxeWall.GetComponentInChildren<RawImage>().texture = ItemDatabaseManager.ItemById((int)ItemsId.ModernAxe).UiData._icon;
        modernAxeWall.GetComponentInChildren<RawImage>().color = new(1, 1, 1, 0.3f);
        ModernAxe.Ui = Interactable.Create(modernAxeWall, 1.5f, Interactable.InteractableType.Open, ItemDatabaseManager.ItemById((int)ItemsId.ModernAxe).UiData._icon);
        ItemCostPair.Add(ItemsId.ModernAxe, new(ModernAxe.Ui, ModernAxe.Cost));
    }

    private static void BuyItem(ItemsId id, int cost)
    {
        LocalPlayer.Inventory.AddItem((int)id);
        LocalPlayer.Inventory.TryEquip((int)id, false);
        CustomInventory.Instance.SetMainItem(CustomInventory.Instance.GetEquippedIndex(), id);
        ScoreSystem.DecScore(cost);
        AudioController.PlayBSound(_fmodEmitter, "event:/Buying/wall-item", AudioController.SoundType.Sfx);
    }

    public void Update()
    {
        if (InputSystem.InputMapping.@default.Use.triggered)
        {
            foreach (var pair in ItemCostPair)
            {
                if (pair.Value.Item1.IsActive && ScoreSystem.Score.Value >= pair.Value.Item2)
                {
                    if (LocalPlayer.Inventory.Owns((int)pair.Key))
                    {
                        LocalPlayer.Sfx.PlayRemove();
                        SonsTools.ShowMessage("You already own this item");
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
}
