using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;



namespace Zombies
{
    public struct DisparoData : IComponentData
    {
        public Entity balaPrefab;
        public int numeroBalasPorDisparo;
        public float balasSpread;
    }
}
