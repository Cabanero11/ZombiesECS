using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
using UnityEngine.EventSystems;
using Unity.Entities;


[BurstCompile]
public partial struct EnemigosMoverseSystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Referencias
        entityManager = state.EntityManager;

        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();

        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        // Forma xd de hacerlo pero weno, itero sobre cada entidad, y miro cuales son Enemigos
        NativeArray<Entity> entidades = entityManager.GetAllEntities();

        foreach(Entity ent in entidades)
        {
            if (entityManager.HasComponent<EnemigosPropiedades>(ent))
            {
                LocalTransform enemigosTransform = entityManager.GetComponentData<LocalTransform>(ent);
                EnemigosPropiedades enemigosPropiedades = entityManager.GetComponentData<EnemigosPropiedades>(ent);


                // Mover enemigos hacia el jugador 
                float3 direccionAlJugador = math.normalize(playerTransform.Position - enemigosTransform.Position);

                enemigosTransform.Position.x += enemigosPropiedades.velocidadEnemigos * SystemAPI.Time.DeltaTime * direccionAlJugador.x;
                enemigosTransform.Position.y = 0.65f;  // Sino se quedan debajo del suelo xd
                enemigosTransform.Position.z += enemigosPropiedades.velocidadEnemigos * SystemAPI.Time.DeltaTime * direccionAlJugador.z;

                //enemigosTransform.Rotation = quaternion.RotateY(GetRotationEnemigos(enemigosTransform.Position, playerTransform.Position));

                float direccion = GetRotationEnemigos(enemigosTransform.Position, playerTransform.Position);

                // Animación de lado a lado
                float tiempoActual = (float)SystemAPI.Time.ElapsedTime;
                float anguloAndar = 0.1f * math.sin(5f * tiempoActual);
                enemigosTransform.Rotation = quaternion.Euler(0, direccion, anguloAndar);


                entityManager.SetComponentData(ent, enemigosTransform);
            }
        }
    }

    // Obtener la rotacion para el Zombie que apunte al jugador
    public static float GetRotationEnemigos(float3 enemigoPosition, float3 jugadorPosition)
    {
        var x = enemigoPosition.x - jugadorPosition.x;
        var y = enemigoPosition.z - jugadorPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}