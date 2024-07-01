using Sons.Ai.Vail;
using SonsSdk;
using SUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForest.Utils;
using TheForest;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using ZombieMode.Gameplay;
using ZombieMode.Libs;
using System.Collections;
using System.Reflection;
using RedLoader;
using ZombieMode.UI;
using ZombieMode.Core;
using static SUI.SUI;

namespace ZombieMode.Helpers;

[RegisterTypeInIl2Cpp]
public class ZombieConsole : MonoBehaviour
{
    public static GameObject ConsoleUpdate;
    public static Observable<string> Command = new("");
    public static bool IsActive;
    public static bool ShowConsole;

    public static string Com(string commandName)
    {
        return $"<color=yellow>{commandName}</color>";
    }

    public static IEnumerator SilentInput()
    {
        KeyCode[] sequence = new KeyCode[]
        {
          KeyCode.Z,
          KeyCode.O,
          KeyCode.M,
          KeyCode.B,
          KeyCode.I,
          KeyCode.E,
          KeyCode.C,
          KeyCode.O,
          KeyCode.N,
          KeyCode.S,
          KeyCode.O,
          KeyCode.L,
          KeyCode.E,
        };

        int index = 0;
        while (true)
        {
            if (Input.GetKeyDown(sequence[index])) { index++; }
            else if (Input.anyKeyDown) { index = 0; }

            IsActive = (index == sequence.Length);
            if (IsActive)
            {
                SonsTools.ShowMessage("Zombie Console <color=green>enabled</color>", 5);
                LocalPlayer._instance.gameObject.AddComponent<ZombieConsole>();
                break;
            }
            yield return null;
        }
        yield break;
    }

    public static void InvokeMethod()
    {
        string[] command = Command.Value.Split(' ', StringSplitOptions.None);
        string methodName = command[0].ToLower();
        MethodInfo method = typeof(ZombieConsole).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null)
        {
            SonsTools.ShowMessage("Invalid command (<color=red>command not found</color>)");
            return;
        }

        if (command.Length > 2)
        {
            SonsTools.ShowMessage("Invalid command (<color=red>too many parameters</color>)");
            return;
        }

        if (command.Length == 2)
        {
            method?.Invoke(methodName, new object[] { command[1].ToLower() });
        }
        else
        {
            method?.Invoke(methodName, new object[] { "" });
        }
    }

    public static List<string> GetAvailableCommands()
    {
        List<string> availableCommands = new();
        MethodInfo[] methods = typeof(ZombieConsole).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var method in methods)
        {
            availableCommands.Add(method.Name);
        }
        return availableCommands;
    }

    private static void donuke(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("donuke")}");
            return;
        }
        Consumables.ConsumablesController.DoNuke();
        SonsTools.ShowMessage($"Ran command {Com("donuke")}");
    }

    private static void dodoublescore(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("dodoublescore")}");
            return;
        }
        Consumables.ConsumablesController.DoDoubleScore().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("dodoublescore")}");
    }

    private static void dofiresale(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("dofiresale")}");
            return;
        }
        Consumables.ConsumablesController.DoFireSale().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("dofiresale")}");
    }

    private static void dolockthemup(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("dolockthemup")}");
            return;
        }
        Consumables.ConsumablesController.DoLockThemUp().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("dolockthemup")}");
    }

    private static void doimperceptible(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("doimperceptible")}");
            return;
        }
        Consumables.ConsumablesController.DoImperceptible().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("doimperceptible")}");
    }

    private static void setscore(string score)
    {
        if (!int.TryParse(score, out int value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setscore score")}");
            return;
        }
        ScoreSystem.Score.Set(value);
        SonsTools.ShowMessage($"Ran command {Com($"setscore {value}")}");
    }

    private static void setscoremul(string mul)
    {
        if (!int.TryParse(mul.Replace(".", ","), out int value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setscoremul multiplier")}");
            return;
        }
        ScoreSystem.ScoreMultiplier = value;
        SonsTools.ShowMessage($"Ran command {Com($"setscoremul {value}")}");
    }

    private static void setsalemul(string mul)
    {
        if (!float.TryParse(mul.Replace(".", ","), out float value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setsalemul multiplier")}");
            return;
        }
        ScoreSystem.SaleMultiplier = value;
        SonsTools.ShowMessage($"Ran command {Com($"setsalemul {value}")}");
    }

    private static void setround(string round)
    {
        if (!int.TryParse(round, out int value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setround round")}");
            return;
        }
        Game.Round.Set(value);
        SonsTools.ShowMessage($"Ran command {Com($"setround {value}")}");
    }

    private static void spawnignite(string count)
    {
        if (string.IsNullOrEmpty(count))
            count = 1.ToString();

        if (!int.TryParse(count, out int value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("spawnignite count")}");
            return;
        }

        Transform mainCam = LocalPlayer.MainCamTr;
        RaycastHit hitSpawn;
        if (Physics.Raycast(mainCam.position, mainCam.forward, out hitSpawn, 10f))
        {
            for (int i = 0; i < value; i++)
            {
                var actor = SpawnSystem.CannibalSpawner.SpawnWorldSimActor(hitSpawn.point + mainCam.forward * 10, 0, 1);
                VailActor spawnedActor = VailActorManager.FindActiveActor(actor);
                ActorsManager.Ignite.SpawnIgnite(spawnedActor);
            }
        }
        SonsTools.ShowMessage($"Ran command {Com($"spawnignite {value}")}");
    }

    private static void spawngiant(string count)
    {
        if (string.IsNullOrEmpty(count))
            count = 1.ToString();

        if (!int.TryParse(count, out int value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("spawngiant count")}");
            return;
        }

        Transform mainCam = LocalPlayer.MainCamTr;
        RaycastHit hitSpawn;
        if (Physics.Raycast(mainCam.position, mainCam.forward, out hitSpawn, 10f))
        {
            for (int i = 0; i < value; i++)
            {
                var actor = SpawnSystem.CannibalSpawner.SpawnWorldSimActor(hitSpawn.point + mainCam.forward * 10, 0, 1);
                VailActor spawnedActor = VailActorManager.FindActiveActor(actor);
                //EnemiesManagement.TheGiant.InitGiant(spawnedActor);
            }
        }
        SonsTools.ShowMessage($"Ran command {Com($"spawngiant {value}")}");
    }

    private static void spawnspeedy(string count)
    {
        if (string.IsNullOrEmpty(count))
            count = 1.ToString();

        if (!int.TryParse(count, out int value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("spawnspeedy count")}");
            return;
        }

        Transform mainCam = LocalPlayer.MainCamTr;
        RaycastHit hitSpawn;
        if (Physics.Raycast(mainCam.position, mainCam.forward, out hitSpawn, 10f))
        {
            for (int i = 0; i < value; i++)
            {
                var actor = SpawnSystem.CannibalSpawner.SpawnWorldSimActor(hitSpawn.point + mainCam.forward * 10, 0, 1);
                VailActor spawnedActor = VailActorManager.FindActiveActor(actor);
                //EnemiesManagement.TheSpeedy.InitSpeedy(spawnedActor);
            }
        }
        SonsTools.ShowMessage($"Ran command {Com($"spawnspeedy {value}")}");
    }

    private static void killallenemies(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("killallenemies")}");
            return;
        }
        ActorsManager.KillAllEnemies();
        SonsTools.ShowMessage($"Ran command {Com("killallenemies")}");
    }

    private static void lockspawn(string str)
    {
        if (!bool.TryParse(str, out bool value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("lockspawn true|false")}");
            return;
        }
        SpawnSystem.LockSpawn = value;
        SonsTools.ShowMessage($"Ran command {Com($"lockspawn {value}")}");
    }

    private static void removedead(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("removedead")}");
            return;
        }
        ActorsManager.RemoveDeadBodies(0).RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("removedead")}");
    }

    private static void showhud(string onoff)
    {
        if (!bool.TryParse(onoff, out bool value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("showhud true|false")}");
            return;
        }
        HUD.HudPanel.Active(value);
        SonsTools.ShowMessage($"Ran command {Com($"showhud {value}")}");
    }

    private static void boxopen(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("boxopen")}");
            return;
        }
        MysteryBoxController.OpenBox().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("boxopen")}");
    }

    private static void boxmove(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("boxmove")}");
            return;
        }
        //MysteryBoxController.MoveBox(0).RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("boxmove")}");
    }

    private static void givehealthcola(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("givejuggernog")}");
            return;
        }
        //PerksManagement.GiveHealthCola().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("givejuggernog")}");
    }

    private static void giverushcola(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("giverush")}");
            return;
        }
        //PerksManagement.GiveRushCola().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("giverush")}");
    }

    private static void givejumpcola(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("givesuperjump")}");
            return;
        }
        //PerksManagement.GiveJumpCola().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("givesuperjump")}");
    }

    private static void giveallperks(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("giveallperks")}");
            return;
        }
        //PerksManagement.GiveHealthCola().RunCoro();
        //PerksManagement.GiveRushCola().RunCoro();
        //PerksManagement.GiveJumpCola().RunCoro();
        SonsTools.ShowMessage($"Ran command {Com("giveallperks")}");
    }

    private static void removeallperks(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("removeallperks")}");
            return;
        }
        //PerksManagement.HasHealthCola.Value = false;
        //PerksManagement.HasRushCola.Value = false;
        //PerksManagement.HasJumpCola.Value = false;
        SonsTools.ShowMessage($"Ran command {Com("removeallperks")}");
    }

    private static void removeperk(string perkName)
    {
        if (string.IsNullOrEmpty(perkName))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("removeperk perkname")}");
            return;
        }

        switch (perkName)
        {
            case "healthcola":
                //PerksManagement.HasHealthCola.Value = false;
                break;
            case "rushcola":
                //PerksManagement.HasRushCola.Value = false;
                break;
            case "jumpcola":
                //PerksManagement.HasJumpCola.Value = false;
                break;
            default:
                SonsTools.ShowMessage($"{perkName} is not a valid perk name");
                return;
        }
        SonsTools.ShowMessage($"Ran command {Com($"removeperk {perkName}")}");
    }

    private static void godmode(string onoff)
    {
        if (!bool.TryParse(onoff, out bool value))
        {
            SonsTools.ShowMessage($"Invalid parameter: usage {Com("godmode true|false")}");
            return;
        }
        string b = value ? "on" : "off";
        DebugConsole.Instance._godmode(b);
        SonsTools.ShowMessage($"Ran command {Com($"godmode {value}")}");
    }

    private static void matchrestart(string str)
    {
        // load new game
    }

    private static void setmasterlevel(string lvl)
    {
        if (!float.TryParse(lvl.Replace(".", ","), out float value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setmasterlevel 0 -> 1")}");
            return;
        }
        SoundManager.OnMasterLvlChange(Mathf.Clamp(value, 0f, 1f));
        SonsTools.ShowMessage($"Ran command {Com($"setmasterlevel {value}")}");
    }

    private static void setmusiclevel(string lvl)
    {
        if (!float.TryParse(lvl.Replace(".", ","), out float value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setmusiclevel 0 -> 1")}");
            return;
        }
        SoundManager.OnMusicLvlChange(Mathf.Clamp(value, 0f, 1f));
        SonsTools.ShowMessage($"Ran command {Com($"setmusiclevel {value}")}");
    }

    private static void setsfxlevel(string lvl)
    {
        if (!float.TryParse(lvl.Replace(".", ","), out float value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("setsfxlevel 0 -> 1")}");
            return;
        }
        SoundManager.OnSfxLvlChange(Mathf.Clamp(value, 0f, 1f));
        SonsTools.ShowMessage($"Ran command {Com($"setsfxlevel {value}")}");
    }

    private static void playsound(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("playsound soundidentifier")}");
            return;
        }
        AudioController.PlaySound(identifier, AudioController.SoundType.Sfx);
        SonsTools.ShowMessage($"Ran command {Com($"playsound {identifier}")}");
    }

    private static void stopsound(string identifier)
    {
        if (!AudioController.StopSound(identifier))
        {
            SonsTools.ShowMessage($"{identifier} is not a valid sound identifier");
            return;
        }
        SonsTools.ShowMessage($"Ran command {Com($"stopsound {identifier}")}");
    }

    private static void stopallsounds(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("stopallsounds")}");
            return;
        }
        AudioController.StopAllSounds();
        SonsTools.ShowMessage($"Ran command {Com("stopallsounds")}");
    }

    private static void debugspawnhealthcola(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("debugspawnhealthcola")}");
            return;
        }

        Transform mainCam = LocalPlayer.MainCamTr;
        RaycastHit hitSpawn;
        if (Physics.Raycast(mainCam.position, mainCam.forward, out hitSpawn, 10f))
        {
            /*VendingMachines.HealthCola = UnityEngine.Object.Instantiate(
                AssetLoader.gameObjects.Find(x => x.name == "HealthCola(Clone)"),
                hitSpawn.point + Vector3.forward * 10f,
                Quaternion.identity);*/
        }
        SonsTools.ShowMessage($"Ran command {Com("debugspawnhealthcola")}");
    }

    private static void debugspawnrushcola(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("debugspawnrushcola")}");
            return;
        }

        Transform mainCam = LocalPlayer.MainCamTr;
        RaycastHit hitSpawn;
        if (Physics.Raycast(mainCam.position, mainCam.forward, out hitSpawn, 10f))
        {
            /*VendingMachines.RushCola = UnityEngine.Object.Instantiate(
                AssetLoader.gameObjects.Find(x => x.name == "RushCola(Clone)"),
                hitSpawn.point + Vector3.forward * 10f,
                Quaternion.identity);*/
        }
        SonsTools.ShowMessage($"Ran command {Com("debugspawnrushcola")}");
    }

    private static void debugspawnjumpcola(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("debugspawnjumpcola")}");
            return;
        }

        Transform mainCam = LocalPlayer.MainCamTr;
        RaycastHit hitSpawn;
        if (Physics.Raycast(mainCam.position, mainCam.forward, out hitSpawn, 10f))
        {
            /*VendingMachines.JumpCola = UnityEngine.Object.Instantiate(
                AssetLoader.gameObjects.Find(x => x.name == "JumpCola(Clone)"),
                hitSpawn.point + Vector3.forward * 10f,
                Quaternion.identity);*/
        }
        SonsTools.ShowMessage($"Ran command {Com("debugspawnjumpcola")}");
    }

    static List<GameObject> _spawnPointsLight = new();
    private static void debugspawnpoints(string str)
    {
        if (!bool.TryParse(str, out var value))
        {
            SonsTools.ShowMessage($"Invalid parameter, usage: {Com("showspawnpoints true|false")}");
            return;
        }

        if (value)
        {
            _spawnPointsLight.ForEach(UnityEngine.Object.Destroy);
            _spawnPointsLight.Clear();
            foreach (var spawnpoint in SpawnSystem.SpawnPointsPos)
            {
                GameObject spawnpointLight = new GameObject("spawnpointLight")
                {
                    name = "SpawnpointLight",
                };
                spawnpointLight.transform.position = spawnpoint;
                spawnpointLight.AddComponent<Light>().type = LightType.Point;
                HDAdditionalLightData hdadditionalLightData = spawnpointLight.AddComponent<HDAdditionalLightData>();
                hdadditionalLightData.SetIntensity(5f, LightUnit.Lux);
                hdadditionalLightData.luxAtDistance = 100f;
                hdadditionalLightData.SetRange(5);
                hdadditionalLightData.affectsVolumetric = false;
                hdadditionalLightData.color = Color.red;
                _spawnPointsLight.Add(spawnpointLight);
            }
            _spawnPointsLight.ForEach(light => { light.SetActive(true); });
        }
        else
        {
            if (_spawnPointsLight.Count <= 0) return;
            _spawnPointsLight.ForEach(light => { light.SetActive(false); });
        }

        SonsTools.ShowMessage($"Ran command {Com($"showspawnpoints {value}")}");
    }

    private static void unlockdoors(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("unlockdoors")}");
            return;
        }
        //DoorsManager.UnlockDoors();
        SonsTools.ShowMessage($"Ran command {Com("unlockdoors")}");
    }

    private static void turnpoweron(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            SonsTools.ShowMessage($"Parameter is not required, usage: {Com("turnpoweron")}");
            return;
        }
        //PowerManagement.EnablePower();
        SonsTools.ShowMessage($"Ran command {Com("turnpoweron")}");
    }

    public static void ToggleSurvivalConsole()
    {
        Command.Value = "";
        ShowConsole = !ShowConsole;
        TogglePanel(ZombieConsoleUi.ZOMBIE_CONSOLE_ID, ShowConsole);
        if (ShowConsole)
        {
            //AudioController.PlaySound("UiInteract", AudioController.SoundType.Sfx);
            ZombieConsoleUi.ConsoleInput.InputFieldObject.Select();
        }
    }

    public void Update()
    {
        if (!IsActive) return;

        if (Input.GetKeyDown(KeyCode.F4))
            ToggleSurvivalConsole();

        if (ShowConsole)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                InvokeMethod();
        }
    }
}