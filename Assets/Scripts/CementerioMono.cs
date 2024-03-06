using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



public class CementerioMono : MonoBehaviour
{
    public float2 CementeryDimesions;
    public float NumberOfTombstoneToSpawn;
    public GameObject TumbaPrefab;

    // Seed random, valor como 100
    public uint RandomSeed;
}


public class CementerioBaker : Baker<CementerioMono>
{
    public override void Bake(CementerioMono authoring)
    {
        var cementerioEntity = GetEntity(TransformUsageFlags.Dynamic);

        // Añadir a la entidad del Cementerio los datos de CementerioData
        // para asi verlos en el editor
        AddComponent(cementerioEntity, new CementerioData
        {
            CementeryDimesions = authoring.CementeryDimesions,
            NumberOfTombstoneToSpawn = authoring.NumberOfTombstoneToSpawn,
            TumbaPrefab = GetEntity(authoring.TumbaPrefab, TransformUsageFlags.Dynamic),
            //ZombiePrefab = GetEntity(authoring.ZombiePrefab, TransformUsageFlags.Dynamic),
            //ZombieSpawnRate = authoring.ZombieSpawnRate
        });

        
        // Generar nº random de Tumbas para spawnear zombies
        AddComponent(cementerioEntity, new CementerioRandom { 
            randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
        });
        
        //AddComponent<ZombieSpawnPoints>(graveyardEntity);
        //AddComponent<ZombieSpawnTimer>(graveyardEntity);
    }
}