using SUI;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using RedLoader;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class PerksManager : MonoBehaviour
{
    public static Observable<bool> HasRushCola = new(false);
    public static Observable<bool> HasJumpCola = new(false);
    public static Observable<bool> HasHealthCola = new(false);

    public static IEnumerator GiveRushCola()
    {
        HasRushCola.Value = true;
        LocalPlayer.FpCharacter.SetWalkSpeed(LocalPlayer.FpCharacter.WalkSpeed + 1f);
        LocalPlayer.FpCharacter.SetRunSpeed(6.5f);
        while (HasRushCola.Value)
        {
            yield return null;
        }
        LocalPlayer.FpCharacter.SetWalkSpeed(LocalPlayer.FpCharacter.WalkSpeed - 1f);
        LocalPlayer.FpCharacter.SetRunSpeed(5.4f);
    }

    public static IEnumerator GiveJumpCola()
    {
        HasJumpCola.Value = true;
        LocalPlayer.FpCharacter.JumpMultiplier = 2f;
        while (HasJumpCola.Value)
        {
            yield return null;
        }
        LocalPlayer.FpCharacter.JumpMultiplier = 1f;
    }

    public static IEnumerator GiveHealthCola()
    {
        HasHealthCola.Value = true;
        LocalPlayer.Vitals.Health.SetMax(150);
        LocalPlayer.Vitals.Health.SetCurrentValue(LocalPlayer.Vitals.Health.GetMax());
        while (HasHealthCola.Value)
        {
            yield return null;
        }
        LocalPlayer.Vitals.Health.SetMax(LocalPlayer.Vitals.Health._defaultValue);
    }

    private void OnDestroy()
    {
        HasRushCola.Set(false);
        HasJumpCola.Set(false);
        HasHealthCola.Set(false);
    }
}
