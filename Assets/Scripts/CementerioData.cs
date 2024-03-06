using Unity.Entities;
using Unity.Mathematics;

public struct CementerioData : IComponentData   
{
    public float2 CementeryDimesions;
    public float NumberOfTombstoneToSpawn;
    public Entity TumbaPrefab;
}