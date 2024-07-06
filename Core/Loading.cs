using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
using Sons.Gameplay.GameSetup;
using Sons.Gui;
using Sons.Save;
using SonsSdk;
using Steamworks;
using SUI;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZombieMode.Gameplay;
using ZombieMode.Libs;
using static SUI.SUI;
using static ZombieMode.Libs.AXSUI;

namespace ZombieMode.Core;

public class Loading
{
    [DllImport("shell32.dll")]
    public static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    public static Guid localLowId = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");

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

    public static string GetKnownFolderPath(Guid knownFolderId)
    {
        IntPtr pszPath = IntPtr.Zero;
        try
        {
            int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
            if (hr >= 0)
                return Marshal.PtrToStringAuto(pszPath);
            throw Marshal.GetExceptionForHR(hr);
        }
        finally
        {
            if (pszPath != IntPtr.Zero)
                Marshal.FreeCoTaskMem(pszPath);
        }
    }

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
        Resources.FindObjectsOfTypeAll<LoadMenu>()
        .First(x => x.name == "SinglePlayerLoadPanel")
        .LoadSlotActivated(1234567890, SaveGameManager.GetData<GameStateData>(SaveGameType.SinglePlayer, 1234567890));

        Game.GameState = Game.GameStates.Loading;
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
