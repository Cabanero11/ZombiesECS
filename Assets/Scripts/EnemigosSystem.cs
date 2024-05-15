using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
using UnityEngine.EventSystems;
using Unity.Entities;


[BurstCompile]
public partial struct EnemigoSystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;
    private Entity enemigoSpawner;

    private EnemigosData enemigosData;


    public void OnUpdate(ref SystemState state)
    {
        // Referencias
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();
        enemigoSpawner = SystemAPI.GetSingletonEntity<EnemigosData>();


    }
}