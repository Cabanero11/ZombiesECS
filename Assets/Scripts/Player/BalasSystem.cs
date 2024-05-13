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
public partial struct BalasSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;

        // Coger todas las entidades a la vez
        NativeArray<Entity> entidadesBalas = entityManager.GetAllEntities();

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

                physicsWorldSingleton.CapsuleCastAll(punto1, punto2, radio, direccion, distanciaMaxima, ref colliderCastHits, new CollisionFilter {
                    BelongsTo = (uint)CapaColisiones.Default,
                    CollidesWith = (uint)CapaColisiones.Enemigo,
                });

                // SI ha colisionado mas de 1 vez, destruimos la bala
                if(colliderCastHits.Length > 0f)
                {
                    entityManager.DestroyEntity(ent);
                }

                colliderCastHits.Dispose();
            }
        }
    }

    
}
