using UnityEngine;
using RedLoader;
using Sons.Input;
using Sons.Gui.Input;
using UnityEngine.UI;
using SonsSdk;
using ZombieMode.Libs;
using ZombieMode.Core;
using TheForest.Utils;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class DoorsManager : MonoBehaviour
{
    // identifier, gameobject
    public static Dictionary<string, GameObject> Doors = new();
    public static Dictionary<LinkUiElement, int> UiCost = new();

    public static float _interactionDistance = 2f;

    private static SonsFMODEventEmitter _fmodEmitter = new();

    public void Start()
    {
        var icon = ResourcesLoader.ResourceToTex("DoorIcon");
        DoorGo.Door.GetComponent<MeshRenderer>().sharedMaterial.shader = Shader.Find("Sons/Uber");

        var one = Instantiate(DoorGo.Door, DoorsData.Spa.DoorLeft.Item1, Quaternion.Euler(DoorsData.Spa.DoorLeft.Item2));
        one.transform.localScale = DoorsData.Spa.DoorLeft.Item3;
        //one.GetComponentInChildren<Text>().text = DoorsData.Spa.DoorLeft.Item4.ToString();
        //var ui1 = Interactable.Create(one, _interactionDistance, Interactable.InteractableType.Open, icon);
        //UiCost.Add(ui1, DoorsData.Spa.DoorLeft.Item4);
        //Doors.Add("SpaLeft", one);

        var two = Instantiate(DoorGo.Door, DoorsData.Spa.DoorRight.Item1, Quaternion.Euler(DoorsData.Spa.DoorRight.Item2));
        two.transform.localScale = DoorsData.Spa.DoorRight.Item3;
        two.GetComponentInChildren<Text>().text = DoorsData.Spa.DoorRight.Item4.ToString();
        var ui2 = Interactable.Create(two, _interactionDistance, Interactable.InteractableType.Open, icon);
        UiCost.Add(ui2, DoorsData.Spa.DoorRight.Item4);
        Doors.Add("SpaRight", two);

        var three = Instantiate(DoorGo.Door, DoorsData.Gym.DoubleDoor.Item1, Quaternion.Euler(DoorsData.Gym.DoubleDoor.Item2));
        three.transform.localScale = DoorsData.Gym.DoubleDoor.Item3;
        three.GetComponentInChildren<Text>().text = DoorsData.Gym.DoubleDoor.Item4.ToString();
        var ui3 = Interactable.Create(three, _interactionDistance, Interactable.InteractableType.Open, icon);
        UiCost.Add(ui3, DoorsData.Gym.DoubleDoor.Item4);
        Doors.Add("GymDouble", three);

        GameObject.Find(DoorsData.Spa.GlassDoors)?.SetActive(false);
        var four = Instantiate(DoorGo.Door, DoorsData.Gym.GlassDoor.Item1, Quaternion.Euler(DoorsData.Gym.GlassDoor.Item2));
        four.transform.localScale = DoorsData.Gym.GlassDoor.Item3;
        four.GetComponentInChildren<Text>().text = DoorsData.Gym.GlassDoor.Item4.ToString();
        var ui4 = Interactable.Create(four, _interactionDistance, Interactable.InteractableType.Open, icon);
        UiCost.Add(ui4, DoorsData.Gym.GlassDoor.Item4);
        Doors.Add("GymGlass", four);

        var five = Instantiate(DoorGo.Door, DoorsData.Spa.BigDoor.Item1, Quaternion.Euler(DoorsData.Spa.BigDoor.Item2));
        five.transform.localScale = DoorsData.Spa.BigDoor.Item3;
        five.GetComponentInChildren<Text>().text = DoorsData.Spa.BigDoor.Item4.ToString();
        var ui5 = Interactable.Create(five, _interactionDistance, Interactable.InteractableType.Open, icon);
        UiCost.Add(ui5, DoorsData.Spa.BigDoor.Item4);
        Doors.Add("SpaBig", five);
    }

    public static void OpenDoor(GameObject door)
    {
        string doorId = Doors.FirstOrDefault(x => x.Value == door).Key;

        switch (doorId)
        {
            case "SpaBig":
                SpawnSystem.EnableSpawnPoint(SpawnSystem.GymSpawns);
                break;
            case "GymDouble":
                SpawnSystem.EnableSpawnPoint(SpawnSystem.GymPoolSpawns);
                break;
            case "SpaLeft":
                SpawnSystem.EnableSpawnPoint(SpawnSystem.StairsHallSpawns);
                break;
            case "SpaRight":
                SpawnSystem.EnableSpawnPoint(SpawnSystem.StairsHallSpawns);
                break;
        }

        door.SetActive(false);
    }

    public void Update()
    {
        if (InputSystem.InputMapping.@default.Use.triggered)
        {
            foreach (var pair in UiCost)
            {
                if (pair.Key.IsActive)
                {
                    if (ScoreSystem.Score.Value >= pair.Value)
                    {
                        UiCost.Remove(pair.Key);
                        OpenDoor(pair.Key.transform.root.gameObject);
                        ScoreSystem.DecScore(pair.Value);
                        AudioController.PlayBSound(_fmodEmitter, "event:/Buying/door-open", AudioController.SoundType.Sfx);
                    }
                    else
                    {
                        LocalPlayer.Sfx.PlayRemove();
                        SonsTools.ShowMessage($"Needs <color=yellow>{pair.Value}</color> score to open");
                    }                  
                }
            }
        }
    }

    private void OnDestroy()
    {
        Doors.Clear();
        UiCost.Clear();
    }
}
