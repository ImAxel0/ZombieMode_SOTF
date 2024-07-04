using UnityEngine;

namespace ZombieMode.Gameplay;

public class DoorsData // Pos, rot, scale, cost
{
    public struct Gym
    {
        public static Tuple<Vector3, Vector3, Vector3, int> DoubleDoor = new(new Vector3(-1176.37f, 59.81f, 26.1f), new Vector3(0, 9, 0), new Vector3(0.1f, 2.3f, 2.7f), 4000);
        public static Tuple<Vector3, Vector3, Vector3, int> GlassDoor = new(new Vector3(-1162.1f, 60.07f, 35.4f), new Vector3(0, 100, 0), new Vector3(0.1f, 3.2f, 1.7f), 6000);
    }

    public struct Spa
    {
        public static string GlassDoors = "BunkerAll/BE_SpaA_L_Baked/Offset/Props/GlassDoors";
        public static Tuple<Vector3, Vector3, Vector3, int> DoorLeft = new(new Vector3(-1094.5f, 60.5f, 11.27f), new Vector3(0, 191, 0), new Vector3(0.1f, 3.7f, 5.58f), 2500);
        public static Tuple<Vector3, Vector3, Vector3, int> DoorRight = new(new Vector3(-1098.7f, 60.4f, -10.21f), new Vector3(0, 191, 0), new Vector3(0.1f, 3.7f, 5.58f), 2500);
        public static Tuple<Vector3, Vector3, Vector3, int> BigDoor = new(new Vector3(-1127.479f, 60.5f, 6.5064f), new Vector3(0, 10, 0), new Vector3(0.65f, 3.7f, 16.15f), 4000);
    }
}
