using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;

[BurstCompile]
public partial struct BalasYNivelesSystem : ISystem
{
    private Entity playerEntity;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DisparoData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();

        // Obtener todas las entidades de balas
        NativeArray<Entity> entidadesBalas = entityManager.GetAllEntities();
        PlayerDa�oData playerDa�oData = entityManager.GetComponentData<PlayerDa�oData>(playerEntity);
        DisparoData disparoData = entityManager.GetComponentData<DisparoData>(playerEntity);

        // Singleton para el mundo de f�sica
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (Entity bala in entidadesBalas)
        {
            if (entityManager.HasComponent<BalasData>(bala) && entityManager.HasComponent<BalasTiempoMono>(bala))
            {
                LocalTransform balaTransform = entityManager.GetComponentData<LocalTransform>(bala);
                BalasData balaData = entityManager.GetComponentData<BalasData>(bala);

                // Mover la bala hacia adelante
                balaTransform.Position += balaData.velocidadBala * SystemAPI.Time.DeltaTime * balaTransform.Forward();
                entityManager.SetComponentData(bala, balaTransform);

                BalasTiempoMono balasTiempo = entityManager.GetComponentData<BalasTiempoMono>(bala);
                balasTiempo.balasTiempoDesaparicion -= SystemAPI.Time.DeltaTime;

                if (balasTiempo.balasTiempoDesaparicion <= 0f)
                {
                    entityManager.DestroyEntity(bala);
                    continue;
                }

                entityManager.SetComponentData(bala, balasTiempo);

                // Detectar colisiones
                NativeList<ColliderCastHit> colliderCastHits = new NativeList<ColliderCastHit>(Allocator.TempJob);

                float3 punto1 = balaTransform.Position - balaTransform.Right() * 0.15f;
                float3 punto2 = balaTransform.Position + balaTransform.Right() * 0.15f;
                float radio = balaData.tama�oBala / 2;
                float3 direccion = float3.zero;
                float distanciaMaxima = 1f;

                uint capasColision = DevolverCapa.ObtenerCapaTrasColision(CapaColisiones.Wall, CapaColisiones.Enemigo);
                physicsWorldSingleton.CapsuleCastAll(punto1, punto2, radio, direccion, distanciaMaxima, ref colliderCastHits, new CollisionFilter
                {
                    BelongsTo = (uint)CapaColisiones.Default,
                    CollidesWith = capasColision,
                });

                if (colliderCastHits.Length > 0)
                {
                    bool balaDestruida = false; // Flag para verificar si la bala ya fue destruida

                    for (int i = 0; i < colliderCastHits.Length; i++)
                    {
                        Entity entidadColisionada = colliderCastHits[i].Entity;

                        if (entityManager.HasComponent<EnemigosPropiedades>(entidadColisionada))
                        {
                            EnemigosPropiedades enemigosPropiedades = entityManager.GetComponentData<EnemigosPropiedades>(entidadColisionada);
                            float da�oRestante = playerDa�oData.da�oBalaJugador;

                            // NIVELES Y EXPERIENCIA DEL JUGADOR
                            // PlayerDa�oData se inicializa en DisparoMono.cs
                            if (enemigosPropiedades.vidaEnemigos <= da�oRestante)
                            {
                                da�oRestante -= enemigosPropiedades.vidaEnemigos;
                                enemigosPropiedades.vidaEnemigos = 0;

                                entityManager.DestroyEntity(entidadColisionada);
                                playerDa�oData.puntuacion += 10;
                                playerDa�oData.experienciaActualJugador += playerDa�oData.experienciaObtenidaPorMatarEnemigo;

                                // Subir de nivel si se alcanza la experiencia necesaria
                                if (playerDa�oData.experienciaActualJugador >= playerDa�oData.experienciaParaProximoNivel)
                                {
                                    playerDa�oData.nivelJugador++;
                                    playerDa�oData.experienciaActualJugador -= playerDa�oData.experienciaParaProximoNivel;
                                    playerDa�oData.experienciaParaProximoNivel *= 1.2f;
                                }

                                // Me faltaba cambiar los valores creo, si, si hac�a falta 
                                entityManager.SetComponentData(playerEntity, playerDa�oData);
                                entityManager.SetComponentData(playerEntity, disparoData);

                                // Destruir la bala actual
                                entityManager.DestroyEntity(bala);
                                balaDestruida = true; // Marca la bala como destruida
                                break;
                            }
                            else
                            {
                                enemigosPropiedades.vidaEnemigos -= da�oRestante;
                                entityManager.SetComponentData(entidadColisionada, enemigosPropiedades);

                                // La bala se destruye despu�s de causar da�o
                                entityManager.DestroyEntity(bala);
                                balaDestruida = true; // Marca la bala como destruida
                                break;
                            }
                        }
                    }

                    if (balaDestruida)
                    {
                        break; // Salir del bucle si la bala fue destruida
                    }
                }

                colliderCastHits.Dispose();
            }
        }

        entidadesBalas.Dispose();
    }
}
