using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class GeneradorMono : MonoBehaviour
    {
        public float2 CementeryDimesions;
        public int NumberOfTombstoneToSpawn;
        public GameObject TumbaPrefab;
        public float areaGeneradorRadio;

        // Seed random, valor como 100
        public uint RandomSeed;

        public GameObject ZombiePrefab;
        public float cooldownSpawneoZombies;

    }


    public class GeneradorBaker : Baker<GeneradorMono>
    {
        public override void Bake(GeneradorMono authoring)
        {
            var cementerioEntity = GetEntity(TransformUsageFlags.Dynamic);

            // Añadir a la entidad del Cementerio los datos de CementerioData
            // para asi verlos en el editor
            AddComponent(cementerioEntity, new CementerioData
            {
                CementeryDimesions = authoring.CementeryDimesions,
                NumberOfTombstoneToSpawn = authoring.NumberOfTombstoneToSpawn,
                TumbaPrefab = GetEntity(authoring.TumbaPrefab, TransformUsageFlags.Dynamic),
                areaGeneradorRadio = authoring.areaGeneradorRadio,
                ZombiePrefab = GetEntity(authoring.ZombiePrefab, TransformUsageFlags.Dynamic),
                cooldownSpawneoZombies = authoring.cooldownSpawneoZombies
            });


            // Generar nº random de Tumbas para spawnear zombies
            AddComponent(cementerioEntity, new CementerioRandom
            {
                randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
            });

            AddComponent<ZombiesSpawn>(cementerioEntity);
            AddComponent<ZombiesSpawnerTiempo>(cementerioEntity);
        }
    }

}

