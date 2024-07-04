using RedLoader;
using SonsSdk;
using TheForest.Utils;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using ZombieMode.Libs;
using ZombieMode.Core;
using Sons.Input;
using ZombieMode.UI;
using static SUI.SUI;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class VendingMachines : MonoBehaviour
{
    public static GameObject HealthCola;
    public static GameObject RushCola;
    public static GameObject JumpCola;

    public static int HealthCola_Score = 4000;
    public static int RushCola_Score = 3000;
    public static int JumpCola_Score = 2500;

    static float _activationDistance = 3f;
    static SonsFMODEventEmitter _fmodEmitter = new();

    private void Start()
    {
        HealthCola = Instantiate(
        HealthColaGo.HealthCola,
        new Vector3(-1147.5f, 65.7f, 29.6f),
        Quaternion.Euler(0, 280, 0));
        Interactable.Create(HealthCola, _activationDistance, Interactable.InteractableType.Take, ResourcesLoader.ResourceToTex("HealthColaIcon"));
        //AudioController.Play3DSound("VendingMachineEngine", AudioController.SoundType.Sfx, HealthCola.transform.position, 2f, true);
        //AudioController.Play3DSound("VendingMachineMusic", AudioController.SoundType.Music, HealthCola.transform.position, 6f, true);

        GameObject HealthColaLight = new GameObject("HealthColaLight");
        HealthColaLight.name  = "HealthColaLight";
        HealthColaLight.transform.parent = HealthCola.transform;
        HealthColaLight.transform.position = HealthCola.transform.position;
        HealthColaLight.AddComponent<Light>().type = LightType.Point;
        HDAdditionalLightData hdadditionalLightData = HealthColaLight.AddComponent<HDAdditionalLightData>();
        hdadditionalLightData.SetIntensity(5f, LightUnit.Lux);
        hdadditionalLightData.luxAtDistance = 100f;
        hdadditionalLightData.SetRange(15);
        hdadditionalLightData.affectsVolumetric = false;
        hdadditionalLightData.color = new Color32(140, 2, 0, 255);

        RushCola = Instantiate(
        RushColaGo.RushCola,
        new Vector3(-1174.57f, 58.67f, 12.28f),
        Quaternion.Euler(0, 100, 0));
        Interactable.Create(RushCola, _activationDistance, Interactable.InteractableType.Take, ResourcesLoader.ResourceToTex("RushColaIcon"));

        GameObject RushColaLight = new GameObject("RushColaLight");
        RushColaLight.name  = "RushColaLight";
        RushColaLight.transform.parent = RushCola.transform;
        RushColaLight.transform.position = RushCola.transform.position;
        RushColaLight.AddComponent<Light>().type = LightType.Point;
        HDAdditionalLightData hdadditionalLightData1 = RushColaLight.AddComponent<HDAdditionalLightData>();
        hdadditionalLightData1.SetIntensity(5f, LightUnit.Lux);
        hdadditionalLightData1.luxAtDistance = 100f;
        hdadditionalLightData1.SetRange(15);
        hdadditionalLightData1.affectsVolumetric = false;
        hdadditionalLightData1.color = new Color32(107, 231, 50, 255);

        JumpCola = Instantiate(
        JumpColaGo.JumpCola,
        new Vector3(-1096.7f, 66.5f, 14.1f),
        Quaternion.Euler(0, 222, 0));
        Interactable.Create(JumpCola, _activationDistance, Interactable.InteractableType.Take, ResourcesLoader.ResourceToTex("JumpColaIcon"));

        GameObject JumpColaLight = new GameObject("JumpColaLight");
        JumpColaLight.name  = "JumpColaLight";
        JumpColaLight.transform.parent = JumpCola.transform;
        JumpColaLight.transform.position = JumpCola.transform.position;
        JumpColaLight.AddComponent<Light>().type = LightType.Point;
        HDAdditionalLightData hdadditionalLightData2 = JumpColaLight.AddComponent<HDAdditionalLightData>();
        hdadditionalLightData2.SetIntensity(5f, LightUnit.Lux);
        hdadditionalLightData2.luxAtDistance = 100f;
        hdadditionalLightData2.SetRange(5);
        hdadditionalLightData2.affectsVolumetric = false;
        hdadditionalLightData2.color = new Color32(92, 164, 200, 255);
    }

    private static void BuyHealthCola()
    {
        PerksManager.GiveHealthCola().RunCoro();
        AudioController.PlayBSound(_fmodEmitter, "event:/Buying/wall-item", AudioController.SoundType.Sfx);
        ScoreSystem.DecScore(HealthCola_Score);
        LocalPlayer.Sfx.PlayDrinkFromWaterSource();
    }

    private static void BuyRushCola()
    {
        PerksManager.GiveRushCola().RunCoro();
        AudioController.PlayBSound(_fmodEmitter, "event:/Buying/wall-item", AudioController.SoundType.Sfx);
        ScoreSystem.DecScore(RushCola_Score);
        LocalPlayer.Sfx.PlayDrinkFromWaterSource();
    }

    private static void BuyJumpCola()
    {
        PerksManager.GiveJumpCola().RunCoro();
        AudioController.PlayBSound(_fmodEmitter, "event:/Buying/wall-item", AudioController.SoundType.Sfx);
        ScoreSystem.DecScore(JumpCola_Score);
        LocalPlayer.Sfx.PlayDrinkFromWaterSource();
    }

    private void Update()
    {
        if (!LocalPlayer._instance || Game.GameState != Game.GameStates.InGame) 
            return;

        TogglePanel("HealthColaPrompt", Interactable.GetUiElement(HealthCola).IsActive);
        TogglePanel("RushColaPrompt", Interactable.GetUiElement(RushCola).IsActive);
        TogglePanel("JumpColaPrompt", Interactable.GetUiElement(JumpCola).IsActive);

        if (InputSystem.InputMapping.@default.Interact.triggered && Interactable.GetUiElement(HealthCola).IsActive)
        {
            if (PerksManager.HasHealthCola.Value)
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage("Already have <color=red>Health Cola</color>");
                return;
            }

            if (ScoreSystem.Score.Value >= HealthCola_Score) BuyHealthCola();
            else
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage($"Need <color=yellow>{HealthCola_Score} $</color>");
            }
        }

        if (InputSystem.InputMapping.@default.Interact.triggered && Interactable.GetUiElement(RushCola).IsActive)
        {
            if (PerksManager.HasRushCola.Value)
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage("Already have <color=red>Rush Cola</color>");
                return;
            }

            if (ScoreSystem.Score.Value >= RushCola_Score) BuyRushCola();
            else
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage($"Need <color=yellow>{RushCola_Score} $</color>");
            }
        }

        if (InputSystem.InputMapping.@default.Interact.triggered && Interactable.GetUiElement(JumpCola).IsActive)
        {
            if (PerksManager.HasJumpCola.Value)
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage("Already have <color=red>Jump Cola</color>");
                return;
            }

            if (ScoreSystem.Score.Value >= JumpCola_Score) BuyJumpCola();
            else
            {
                LocalPlayer.Sfx.PlayRemove();
                SonsTools.ShowMessage($"Need <color=yellow>{JumpCola_Score} $</color>");
            }
        }
    }
}
