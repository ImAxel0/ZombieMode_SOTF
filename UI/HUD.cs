﻿using SonsSdk;
using SUI;
using UnityEngine;
using RedLoader;

using static SUI.SUI;
using static ZombieMode.Libs.AXSUI;
using TheForest.Utils;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using ZombieMode.Libs;
using ZombieMode.Gameplay;
using Sons.Items.Core;

namespace ZombieMode.UI;

[RegisterTypeInIl2Cpp]
public class HUD : MonoBehaviour
{
    public static SContainerOptions HudPanel;
    public static SContainerOptions AmmoPanel;
    public static Observable<string> AmmoInfo = new("<size=80>12</size><size=60>/120</size>");
    static Observable<bool> _showAmmo = new(false);
    public static Observable<string> ScoreInfo = new("<size=40>1000</size>");
    public static Observable<Texture> EquippedItemIcon = new(new Texture());
    public static Observable<Texture> SecondaryItemIcon = new(new Texture());
    public static Observable<string> RoundInfo = new("<size=180>1</size>");
    public static List<SContainerOptions> TextInfoPrompts = new();

    static SLabelOptions _roundText;

    static Texture _closedFist;

    public static void UiCreate()
    {
        TMP_FontAsset font_asset = TMP_FontAsset.CreateFontAsset(UiManager.Headliner45);

        HudPanel = AxCreateFillPanel("ZombieModeHud", Color.black.WithAlpha(0)).Active(false).OverrideSorting(200);

        // consumables panel
        var consumablesBox = AxCreatePanel("ConsumablesBox", false, new Vector2(850, 120), AnchorType.TopCenter, Color.black.WithAlpha(0), EBackground.None).Horizontal();
        consumablesBox.Add(OnScreenIcon(ResourcesLoader.ResourceToTex("DoubleScoreIcon")).BindVisibility(Consumables.IsDoubleScore));
        consumablesBox.Add(OnScreenIcon(ResourcesLoader.ResourceToTex("FireSaleIcon")).BindVisibility(Consumables.IsFireSale));
        consumablesBox.Add(OnScreenIcon(ResourcesLoader.ResourceToTex("ImperceptibleIcon")).BindVisibility(Consumables.IsImperceptible));
        consumablesBox.Add(OnScreenIcon(ResourcesLoader.ResourceToTex("LockThemUpIcon")).BindVisibility(Consumables.IsLockThemUp));
        HudPanel.Add(consumablesBox);

        // BottomRight
        var bottomRight = AxCreatePanel("ZombieModeBottomRight", false, new Vector2(600, 300), AnchorType.BottomRight, Color.black.WithAlpha(0));
        var splatter = SImage.Texture(ResourcesLoader.ResourceToTex("ink-splatter")).Dock(EDockType.Fill);
        splatter.ImageObject.color = Color.white.WithAlpha(0.5f);
        bottomRight.Add(splatter);

        AmmoPanel = SContainer.Dock(EDockType.Fill).Background(Color.black.WithAlpha(0), EBackground.None).Vertical(0, "EX");

        var scoreCt = SContainer.Dock(EDockType.Fill).Height(100)
            - AxTextDynamic(ScoreInfo, 40, TextAlignmentOptions.Bottom).FontColor(Color.white.WithAlpha(0.5f));

        var ammoCt = SContainer.Dock(EDockType.Fill).Horizontal(0, "EE").Height(200)
            - AxTextDynamic(AmmoInfo, 18).FontColor(Color.white.WithAlpha(0.5f)).BindVisibility(_showAmmo);

        var itemIcon = SImage.Bind(EquippedItemIcon).Dock(EDockType.Fill).AspectRatio(AspectRatioFitter.AspectMode.HeightControlsWidth);
        itemIcon.ImageObject.color = Color.white.WithAlpha(0.5f);
        EquippedItemIcon.Set(ResourcesLoader.ResourceToTex("closed-fist"));

        var secondaryItemIcon = SImage.Bind(SecondaryItemIcon).Dock(EDockType.Fill).AspectRatio(AspectRatioFitter.AspectMode.HeightControlsWidth);
        secondaryItemIcon.ImageObject.color = Color.white.WithAlpha(0.2f);
        secondaryItemIcon.ImageObject.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        ammoCt.Add(itemIcon);
        ammoCt.Add(secondaryItemIcon);
        AmmoPanel.Add(scoreCt);
        AmmoPanel.Add(ammoCt);
        bottomRight.Add(AmmoPanel);
        HudPanel.Add(bottomRight);

        // BottomLeft
        var bottomLeft = AxCreatePanel("ZombieModeBottomLeft", false, new Vector2(250, 300), AnchorType.BottomLeft, Color.black.WithAlpha(0)).Vertical(20, "EX");

        var perksCt = SContainer.Dock(EDockType.Fill).Background(Color.black.WithAlpha(0), EBackground.None).Height(50).Horizontal(0, "EE").PaddingHorizontal(5)
            - SImage.Dock(EDockType.Fill).AspectRatio(AspectRatioFitter.AspectMode.HeightControlsWidth).Texture(ResourcesLoader.ResourceToTex("HealthColaIcon"))
            .BindVisibility(PerksManager.HasHealthCola)
            - SImage.Dock(EDockType.Fill).AspectRatio(AspectRatioFitter.AspectMode.HeightControlsWidth).Texture(ResourcesLoader.ResourceToTex("RushColaIcon"))
            .BindVisibility(PerksManager.HasRushCola)
            - SImage.Dock(EDockType.Fill).AspectRatio(AspectRatioFitter.AspectMode.HeightControlsWidth).Texture(ResourcesLoader.ResourceToTex("JumpColaIcon"))
            .BindVisibility(PerksManager.HasJumpCola);

        perksCt.Root.GetChildren().ForEach(x => x.GetComponent<RawImage>().color = Color.white.WithAlpha(0.5f));

        var roundCt = SContainer.Dock(EDockType.Fill).Background(Color.black.WithAlpha(0), EBackground.None).Height(200);
        _roundText = AxTextDynamic(RoundInfo, 180).FontColor(Color.red.WithAlpha(0.5f)).Font(font_asset);
        roundCt.Add(_roundText);

        bottomLeft.Add(perksCt);
        bottomLeft.Add(roundCt);
        HudPanel.Add(bottomLeft);

        CreatePrompts();
    }

    private static void CreatePrompts()
    {
        InfoTextBox("MysteryBoxPrompt", $"Mystery Box [Cost: <color=yellow>{MysteryBoxController.NeededScore} $</color>]");
        InfoTextBox("HealthColaPrompt", $"Health Cola [Cost: <color=yellow>{VendingMachines.HealthCola_Score} $</color>]");
        InfoTextBox("RushColaPrompt", $"Rush Cola [Cost: <color=yellow>{VendingMachines.RushCola_Score} $</color>]");
        InfoTextBox("JumpColaPrompt", $"Jump Cola [Cost: <color=yellow>{VendingMachines.JumpCola_Score} $</color>]");
        InfoTextBox("ForgePrompt", $"Upgrade weapon [Cost: <color=yellow>{ForgeController.NeededScore} $</color>]");
    }

    private static void InfoTextBox(string identifier, string text)
    {
        var prompt = AxCreatePanel(identifier, false, new Vector2(800, 60), AnchorType.BottomCenter, Color.black.WithAlpha(0.6f), EBackground.Sons).Position(null, 30).Active(false)
            - SLabel.RichText(text).FontAutoSize(true).Dock(EDockType.Fill).Alignment(TextAlignmentOptions.Center);
        TextInfoPrompts.Add(prompt);
    }

    private static SContainerOptions OnScreenIcon(Texture icon)
    {
        var ct = SContainer.Background(Color.black.WithAlpha(0), EBackground.None);
        var _icon = SImage.Texture(icon).AspectRatio(AspectRatioFitter.AspectMode.HeightControlsWidth).FlexHeight(1);
        _icon.ImageObject.color = Color.white.WithAlpha(0.5f);
        ct.Add(_icon);
        return ct;
    }

    public static void DisableGameHud()
    {
        Sons.Gui.UiManager.SetShowMiniMapHud(false);
        Sons.Gui.UiManager.SetShowVitals(false);
    }

    public static IEnumerator RoundCrossFade()
    {
        for (int i = 0; i < 6; i++)
        {
            _roundText.TextObject.CrossFadeAlpha(0, 0, false);
            yield return new WaitForSeconds(1.5f);
        }
    }

    public void Update()
    {
        if (_closedFist == null)
        {
            _closedFist = ResourcesLoader.ResourceToTex("closed-fist");
        }

        ScoreInfo.Set(ScoreSystem.Score.Value.ToString());
        RoundInfo.Set(Game.Round.Value.ToString());

        if (LocalPlayer.Inventory.RightHandItem?.Data._weaponType == Sons.Items.Core.ItemData.WeaponType.AmmoBased)
        {
            AmmoInfo.Set($"<size=80>{Player.RemainingAmmo.Value}</size><size=60>/{Player.TotalAmmo.Value}</size>");
            _showAmmo.Set(true);
        }
        else _showAmmo.Set(false);

        if (LocalPlayer.Inventory.RightHandItem != null)
        {
            EquippedItemIcon.Set(LocalPlayer.Inventory.RightHandItem.Data.UiData._icon);
        }
        else EquippedItemIcon.Set(_closedFist);

        int equippedIndex = CustomInventory.Instance.GetEquippedIndex();
        if (equippedIndex == 0)
        {
            SecondaryItemIcon.Set(ItemDatabaseManager.ItemById(CustomInventory.Instance.MainItems[1]).UiData._icon);
        }
        else if (equippedIndex == 1)
        {
            SecondaryItemIcon.Set(ItemDatabaseManager.ItemById(CustomInventory.Instance.MainItems[0]).UiData._icon);
        }      

        _roundText.TextObject.CrossFadeAlpha(1, 0.5f, false);
        _roundText.TextObject.CrossFadeAlpha(0.5f, 0.5f, false);
    }

    private void OnDestroy()
    {
        // disable all prompts on returning to menu
        TextInfoPrompts.ForEach(p => { p.Active(false); });
        TextInfoPrompts.Clear();
    }
}
