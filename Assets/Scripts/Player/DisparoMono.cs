using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class DisparoMono : MonoBehaviour
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


    public class DisparoBaker : Baker<DisparoMono>
    {
        public override void Bake(DisparoMono authoring)
        {
            var cementerioEntity = GetEntity(TransformUsageFlags.Dynamic);

            // Añadir a la entidad del Cementerio los datos de CementerioData
            // para asi verlos en el editor
            AddComponent(cementerioEntity, new CementerioData
            {
                //CementeryDimesions = authoring.CementeryDimesions,

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

