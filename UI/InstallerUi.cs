using UnityEngine;
using SUI;
using static SUI.SUI;
using static ZombieMode.Libs.AXSUI;
using SonsSdk;
using RedLoader;
using TMPro;
using ZombieMode.Libs;
using ZombieMode.Core;

namespace ZombieMode.UI;

[RegisterTypeInIl2Cpp]
public class InstallerUi : MonoBehaviour
{
    static GameObject InstallerGo;
    public static SContainerOptions InstallerPanel;
    public static Observable<bool> ShowInstall = new(false);
    public static Observable<bool> ShowUninstall = new(false);
    public static Observable<bool> ShowRepair = new(false);
    public static Observable<bool> ShowExit = new(false);

    public static void UiCreate()
    {
        InstallerPanel = AxCreateFillPanel("InstallerPanelContainer", Color.black.WithAlpha(0.6f)).OverrideSorting(100).Active(false);
        var image = SImage.Dock(EDockType.Fill).Texture(ResourcesLoader.ResourceToTex("DamageOverlay"));
        image.ImageObject.CrossFadeAlpha(0.1f, 0, false);
        InstallerPanel.Add(image);

        var mainInstaller = AxCreateMenuPanel("InstallerPanel", false, false, "ZombieMode Installer", new Vector2(1000, 500), AnchorType.MiddleCenter, new Color32(32, 32, 32, 255), EBackground.None)
        .OverrideSorting(200);

        Font font = new Font(@"C:\Users\Alex\Downloads\HeadlinerNo.45.ttf");
        TMP_FontAsset font_asset = TMP_FontAsset.CreateFontAsset(font);

        AxGetMenuTitle((SPanelOptions)mainInstaller).Font(font_asset).FontColor(Color.red.WithAlpha(0.7f));

        var main = AxGetMainContainer((SPanelOptions)mainInstaller);
        main.Add(AxTextDynamic(Installer.TextContent, 26));

        var bottomContainer = SContainer.Dock(EDockType.Bottom).Horizontal(10, "EX").Height(100).Padding(10, 10, 0, 10);
        mainInstaller.Add(bottomContainer);

        var progressContainer = SContainer.Dock(EDockType.Bottom).Horizontal(0, "CX").Height(100).BindVisibility(Installer.IsDownloading);
        mainInstaller.Add(progressContainer);

        bottomContainer.Add(AxMenuButton("Exit installer", Installer.OnExitInstaller_Click, Color.red.WithAlpha(0.4f), EBackground.RoundedStandard)
            .BindVisibility(ShowExit));
        bottomContainer.Add(AxMenuButton("Cancel download", Installer.OnCancelDownload_Click, Color.red.WithAlpha(0.4f), EBackground.RoundedStandard)
            .BindVisibility(Installer.IsDownloading));
        bottomContainer.Add(AxMenuButton("Uninstall", Installer.OnExitInstaller_Click, new Color(0.51f, 0.03f, 0.03f), EBackground.RoundedStandard)
            .BindVisibility(ShowUninstall));
        bottomContainer.Add(AxMenuButton("Repair installation", Installer.OnExitInstaller_Click, new Color(0.83f, 0.22f, 0.06f), EBackground.RoundedStandard)
            .BindVisibility(ShowRepair));
        bottomContainer.Add(AxMenuButton($"Start Download\n({Installer.DownloadSize})", Installer.OnDownload_ClickAsync, Color.green.WithAlpha(0.4f), EBackground.RoundedStandard)
            .BindVisibility(ShowInstall));

        var progressSlider = SContainer.Background(Color.black.WithAlpha(0.9f), EBackground.None).Horizontal(5, "CE").PaddingHorizontal(5)
                                - AxText("Progress %", 24);

        var slider = SSlider.Range(0, 100).Value(0).Bind(Installer.DownloadProgress).HOffset(10, -10).Format("0.0").Options(SSliderOptions.VisibilityMask.Readout);
        slider.SliderObject.interactable = false;
        progressSlider.Add(slider);
        progressContainer.Add(progressSlider);

        var downloadSpeed = SContainer.Background(Color.black.WithAlpha(0.9f), EBackground.None).PWidth(100).PaddingHorizontal(5)
                                - AxTextDynamic(Installer.DownloadSpeed);

        progressContainer.Add(downloadSpeed);

        InstallerPanel.Add(mainInstaller);

        InstallerGo = new();
        InstallerGo.DontDestroyOnLoad();
        InstallerGo.AddComponent<InstallerUi>();
    }

    public static void ShowInstaller()
    {
        InstallerPanel.Active(true);
    }

    public void Update()
    {
        if (Installer.IsDownloading.Value)
        {
            ShowInstall.Set(false);
            ShowUninstall.Set(false);
            ShowRepair.Set(false);
            ShowExit.Set(false);
            return;
        }

        if (Installer.CheckInstallation())
        {
            ShowInstall.Set(true);
            ShowUninstall.Set(true);
            ShowRepair.Set(true);
            ShowExit.Set(true);
        }
        else
        {
            ShowInstall.Set(true);
            ShowUninstall.Set(false);
            ShowRepair.Set(false);
            ShowExit.Set(true);
        }
    }
}
