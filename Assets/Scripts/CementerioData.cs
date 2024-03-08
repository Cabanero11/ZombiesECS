using Unity.Entities;
using Unity.Mathematics;



namespace Zombies
{
    public struct CementerioData : IComponentData
    {
        public float2 CementeryDimesions;
        public int NumberOfTombstoneToSpawn;
        public Entity TumbaPrefab;
    }
}
