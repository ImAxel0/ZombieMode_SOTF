using SonsSdk;
using UnityEngine;
using RedLoader;
using UnityEngine.SceneManagement;
using FMODUnity;
using ZombieMode.Libs;
using ZombieMode.Core;

namespace ZombieMode.Gameplay;

[RegisterTypeInIl2Cpp]
public class SceneMaterialsSwap : MonoBehaviour
{
    static List<SonsFMODEventEmitter> _lavaEmitters = new();
    static Material _lavaMat;

    /// <summary>
    /// Children = WallsMeshes, CeilingMeshes, FloorMesh->FloorDarkMesh
    /// </summary>
    static string _spaBase = "BunkerAll/BE_SpaA_L_Baked/Offset/Level";

    /// <summary>
    /// Children:
    /// PanelFloorA (n) > set all to lava
    /// PanelFloorB (n) > the ones next to the Spa
    /// FloorMatCentralA -> FloorMatCentralALOD0MESH > floor around the lava
    /// FloorMatA -> 6 child > the ones left to the lava
    /// RoofPanelingA -> RoofPanelingALOD0MESH > most of the roof around the lava
    /// </summary>
    static string _gymCBase = "BunkerAll/Gyms/BE_GymC_Baked/LAYOUT";

    /// <summary>
    /// PanelFloorA (n) > set all to lava
    /// PanelFloorB (n) > the ones next to the Gym C
    /// FloorMatCentralA -> FloorMatCentralALOD0MESH > floor around the lava
    /// FloorMatA -> 6 child > next to the pool entrance
    /// RoofPanelingA -> RoofPanelingALOD0MESH > most of the roof around the lava
    /// </summary>
    static string _gymBBase = "BunkerAll/Gyms/BE_GymB_Baked/LAYOUT";

    /// <summary>
    /// PanelFloorA (n) > the ones inside the glass structure
    /// PanelFloorB (n) > the ones next to the Gym B
    /// BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/FloorMatCentralA -> FloorMatCentralALOD0MESH > floor around the glass structure
    /// BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/FloorMatA -> 6 child > next to the door exit
    /// RoofPanelingA -> RoofPanelingALOD0MESH > most of the roof
    /// </summary>
    static string _gymABase = "BunkerAll/Gyms/BE_GymA_Baked/LAYOUT";

    public void Start()
    {
        SceneManager.GetSceneByName("BunkerLuxury").GetRootGameObjects().FirstWithName("CaveEInternal-Local").SetActive(true);
        _lavaMat = GameObject.Find("CaveEInternal-Local/LAVA/LakeCaveEA/_LakeCaveEARenderable_/_LakeCaveEA_/_LakeCaveEALOD0_").GetComponent<MeshRenderer>().sharedMaterial;

        var sh = Shader.Find("HDRP/Lit");
        LavaTerrainMat.LavaTerrain.GetComponent<MeshRenderer>().sharedMaterial.shader = sh;
        BlackScribbleMat.BlackScribble.GetComponent<MeshRenderer>().sharedMaterial.shader = sh;
        GreenScribbleMat.GreenScribble.GetComponent<MeshRenderer>().sharedMaterial.shader = sh;
        LavaFloorMat.LavaFloor.GetComponent<MeshRenderer>().sharedMaterial.shader = sh;

        SwapMaterials();
    }

    public static void SwapMaterials()
    {
        SwapSpa();
        SwapGym();
    }

    private static void SwapSpa()
    {
        var blackScribble = BlackScribbleMat.BlackScribble.GetComponent<MeshRenderer>().sharedMaterial;
        var gridLine = LavaTerrainMat.LavaTerrain.GetComponent<MeshRenderer>().sharedMaterial;

        GameObject.Find($"{_spaBase}/FloorMesh").GetChildren().ForEach(child =>
        {
            child.parent.GetComponent<MeshRenderer>().sharedMaterial = blackScribble;
            child.GetComponent<MeshRenderer>().sharedMaterial = gridLine;
        });

        GameObject.Find($"{_spaBase}/CeilingMeshes").GetChildren().ForEach(child =>
        {
            child.GetComponent<MeshRenderer>().sharedMaterial = gridLine;
        });

        GameObject.Find($"{_spaBase}/WallsMeshes").GetChildren().ForEach(child =>
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                child.GetComponent<MeshRenderer>().sharedMaterial = blackScribble;
            }
        });

        GameObject.Find("BunkerAll/BE_SpaA_L_Baked/Offset/Reception/ReceptionWallSide (1)")
            .GetComponent<MeshRenderer>().sharedMaterial = blackScribble;
        GameObject.Find("BunkerAll/BE_SpaA_L_Baked/Offset/Reception/ReceptionWallSide")
            .GetComponent<MeshRenderer>().sharedMaterial = blackScribble;
        GameObject.Find("BunkerAll/BE_SpaA_L_Baked/Offset/Reception/Cube_014_8.001")
            .GetComponent<MeshRenderer>().sharedMaterial = blackScribble;
    }

    public static void SwapGym()
    {
        var lavaFloor = LavaFloorMat.LavaFloor.GetComponent<MeshRenderer>().sharedMaterial;

        // Gym C
        GameObject.Find($"{_gymCBase}").GetChildren().Where(ch => ch.name.Contains("PanelFloorA")).ToList().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = _lavaMat;
            _lavaEmitters.Add(floor.gameObject.AddComponent<SonsFMODEventEmitter>());

            var box = floor.GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(box.size.x, 0.2f, box.size.z);

            floor.gameObject.AddComponent<LavaPanel>();
        });

        GameObject.Find($"{_gymCBase}").GetChildren().Where(ch => ch.name.Contains("PanelFloorB")).ToList().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = lavaFloor;
        });

        var a = GameObject.Find($"{_gymCBase}/FloorMatCentralA/FloorMatCentralALOD0MESH");
        var am = a.GetComponent<MeshRenderer>().sharedMaterials;
        for (var i = 0; i < am.Length; i++)
        {
            am[i] = lavaFloor;
        }
        a.GetComponent<MeshRenderer>().sharedMaterials = am;

        GameObject.Find($"{_gymCBase}/FloorMatA").GetChildren().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = lavaFloor;
        });

        // Gym B
        GameObject.Find($"{_gymBBase}").GetChildren().Where(ch => ch.name.Contains("PanelFloorA")).ToList().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = _lavaMat;
            _lavaEmitters.Add(floor.gameObject.AddComponent<SonsFMODEventEmitter>());

            var box = floor.GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(box.size.x, 0.2f, box.size.z);

            floor.gameObject.AddComponent<LavaPanel>();
        });

        GameObject.Find($"{_gymBBase}").GetChildren().Where(ch => ch.name.Contains("PanelFloorB")).ToList().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = lavaFloor;
        });

        var b = GameObject.Find($"{_gymBBase}/FloorMatCentralA/FloorMatCentralALOD0MESH");
        var bm = b.GetComponent<MeshRenderer>().sharedMaterials;
        for (var i = 0; i < bm.Length; i++)
        {
            bm[i] = lavaFloor;
        }
        b.GetComponent<MeshRenderer>().sharedMaterials = bm;

        GameObject.Find($"{_gymBBase}/FloorMatA").GetChildren().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = lavaFloor;
        });

        // Gym A
        GameObject.Find($"{_gymABase}").GetChildren().Where(ch => ch.name.Contains("PanelFloorA")).ToList().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = _lavaMat;
            _lavaEmitters.Add(floor.gameObject.AddComponent<SonsFMODEventEmitter>());

            var box = floor.GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(box.size.x, 0.2f, box.size.z);

            floor.gameObject.AddComponent<LavaPanel>();
        });

        GameObject.Find($"{_gymABase}").GetChildren().Where(ch => ch.name.Contains("PanelFloorB")).ToList().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = lavaFloor;
        });

        var c = GameObject.Find("BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/FloorMatCentralA/FloorMatCentralALOD0MESH");
        var cm = c.GetComponent<MeshRenderer>().sharedMaterials;
        for (var i = 0; i < cm.Length; i++)
        {
            cm[i] = lavaFloor;
        }
        c.GetComponent<MeshRenderer>().sharedMaterials = cm;

        GameObject.Find($"BunkerAll/Gyms/BE_GymA_Baked/SETDRESSING/FloorMatA").GetChildren().ForEach(floor =>
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial = lavaFloor;
        });

        foreach (var emitter in _lavaEmitters)
        {
            AudioController.PlayBSound(emitter, "event:/Ambient/lava-loop", AudioController.SoundType.Sfx);
        }
    }

    void Update()
    {
        foreach (var emitter in _lavaEmitters)
        {
            emitter.instance.set3DAttributes(emitter.gameObject.transform.position.To3DAttributes());
        }
    }

    [RegisterTypeInIl2Cpp]
    public class LavaPanel : MonoBehaviour
    {
        /*
        void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {              
                DebugConsole.HitLocalPlayer(1);
            }
        }
        */

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Player.IsOnLava = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                Player.IsOnLava = false;
            }
        }
    }
}
