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

    // Variable Random
    private Unity.Mathematics.Random numeroRandom;


    public void OnCreate(ref SystemState state)
    {
        // Obtener numero random a partir del hash del enemigo
        numeroRandom = Unity.Mathematics.Random.CreateFromIndex((uint) enemigosData.GetHashCode());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Referencias
        entityManager = state.EntityManager;
        enemigoSpawner = SystemAPI.GetSingletonEntity<EnemigosData>();
        enemigosData = entityManager.GetComponentData<EnemigosData>(enemigoSpawner);

        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();



        SpawnearOleadaEnemigos(ref state);

    }

    // 5 OLEADAS MAXIMO, la ultima sera de "maximoNumeroDeEnemigos"
    [BurstCompile]
    public void SpawnearOleadaEnemigos(ref SystemState state)
    {
        enemigosData.cooldownActualSpawneo -= SystemAPI.Time.DeltaTime;

        
        // Si acabo el cooldown, spawneamos enemigos
        if(enemigosData.cooldownActualSpawneo <= 0)
        {
            for(int i = 0; i < enemigosData.numeroDeEnemigosSpawneadosPorSegundo; i++)
            {
                EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

                // Creamos enemigo
                Entity enemigoEntidad = entityManager.Instantiate(enemigosData.enemigoPrefab);

                LocalTransform enemigoTransform = entityManager.GetComponentData<LocalTransform>(enemigoEntidad);

                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

                // CALCULAR POSICION DONDE PONER A LOS ENEMIGOS EN UN CIRCULO ALREDEDOR DEL JUGADOR
                float distanciaMinimaAlCuadradro = enemigosData.distanciaMinimaAlJugador * enemigosData.distanciaMinimaAlJugador;

                float3 randomOffset = numeroRandom.NextFloat3Direction() * numeroRandom.NextFloat(enemigosData.distanciaMinimaAlJugador, enemigosData.radioSpawneoEnemigos);

                float3 playerPosition = new float3(playerTransform.Position.x, playerTransform.Position.y, playerTransform.Position.z);

                float3 puntoSpawn = playerPosition + randomOffset;

                float distanciaAlCuadradro = math.lengthsq(puntoSpawn - playerPosition);

                if(distanciaAlCuadradro < distanciaMinimaAlCuadradro)
                {
                    puntoSpawn = playerPosition + math.normalize(randomOffset) * math.sqrt(distanciaAlCuadradro);
                }

                // TRAS CALCULAR POSICION LE ASIGNAMOS LA POSICION Y SU ROTATION AL JUGADOR

                enemigoTransform.Position = new float3(puntoSpawn.x, 0.65f, puntoSpawn.z);

                enemigoTransform.Rotation = quaternion.RotateY(GetRotationEnemigos(enemigoTransform.Position, playerPosition));

                enemigoTransform.Scale = 1.3f;

                entityCommandBuffer.SetComponent(enemigoEntidad, enemigoTransform);

                // INICIALIZAR ENEMIGOS PROPIEDADES
                entityCommandBuffer.AddComponent(enemigoEntidad, new EnemigosPropiedades
                {
                    vidaEnemigos = 50f,
                    velocidadEnemigos = 3.5f
                });

                // Realizar todos los cambios que hacemos
                entityCommandBuffer.Playback(entityManager);
                
                // Vaciar el entityCommandBuffer manualmente
                entityCommandBuffer.Dispose();
            }

            // Incrementar el nº de enemigos por oleada
            int numeroDeEnemigosPorOleada = enemigosData.numeroDeEnemigosSpawneadosPorSegundo + enemigosData.incrementoDeNumeroDeEnemigosPorOleada;

            // Spawnean muchos xd, pongo el min
            int enemigosPorOleada = math.min(numeroDeEnemigosPorOleada, enemigosData.maximoNumeroDeEnemigos);

            // Despues del min, actualizo el valor entre el incrementado o el tope
            enemigosData.numeroDeEnemigosSpawneadosPorSegundo = enemigosPorOleada;

            enemigosData.cooldownActualSpawneo = enemigosData.cooldownSpawneoEnemigos;
        }

        entityManager.SetComponentData(enemigoSpawner, enemigosData);
    }

    // Obtener la rotacion para el Zombie que apunte al jugador
    public static float GetRotationEnemigos(float3 enemigoPosition, float3 jugadorPosition)
    {
        var x = enemigoPosition.x - jugadorPosition.x;
        var y = enemigoPosition.z - jugadorPosition.z;

        return math.atan2(x, y) + math.PI;
    }




}