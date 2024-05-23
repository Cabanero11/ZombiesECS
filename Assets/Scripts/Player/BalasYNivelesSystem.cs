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
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();

        // Obtener todas las entidades de balas
        NativeArray<Entity> entidadesBalas = entityManager.GetAllEntities();
        PlayerDañoData playerDañoData = entityManager.GetComponentData<PlayerDañoData>(playerEntity);
        DisparoData disparoData = entityManager.GetComponentData<DisparoData>(playerEntity);

        // Singleton para el mundo de física
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
                NativeList<ColliderCastHit> colliderCastHits = new NativeList<ColliderCastHit>(Allocator.Temp);

                float3 punto1 = new float3(balaTransform.Position - balaTransform.Right() * 0.15f);
                float3 punto2 = new float3(balaTransform.Position + balaTransform.Right() * 0.15f);
                float radio = balaData.tamañoBala / 2;
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
                    for (int i = 0; i < colliderCastHits.Length; i++)
                    {
                        Entity entidadColisionada = colliderCastHits[i].Entity;

                        if (entityManager.HasComponent<EnemigosPropiedades>(entidadColisionada))
                        {
                            EnemigosPropiedades enemigosPropiedades = entityManager.GetComponentData<EnemigosPropiedades>(entidadColisionada);
                            float dañoRestante = playerDañoData.dañoBalaJugador;

                            // NIVELES Y EXPERIENCIA DEL JUGADOR
                            // PlayerDañoData se inicializa en DisparoMono.cs
                            // Si 
                            if (enemigosPropiedades.vidaEnemigos <= dañoRestante)
                            {
                                dañoRestante -= enemigosPropiedades.vidaEnemigos;
                                enemigosPropiedades.vidaEnemigos = 0;

                                entityManager.DestroyEntity(entidadColisionada);
                                playerDañoData.puntuacion += 10;
                                playerDañoData.experienciaActualJugador += playerDañoData.experienciaObtenidaPorMatarEnemigo;

                                // Subir de nivel si se alcanza la experiencia necesaria
                                if (playerDañoData.experienciaActualJugador >= playerDañoData.experienciaParaProximoNivel)
                                {
                                    playerDañoData.nivelJugador++;
                                    playerDañoData.experienciaActualJugador -= playerDañoData.experienciaParaProximoNivel;
                                    playerDañoData.experienciaParaProximoNivel *= 1.2f;
                                }

                                // Me faltaba cambiar los valores creo, si, si hacia falta 
                                entityManager.SetComponentData(playerEntity, playerDañoData);
                                entityManager.SetComponentData(playerEntity, disparoData);

                                // Destruir la bala actual
                                entityManager.DestroyEntity(bala);
                                break;
                            }
                            // Recibir daño de forma normal
                            else
                            {
                                enemigosPropiedades.vidaEnemigos -= dañoRestante;
                                entityManager.SetComponentData(entidadColisionada, enemigosPropiedades);

                                // La bala continúa, no la destruimos
                                break;
                            }
                        }
                    }
                }

                colliderCastHits.Dispose();
            }
        }

        entidadesBalas.Dispose();
    }
}
