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
