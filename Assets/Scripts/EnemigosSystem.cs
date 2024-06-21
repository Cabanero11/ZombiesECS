using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
using UnityEngine.EventSystems;
using Unity.Entities;
using System.Runtime.CompilerServices;


[BurstCompile]
public partial struct EnemigoSystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;
    private Entity enemigoSpawner;

    private EnemigosData enemigosData;

    // Variable Random
    private Unity.Mathematics.Random numeroRandom;

    // Límites del mapa
    private float3 limiteMin;
    private float3 limiteMax;

    // Valor para ir controlando la oleadaSiguiente
    private int oleadaSiguiente;

    public void OnCreate(ref SystemState state)
    {
        // Obtener numero random a partir del hash del enemigo
        numeroRandom = Unity.Mathematics.Random.CreateFromIndex((uint) enemigosData.GetHashCode());

        // Inicializar los límites del mapa
        limiteMin = new float3(-75f, 0f, -75f);
        limiteMax = new float3(75f, 0f, 75f);

        // Para que debe primero debe esperar a que el EnemigosData, exista, asi no crashea
        state.RequireForUpdate<EnemigosData>();


        oleadaSiguiente = 2;
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
        if (enemigosData.cooldownActualSpawneo <= 0)
        {
            for (int i = 0; i < enemigosData.numeroDeEnemigosSpawneadosPorSegundo; i++)
            {
                EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

                // Para tener 3 tipos de enemigos
                Entity enemigoPrefab;

                // Enemigo prefab
                if (enemigosData.numeroOleada % 3 == 1)
                {
                    enemigoPrefab = enemigosData.enemigoPrefab; 
                }
                // Enemigo Fuerte
                else if (enemigosData.numeroOleada % 3 == 0)
                {
                    enemigoPrefab = enemigosData.enemigoFuertePrefab; 
                }
                // Enemigo Rapido
                else
                {
                    enemigoPrefab = enemigosData.enemigoRapidoPrefab;
                }

                // Creamos enemigo
                Entity enemigoEntidad = entityManager.Instantiate(enemigoPrefab);

                LocalTransform enemigoTransform = entityManager.GetComponentData<LocalTransform>(enemigoEntidad);

                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

                // Calcular posición de spawn
                float3 randomOffset = numeroRandom.NextFloat3Direction() * numeroRandom.NextFloat(enemigosData.distanciaMinimaAlJugador, enemigosData.radioSpawneoEnemigos);
                float3 playerPosition = new float3(playerTransform.Position.x, 0f, playerTransform.Position.z);
                float3 puntoSpawn = math.clamp(playerPosition + randomOffset, limiteMin, limiteMax);

                // Asegurar que el punto de spawn no está demasiado cerca del jugador
                if (math.lengthsq(puntoSpawn - playerPosition) < enemigosData.distanciaMinimaAlJugador * enemigosData.distanciaMinimaAlJugador)
                {
                    randomOffset = math.normalize(randomOffset) * enemigosData.distanciaMinimaAlJugador;
                    puntoSpawn = math.clamp(playerPosition + randomOffset, limiteMin, limiteMax);
                }

                // TRAS CALCULAR POSICION LE ASIGNAMOS LA POSICION Y SU ROTATION AL JUGADOR, mantenemos la Y estatica y evitamos calculos de mas
                enemigoTransform.Position = new float3(puntoSpawn.x, 0.65f, puntoSpawn.z);
                enemigoTransform.Rotation = quaternion.RotateY(GetRotationEnemigos(enemigoTransform.Position, playerPosition));
                enemigoTransform.Scale = 1.3f;

                entityCommandBuffer.SetComponent(enemigoEntidad, enemigoTransform);


               
                
                PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);

                // Si el jugaador sube de nivel, entonces incrementamos la dificultad, entonces los enemigos nuevos que spawneen serán mas dificiles
                if(enemigosData.numeroOleada == oleadaSiguiente)
                {

                    //Debug.Log("Subi de oleada");
                    enemigosData.velocidadNormal += 0.33f;
                    enemigosData.vidaNormal += 5f;

                    enemigosData.velocidadFuerte += 0.20f;
                    enemigosData.vidaFuerte += 10f;

                    enemigosData.velocidadRapido += 0.5f;
                    enemigosData.vidaRapido += 3f;

                    oleadaSiguiente++;
                }


                // INICIALIZAR ENEMIGOS PROPIEDADES segun el tipo de enemigo
                // Enemigo Prefab
                if (enemigosData.numeroOleada % 3 == 1)
                {
                    entityCommandBuffer.AddComponent(enemigoEntidad, new EnemigosPropiedades
                    {
                        vidaEnemigos = enemigosData.vidaNormal,
                        velocidadEnemigos = enemigosData.velocidadNormal,
                        radioReducirVelocidad = 5f,
                        factorReduccionVelocidad = 0.35f
                    });
                }   
                // Enemigo Fuerte
                else if (enemigosData.numeroOleada % 3 == 0)
                {
                    entityCommandBuffer.AddComponent(enemigoEntidad, new EnemigosPropiedades
                    {
                        // Doble de vida de uno normal
                        vidaEnemigos = enemigosData.vidaFuerte,
                        velocidadEnemigos = enemigosData.velocidadFuerte,
                        radioReducirVelocidad = 5f,
                        factorReduccionVelocidad = 0.35f
                    });
                }
                // Enemigo Rapido
                else
                {
                    entityCommandBuffer.AddComponent(enemigoEntidad, new EnemigosPropiedades
                    {
                        vidaEnemigos = enemigosData.vidaRapido,
                        velocidadEnemigos = enemigosData.velocidadRapido,
                        radioReducirVelocidad = 5f,
                        factorReduccionVelocidad = 0.35f
                    });
                }
                

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

            // Incremento el nº de oleada tras hacer todo de la oleada
            enemigosData.numeroOleada++;
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