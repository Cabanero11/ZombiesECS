using Unity.Entities;
using Unity.Mathematics;



namespace Zombies
{
    public struct CementerioData : IComponentData
    {
        public float2 CementeryDimesions;
        public int NumberOfTombstoneToSpawn;
        public Entity TumbaPrefab;
        public float areaGeneradorRadio;

        public Entity ZombiePrefab;
        public float cooldownSpawneoZombies;
    }


    public struct ZombiesSpawnerTiempo : IComponentData
    {
        public float tiempoSpawn; // en segundos
    }


}
