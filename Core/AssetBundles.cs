using SonsSdk.Attributes;
using UnityEngine;

namespace ZombieMode.Core;

[AssetBundle("misterybox")]
public static class MysteryBoxBundle
{
    [AssetReference("MisteryBox")]
    public static GameObject MysteryBox { get; set; }
}

[AssetBundle("wallitem")]
public static class WallItemBundle
{
    [AssetReference("WallItem")]
    public static GameObject WallItem { get; set; }
}

[AssetBundle("door")]
public static class DoorGo
{
    [AssetReference("Door")]
    public static GameObject Door { get; set; }
}

[AssetBundle("igniteexplosion")]
public static class IgniteExplosionFx
{
    [AssetReference("IgniteExplosion")]
    public static GameObject IgniteExplosion { get; set; }
}

[AssetBundle("trailred")]
public static class TrailRedFx
{
    [AssetReference("TrailRed")]
    public static GameObject TrailRed { get; set; }
}

[AssetBundle("trailwater")]
public static class TrailWaterFx
{
    [AssetReference("TrailWater")]
    public static GameObject TrailWater { get; set; }
}

[AssetBundle("pistolupgradeimpactfx")]
public static class PistolUpgradeImpactFx
{
    [AssetReference("PistolUpgradeImpactFx")]
    public static GameObject PistolUpgradeImpact { get; set; }
}

[AssetBundle("shotgunupgradeimpactfx")]
public static class ShotgunUpgradeImpactFx
{
    [AssetReference("ShotgunUpgradeImpactFx")]
    public static GameObject ShotgunUpgradeImpact { get; set; }
}

[AssetBundle("nukeconsumable")]
public static class NukeConsumableGo
{
    [AssetReference("NukeConsumable")]
    public static GameObject NukeConsumable { get; set; }
}

[AssetBundle("firesaleconsumable")]
public static class FireSaleConsumableGo
{
    [AssetReference("FireSaleConsumable")]
    public static GameObject FireSaleConsumable { get; set; }
}

[AssetBundle("imperceptibleconsumable")]
public static class ImperceptibleConsumableGo
{
    [AssetReference("ImperceptibleConsumable")]
    public static GameObject ImperceptibleConsumable { get; set; }
}

[AssetBundle("lockthemupconsumable")]
public static class LockThemUpConsumableGo
{
    [AssetReference("LockThemUpConsumable")]
    public static GameObject LockThemUpConsumable { get; set; }
}

[AssetBundle("doublescoreconsumable")]
public static class DoubleScoreConsumableGo
{
    [AssetReference("DoubleScoreConsumable")]
    public static GameObject DoubleScoreConsumable { get; set; }
}

[AssetBundle("healthcola")]
public static class HealthColaGo
{
    [AssetReference("HealthCola")]
    public static GameObject HealthCola { get; set; }
}

[AssetBundle("rushcola")]
public static class RushColaGo
{
    [AssetReference("RushCola")]
    public static GameObject RushCola { get; set; }
}

[AssetBundle("jumpcola")]
public static class JumpColaGo
{
    [AssetReference("JumpCola")]
    public static GameObject JumpCola { get; set; }
}

[AssetBundle("menuvideo")]
public static class MenuVideoGo
{
    [AssetReference("MenuVideo")]
    public static GameObject MenuVideo { get; set; }
}

[AssetBundle("lavaterrain")]
public static class LavaTerrainMat
{
    [AssetReference("LavaTerrain")]
    public static GameObject LavaTerrain { get; set; }
}

[AssetBundle("blackscribble")]
public static class BlackScribbleMat
{
    [AssetReference("BlackScribble")]
    public static GameObject BlackScribble { get; set; }
}

[AssetBundle("greenscribble")]
public static class GreenScribbleMat
{
    [AssetReference("GreenScribble")]
    public static GameObject GreenScribble { get; set; }
}

[AssetBundle("woodfloor")]
public static class WoodFloorMat
{
    [AssetReference("WoodFloor")]
    public static GameObject WoodFloor { get; set; }
}

[AssetBundle("obliquefloor")]
public static class ObliqueFloorMat
{
    [AssetReference("ObliqueFloor")]
    public static GameObject ObliqueFloor { get; set; }
}

[AssetBundle("damagetext")]
public static class DamageTextGo
{
    [AssetReference("DamageText")]
    public static GameObject DamageText { get; set; }
}

[AssetBundle("lavafloor")]
public static class LavaFloorMat
{
    [AssetReference("LavaFloor")]
    public static GameObject LavaFloor { get; set; }
}

[AssetBundle("forge")]
public static class ForgeGo
{
    [AssetReference("Forge")]
    public static GameObject Forge { get; set; }
}
