using Unity.Entities;
using Unity.Mathematics;



namespace Zombies
{
    public struct DisparoData : IComponentData
    {
        public Entity balaPrefab;
        public int numeroBalasPorDisparo;
        public float balasSpread;
    }
}
