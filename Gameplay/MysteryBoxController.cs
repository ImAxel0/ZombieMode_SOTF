using UnityEngine;
using RedLoader;
using static ZombieMode.Helpers.ItemsIdManager;
using Sons.Gui.Input;
using Sons.Input;
using SonsSdk;
using Sons.Items.Core;
using TheForest.Items.Utils;
using Random = UnityEngine.Random;
using System.Collections;
using FMODUnity;
using ZombieMode.Libs;
using ZombieMode.Core;
using TheForest.Utils;
using System.Diagnostics;
using static SUI.SUI;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class MysteryBoxController : MonoBehaviour
{
    public static GameObject MysteryBox;
    static LinkUiElement _uiElement;
    public static int NeededScore = 950;
    public static bool IsBoxOpen;
    public static bool IsCyclingItems;
    static WaitForSeconds _cycleSpeed = new WaitForSeconds(0.1f);
    static WaitForSeconds _timeToWeapon = new WaitForSeconds(4f);
    static WaitForSeconds _timeToClose = new WaitForSeconds(7f);
    static WaitForSeconds _delayToReopen = new WaitForSeconds(1f);

    static Animator _controller;
    static Animation _itemMovement;
    static SonsFMODEventEmitter _fmodEmitter;

    static List<int> _misteryBoxItems = new List<int>()
    {
        (int)ItemsId.CompactPistol,
        (int)ItemsId.ShotgunPumpAction,
        (int)ItemsId.Revolver,
        (int)ItemsId.Rifle,
        (int)ItemsId.Crossbow,
        (int)ItemsId.Katana,
        (int)ItemsId.FireAxe,
        (int)ItemsId.ModernAxe,
        (int)ItemsId.TaserStick,
        (int)ItemsId.StunGun,
        //(int)CustomItemIDs.TeddyBear,
    };

    public void Start()
    {
        MysteryBox = Instantiate(MysteryBoxBundle.MysteryBox,
            new Vector3(-1100.2f, 59, 22),
            Quaternion.Euler(0, 100, 0));

        MysteryBox.GetChildren().ForEach(x =>
        {
            if (x.GetComponent<MeshRenderer>())
            {
                x.GetComponent<MeshRenderer>().sharedMaterial.shader = Shader.Find("HDRP/Lit");
            }
        });

        _uiElement = Interactable.Create(MysteryBox, 2.5f, Interactable.InteractableType.Open, ResourcesLoader.ResourceToTex("MysteryBoxInteract"));
        _controller = MysteryBox.GetComponentInChildren<Animator>();
        _itemMovement = MysteryBox.transform.Find("GunContainer/Gun").GetComponent<Animation>();
        _fmodEmitter = MysteryBox.AddComponent<SonsFMODEventEmitter>();
    }

    public static bool CanOpenBox()
    {
        if (_uiElement.IsActive && !IsBoxOpen)
            return true;

        return false;
    }

    public static IEnumerator OpenBox()
    {
        IsBoxOpen = true;
        _uiElement.gameObject.SetActive(false);

        AudioController.PlayBSound(_fmodEmitter,
            "event:/Mystery Box/open",
            AudioController.SoundType.Sfx);

        AudioController.PlayBSound(_fmodEmitter,
            "event:/Mystery Box/jingle",
            AudioController.SoundType.Sfx);

        _controller.Play("OpenLid");
        _itemMovement.Play();

        IsCyclingItems = true;
        CycleThroughItems().RunCoro();

        yield return _timeToWeapon;

        IsCyclingItems = false;

        Transform gunPos = MysteryBox.transform.Find("GunContainer/Gun");
        ItemData spawnedItemData = ItemDatabaseManager.ItemById(_misteryBoxItems[Random.Range(0, _misteryBoxItems.Count - 1)]);
        GameObject spawnedItem = ItemUtils.SpawnItemPickup(spawnedItemData, gunPos.position, gunPos.rotation, Vector3.zero, false, false, false, false, null, null, 1, new(Vector3.zero), new(Vector3.zero));
        spawnedItem.SetParent(gunPos, true);

        if (spawnedItem.name == "TeddyBear(Clone)(Clone)")
        {
            // play move box anim
            //MoveBox().RunCoro();

            yield return new WaitForSeconds(3);
            //Object.Destroy(spawnedItem);
            yield break;
        }

        if (!LocalPlayer.Inventory.Owns(spawnedItemData.Id))
        {
            int lastEquipped = -1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!LocalPlayer.Inventory.Owns(spawnedItemData.Id))
            {
                lastEquipped = CustomInventory.Instance.GetEquippedIndex();
                if (sw.Elapsed.TotalSeconds > _timeToClose.m_Seconds)
                {
                    break;
                }
                yield return null;
            }
            sw.Stop();

            // if item was picked up
            if (LocalPlayer.Inventory.Owns(spawnedItemData.Id))
            {
                switch ((ItemsId)spawnedItemData.Id)
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
                        LocalPlayer.Inventory.AddItem((int)ItemsId.CrossbowAmmoBolt, 30);
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
                CustomInventory.Instance.SetMainItem(lastEquipped, (ItemsId)spawnedItemData.Id);
            }
            var delay = _timeToClose.m_Seconds - sw.Elapsed.TotalSeconds;
            yield return new WaitForSeconds((float)delay);
        }
        else yield return _timeToClose;

        Destroy(spawnedItem);
        _controller.Play("CloseLid");
        AudioController.PlayBSound(_fmodEmitter,
            "event:/Mystery Box/close",
            AudioController.SoundType.Sfx);

        yield return _delayToReopen;

        IsBoxOpen = false;
        _uiElement.gameObject.SetActive(true);
    }

    private static IEnumerator CycleThroughItems()
    {
        Transform gunPos = MysteryBox.transform.Find("GunContainer/Gun");
        gunPos.gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameObject cycleItem;
        while (IsCyclingItems)
        {
            cycleItem = ItemUtils.SpawnItemPickup(ItemDatabaseManager.ItemById(_misteryBoxItems[Random.Range(0, _misteryBoxItems.Count - 1)]),
                gunPos.position, gunPos.rotation, Vector3.zero, false, false, false, false, null, null, 1, new(new Vector3(0, 0, 0)), new(new Vector3(0, 0, 0)));
            cycleItem.SetParent(gunPos, true);
            yield return _cycleSpeed;

            Destroy(cycleItem);
        }
    }

    public void Update()
    {
        TogglePanel("MysteryBoxPrompt", _uiElement.IsActive);

        if (InputSystem.InputMapping.@default.Use.triggered && CanOpenBox())
        {
            if (ScoreSystem.Score.Value >= NeededScore)
            {
                ScoreSystem.DecScore(NeededScore);
                OpenBox().RunCoro();
            }
            else
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage($"Need <color=yellow>{NeededScore}</color> score to open the box");
            }
        }
        _fmodEmitter.instance.set3DAttributes(MysteryBox.transform.position.To3DAttributes());
    }

    private void OnDestroy()
    {
        IsBoxOpen = false;
        IsCyclingItems = false;
    }
}
