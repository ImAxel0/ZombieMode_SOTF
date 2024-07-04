using RedLoader;
using UnityEngine;
using SUI;
using static SUI.SUI;
using static ZombieMode.Libs.AXSUI;
using SonsSdk;
using TMPro;
using UnityEngine.SceneManagement;
using Sons.Loading;
using UnityEngine.Video;
using Endnight.Utilities;
using Sons.Music;
using System.Collections;
using ZombieMode.Libs;
using ZombieMode.UI;

namespace ZombieMode.Core;

[RegisterTypeInIl2Cpp]
public class ZMainMenu : MonoBehaviour
{
    public static SContainerOptions SonsPanel;
    public static SContainerOptions ZombiePanel;
    public static SContainerOptions BlackTransitionPanel;
    static GameObject _videoPlayer;
    static Vector3 _cameraPos;
    static SSliderOptions _transitionSlider;

    public static void UiCreate()
    {
        SonsPanel = AxCreatePanel("SonsPanel", false, new Vector2(300, 100), AnchorType.TopRight, Color.black.WithAlpha(0), EBackground.RoundedStandard)
            .Position(-310, -110).Vertical(5, "EC").OverrideSorting(50)
            - AxMenuButton("Play ZombieMode", () => { ToggleMenu().RunCoro(); }, new Color(0.72f, 0.25f, 0.25f), EBackground.RoundedStandard)
            - AxMenuButton("Installer", InstallerUi.ShowInstaller, new Color(0.48f, 0.16f, 0.16f), EBackground.RoundedStandard);

        TMP_FontAsset font_asset = TMP_FontAsset.CreateFontAsset(UiManager.Headliner45);

        ZombiePanel = AxCreateFillPanel("ZombiePanel", Color.black.WithAlpha(0)).OverrideSorting(100).Active(false);

        var buttonsCt = AxContainer(AnchorType.BottomLeft, new Vector2(400, 500), Color.black.WithAlpha(0)).Vertical(40, "EC").Padding(0, 50, 10, 10)
            - AxMenuButtonText("Play", () => { Loading.ToggleLoading(); ToggleMenu(true, false).RunCoro(); Settings.ShowSettings(false); Loading.LoadIntoGame().RunCoro(); }, 60)["Play_get"].As<SButtonOptions>().Font(font_asset)
            - AxMenuButtonText("Settings", () => { Settings.ShowSettings(true); }, 60)["Settings_get"].As<SButtonOptions>().Font(font_asset)
            - AxMenuButtonText("Exit", () => { ToggleMenu().RunCoro(); }, 60)["Exit_get"].As<SButtonOptions>().FontColor(Color.red).Font(font_asset);
        ZombiePanel.Add(buttonsCt);

        var creditsCt = AxContainer(AnchorType.BottomRight, new Vector2(300, 100), Color.black.WithAlpha(0))
            - AxMenuButtonText("Credits", Credits.ToggleCreditsPanel, 60)["Credits_get"].As<SButtonOptions>().Font(font_asset);
        ZombiePanel.Add(creditsCt);

        BlackTransitionPanel = AxCreateFillPanel("BlackScreen", Color.black).OverrideSorting(999).Active(false)
            - SImage.Dock(EDockType.Fill).Texture(ResourcesLoader.ResourceToTex("TransitionImage"));

        var progressContainer = SContainer.Dock(EDockType.Bottom).Background(Color.black, EBackground.None).Horizontal(0, "CX").Height(50).Position(null, 80);

        var progressSlider = SContainer.Background(Color.black, EBackground.None).Horizontal(5, "CE").PaddingHorizontal(5)
                        - AxText("%", 40).Margin(20, 0, 0, 0);

        _transitionSlider = SSlider.Range(0, 100).Value(0).HOffset(10, -10).Format("0").Options(SSliderOptions.VisibilityMask.Readout);
        _transitionSlider.SliderObject.interactable = false;

        progressSlider.Add(_transitionSlider);
        progressContainer.Add(progressSlider);
        BlackTransitionPanel.Add(progressContainer);
    }

    public static IEnumerator ToggleMenu(bool isPlay = false, bool stopMusic = true)
    {
        var onoff = !ZombiePanel.Root.activeSelf;
        SetSonsUi(!onoff);
        VideoPlay(onoff);

        if (!isPlay)
        {
            MusicManager.Stop();
            BlackTransitionPanel.Active(true);

            int progress = 0;
            while (progress != 100)
            {
                progress += 2;
                _transitionSlider.Value(progress);
                yield return new WaitForSeconds(0.001f);
            }

            BlackTransitionPanel.Active(false);
            _transitionSlider.Value(0);
        }

        if (onoff)
        {
            //GameObject.Find("HelicopterAudioEmitter").GetComponent<FMOD_StudioEventEmitter>().Stop();
            SceneManager.GetSceneByName(SonsSceneManager.TitleSceneName).GetRootGameObjects().FirstWithName("TwinsTitleScene").SetActive(false);
            AudioController.PlayBSound(SoundManager.musicEmitter, "event:/Music/ZombieModeSoundtrack", AudioController.SoundType.Music);
            SonsPanel.Active(false);
        }
        else
        {
            if (stopMusic)
            {
                SoundManager.musicEmitter.Stop();
                SceneManager.GetSceneByName(SonsSceneManager.TitleSceneName).GetRootGameObjects().FirstWithName("TwinsTitleScene").SetActive(true);
                //GameObject.Find("HelicopterAudioEmitter").GetComponent<FMOD_StudioEventEmitter>().Play();
                MusicManager.Play();
            }
            SonsPanel.Active(true);
        }
        ZombiePanel.Active(onoff);
    }

    private static void VideoPlay(bool onoff)
    {
        Camera.main.gameObject.GetOrAddComponent<ZMainMenu>().enabled = false;
        if (onoff)
        {
            _cameraPos = Camera.main.transform.localPosition;
            Camera.main.gameObject.GetComponent<ZMainMenu>().enabled = true;
            _videoPlayer = Instantiate(MenuVideoGo.MenuVideo);
            _videoPlayer.GetComponent<VideoPlayer>().targetCamera = Camera.main;
            _videoPlayer.GetComponent<VideoPlayer>().Play();
            return;
        }
        Camera.main.gameObject.GetComponent<ZMainMenu>().enabled = false;
        Camera.main.gameObject.transform.localPosition = _cameraPos;
        Destroy(_videoPlayer);
    }

    private static void SetSonsUi(bool onoff)
    {
        var titleScene = SceneManager.GetSceneByName(SonsSceneManager.TitleSceneName);

        var gameObjects = titleScene.GetRootGameObjects().Where(go => go.layer == LayerMask.NameToLayer("UI") || go.name == "VersionReadout");

        foreach (var go in gameObjects)
            go.SetActive(onoff);
    }

    public void Update()
    {
        Camera.main.transform.position = Vector3.zero;
    }
}
