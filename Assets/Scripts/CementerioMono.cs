using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class CementerioMono : MonoBehaviour
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


    public class CementerioBaker : Baker<CementerioMono>
    {
        public override void Bake(CementerioMono authoring)
        {
            var cementerioEntity = GetEntity(TransformUsageFlags.Dynamic);

            // A�adir a la entidad del Cementerio los datos de CementerioData
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


            // Generar n� random de Tumbas para spawnear zombies
            AddComponent(cementerioEntity, new CementerioRandom
            {
                randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
            });

            AddComponent<ZombiesSpawn>(cementerioEntity);
            AddComponent<ZombiesSpawnerTiempo>(cementerioEntity);
        }
    }

}

