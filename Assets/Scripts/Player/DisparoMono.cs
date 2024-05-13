using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class DisparoMono : MonoBehaviour
    {
        public GameObject balaPrefab;
        public int numeroBalasPorDisparo;
        public float balasSpread;
    }


    public class DisparoBaker : Baker<DisparoMono>
    {
        public override void Bake(DisparoMono authoring)
        {
            var disparoEntidad = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(disparoEntidad, new DisparoData
            {
                balaPrefab = GetEntity(authoring.balaPrefab, TransformUsageFlags.Dynamic),
                numeroBalasPorDisparo = authoring.numeroBalasPorDisparo,
                balasSpread = authoring.balasSpread
            });

        }

    }

}

