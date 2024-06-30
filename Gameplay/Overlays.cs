using SonsSdk;
using SUI;
using TheForest.Utils;
using UnityEngine;
using static SUI.SUI;
using static ZombieMode.UI.AXSUI;
using RedLoader;
using ZombieMode.Libs;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class Overlays : MonoBehaviour
{
    static List<SImageOptions> _overlaysImages = new();

    public static SImageOptions BloodImage;
    static readonly float _bloodTime = 2;

    public static SImageOptions FireImage;
    static readonly float _fireTime = 2;

    public enum OverlayTypes
    {
        Blood,
        Fire
    }

    public static void UiCreate()
    {
        var panel = AxCreateFillPanel("OverlayPanel", Color.black.WithAlpha(0));

        BloodImage = SImage.Texture(ResourcesLoader.ResourceToTex("DamageOverlay")).Dock(EDockType.Fill);
        _overlaysImages.Add(BloodImage);
        panel.Add(BloodImage);

        FireImage = SImage.Texture(ResourcesLoader.ResourceToTex("FlamesOverlay")).Dock(EDockType.Fill);
        _overlaysImages.Add(FireImage);
        panel.Add(FireImage);

        KillAll();
    }

    private static void GetOverlayInfo(OverlayTypes overlay, out SImageOptions image, out float time)
    {
        switch (overlay)
        {
            case OverlayTypes.Blood:
                image = BloodImage;
                time = _bloodTime;
                return;

            case OverlayTypes.Fire:
                image = FireImage;
                time = _fireTime;
                return;
        }
        image = null; time = 0;
    }

    public static void KillOverlay(OverlayTypes overlay)
    {
        GetOverlayInfo(overlay, out var image, out var time);
        image.ImageObject.CrossFadeAlpha(0, 0, false);
    }

    public static void KillAll()
    {
        _overlaysImages.ForEach(ov => ov.ImageObject.CrossFadeAlpha(0, 0, false));
    }

    public static void DoOverlay(OverlayTypes overlay, float toAlpha = 0, float startAlpha = 1)
    {
        /*
        if (overlay == OverlayTypes.Blood)
        {  
            return;
        }*/

        GetOverlayInfo(overlay, out var image, out var time);
        image.ImageObject.CrossFadeAlpha(startAlpha, 0, true);
        image.ImageObject.CrossFadeAlpha(toAlpha, time, true);
    }

    public void Update()
    {
        if (LocalPlayer.Vitals.Health._currentValue >= LocalPlayer.Vitals.Health.GetMax())
        {
            BloodImage.ImageObject.CrossFadeAlpha(0, 0, true);
            return;
        }
        else if (LocalPlayer.Vitals.Health._currentValue >= 80)
        {
            BloodImage.ImageObject.CrossFadeAlpha(Mathf.Clamp(5 / LocalPlayer.Vitals.Health._currentValue, 0, 1), 0, true);
            return;
        }
        BloodImage.ImageObject.CrossFadeAlpha(Mathf.Clamp(10 / LocalPlayer.Vitals.Health._currentValue, 0, 1), 0, true);
    }
}
