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
        public Entity puntoDisparo;
        public Entity cameraPosition;
        public Entity cameraHolder;
        public Entity mainCamera;
        public Entity orientation;
    }
}
