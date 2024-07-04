using Construction.Utils;
using RedLoader;
using Sons.Ai.Vail;
using Sons.Gui;
using Sons.Save;
using SonsSdk;
using SUI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZombieMode.Gameplay;
using ZombieMode.Libs;
using ZombieMode.UI;
using static SUI.SUI;
using static ZombieMode.Libs.AXSUI;

namespace ZombieMode.Core;

public class Loading
{
    public static SContainerOptions LoadingPanel;
    public static Observable<string> LoadingInfo = new("");
    public static Observable<float> LoadingProgress = new(0);

    private static Observable<string> _currentTip = new("");
    private static string[] _loadingTips = { 
        "Buy <color=yellow>perks</color> at the vending machines to increase your chance of surviving",
        "Exchange points for a random weapon at the <color=yellow>MysteryBox</color>",
        "Upgrade your weapons at the <color=yellow>Forge</color>",
        "Unlock doors to to discover new areas",
        "Buy items on walls for points",
        "Enemies have a random chance of dropping a <color=yellow>consumable</color> when killed",
        "Stay away from the lava",
        "Take headshots to gain more points",
    };

    public static void UiCreate()
    {
        LoadingPanel = AxCreateFillPanel("LoadingPanel", Color.black.WithAlpha(0)).OverrideSorting(101).Active(false)
            - SImage.Dock(EDockType.Fill).Texture(ResourcesLoader.ResourceToTex("ZombieModeLoading"));

        _currentTip.Value = _loadingTips[UnityEngine.Random.Range(0, _loadingTips.Length)];
        var tipsContainer = SContainer.Dock(EDockType.Bottom).Background(Color.black.WithAlpha(0.7f), EBackground.Sons).Height(50).Position(null, 200)
                            - AxTextDynamic(_currentTip, 40);

        LoadingPanel.Add(tipsContainer);

        var progressContainer = SContainer.Dock(EDockType.Bottom).Background(Color.black, EBackground.None).Horizontal(0, "CX").Height(50).Position(null, 80);

        var progressSlider = SContainer.Background(Color.black, EBackground.None).Horizontal(5, "CE").PaddingHorizontal(5)
                        - AxText("%", 40).Margin(20, 0, 0, 0);

        var slider = SSlider.Range(0, 100).Value(0).Bind(LoadingProgress).HOffset(10, -10).Format("0").Options(SSliderOptions.VisibilityMask.Readout);
        slider.SliderObject.interactable = false;

        var loadingInfo = SContainer.Background(Color.black, EBackground.None).PWidth(300).PaddingHorizontal(5)
                        - AxTextDynamic(LoadingInfo, 30);

        progressSlider.Add(slider);
        progressContainer.Add(progressSlider);
        progressContainer.Add(loadingInfo);
        LoadingPanel.Add(progressContainer);
    }

    public static void ToggleLoading()
    {
        LoadingPanel.Active(!LoadingPanel.Root.activeSelf);
    }

    public static void ExitToMenu()
    {
        Overlays.KillAll();
        AudioController.StopBSound(SoundManager.musicEmitter);
        SceneManager.LoadScene("SonsTitleScene", LoadSceneMode.Single);
    }

    public static IEnumerator LoadIntoGame()
    {
        Game.GameState = Game.GameStates.Loading;
        Resources.FindObjectsOfTypeAll<LoadMenu>()
        .First(x => x.name == "SinglePlayerLoadPanel")
        .LoadSlotActivated(0000000001, SaveGameManager.GetData<GameStateData>(SaveGameType.SinglePlayer, 0000000001));

        SceneLoadingIndicator loading = UnityEngine.Object.FindObjectOfType<SceneLoadingIndicator>();
        while (loading == null)
        {
            loading = UnityEngine.Object.FindObjectOfType<SceneLoadingIndicator>();
            yield return null;
        }

        ChangeTip(loading).RunCoro();
        while (loading._lastAmount < 0.99f)
        {
            LoadingInfo.Set("Loading scene...");
            LoadingProgress.Set(loading._lastAmount * 100f);
            yield return null;
        }

        while (VailActorManager.GetActiveCount() <= 0)
        {
            LoadingInfo.Set("Waiting for actors...");
            yield return null;
        }
        Game.OnSpawn();
    }

    private static IEnumerator ChangeTip(SceneLoadingIndicator loading)
    {
        while (loading != null)
        {
            yield return new WaitForSeconds(6f);
            var lastTip = _currentTip.Value;
            _currentTip.Value = _loadingTips[UnityEngine.Random.Range(0, _loadingTips.Length)];
            if (lastTip == _currentTip.Value)
            {
                _currentTip.Value = _loadingTips[UnityEngine.Random.Range(0, _loadingTips.Length)];
            }
        }
    }
}
