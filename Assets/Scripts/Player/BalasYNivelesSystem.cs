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


        // Coger todas las entidades a la vez
        NativeArray<Entity> entidadesBalas = entityManager.GetAllEntities();
        // Variable para niveles
        PlayerDañoData playerDañoData = entityManager.GetComponentData<PlayerDañoData>(playerEntity);
        DisparoData disparoData = entityManager.GetComponentData<DisparoData>(playerEntity);

        // DETECTAR COLISIONES
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach(Entity ent in entidadesBalas)
        {
            // Filtrar a entidades de tipo BalasData y BalasTiempoMono
            if (entityManager.HasComponent<BalasData>(ent) && entityManager.HasComponent<BalasTiempoMono>(ent))
            {
                // Coger Transform y balasData de la entidad
                LocalTransform balasTransform = entityManager.GetComponentData<LocalTransform>(ent);
                BalasData balasData = entityManager.GetComponentData<BalasData>(ent);

                // Mover las balas en direccion hacia delante
                balasTransform.Position += balasData.velocidadBala * SystemAPI.Time.DeltaTime * balasTransform.Forward();

                entityManager.SetComponentData(ent, balasTransform);

                BalasTiempoMono balasTiempo = entityManager.GetComponentData<BalasTiempoMono>(ent);

                balasTiempo.balasTiempoDesaparicion -= SystemAPI.Time.DeltaTime;

                // Si es 0 su tiempo se detruye, ya que no queremos balas infinitas
                if (balasTiempo.balasTiempoDesaparicion <= 0f)
                {
                    entityManager.DestroyEntity(ent);
                    continue;
                }

                entityManager.SetComponentData(ent, balasTiempo);


                // Detectar layers
                NativeList<ColliderCastHit> colliderCastHits = new NativeList<ColliderCastHit>(Allocator.Temp);

                float3 punto1 = new float3(balasTransform.Position - balasTransform.Right() * 0.15f);
                float3 punto2 = new float3(balasTransform.Position + balasTransform.Right() * 0.15f);
                float radio = balasData.tamañoBala / 2;
                float3 direccion = float3.zero;
                float distanciaMaxima = 1f;

                // Ver si chocamos con una pared o un enemigo a la vez
                uint capasColision = DevolverCapa.ObtenerCapaTrasColision(CapaColisiones.Wall, CapaColisiones.Enemigo);

                physicsWorldSingleton.CapsuleCastAll(punto1, punto2, radio, direccion, distanciaMaxima, ref colliderCastHits, new CollisionFilter {
                    BelongsTo = (uint)CapaColisiones.Default,
                    CollidesWith = capasColision,
                });



                // SI ha colisionado mas de 1 vez, destruimos la bala
                if(colliderCastHits.Length > 0f)
                {
                    // Iterar por entidades colisionadas y ver si es un Zombie
                    for(int i = 0; i <  colliderCastHits.Length; i++)
                    {
                        Entity entidadColisionada = colliderCastHits[i].Entity;
                        
                        if(entityManager.HasComponent<EnemigosPropiedades>(entidadColisionada))
                        {
                            EnemigosPropiedades enemigosPropiedades = entityManager.GetComponentData<EnemigosPropiedades>(entidadColisionada);

                            enemigosPropiedades.vidaEnemigos -= playerDañoData.dañoBalaJugador;

                            entityManager.SetComponentData(entidadColisionada, enemigosPropiedades);

                            // Si la vida del enemigo es menor a 0 lo destruimos 
                            if (enemigosPropiedades.vidaEnemigos <= 0f)
                            {
                                entityManager.DestroyEntity(entidadColisionada);
                                playerDañoData.puntuacion += 10;

                                // NIVELES Y EXPERIENCIA DEL JUGADOR
                                // PlayerDañoData se inicializa en DisparoMono.cs

                                playerDañoData.experienciaActualJugador += playerDañoData.experienciaObtenidaPorMatarEnemigo;

                                // Subir de nivel si se alcanza la experiencia necesaria
                                if (playerDañoData.experienciaActualJugador >= playerDañoData.experienciaParaProximoNivel)
                                {
                                    playerDañoData.nivelJugador++;
                                    playerDañoData.experienciaActualJugador -= playerDañoData.experienciaParaProximoNivel;
                                    //playerDañoData.dañoBalaJugador += 5f;
                                    //disparoData.velocidadJugador += 2f;

                                    // Para pruebas 1.2 o asi, oficial = 2.3f
                                    playerDañoData.experienciaParaProximoNivel *= 1.2f;
                                    

                                }

                                // Me faltaba cambiar los valores creo, asi si hacia falta 
                                entityManager.SetComponentData(playerEntity, playerDañoData);
                                entityManager.SetComponentData(playerEntity, disparoData);
                            }
                        }
                    }
                    
                    entityManager.DestroyEntity(ent);

                }

                colliderCastHits.Dispose();
            }
        }
    }

    
}
