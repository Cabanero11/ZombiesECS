using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
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

        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);

        // Forma xd de hacerlo pero weno, itero sobre cada entidad, y miro cuales son Enemigos
        NativeArray<Entity> entidades = entityManager.GetAllEntities(Allocator.Temp);

        foreach (Entity ent in entidades)
        {
            if (entityManager.HasComponent<EnemigosPropiedades>(ent))
            {
                LocalTransform enemigosTransform = entityManager.GetComponentData<LocalTransform>(ent);
                EnemigosPropiedades enemigosPropiedades = entityManager.GetComponentData<EnemigosPropiedades>(ent);

                // Mover enemigos hacia el jugador 
                float3 direccionAlJugador = math.normalize(playerTransform.Position - enemigosTransform.Position);

                // Subio de nivel el jugador, los enemigos son mas rapidos
                if (playerDamage.nivelJugador == playerDamage.nivelSiguiente)
                {
                    enemigosPropiedades.velocidadEnemigos += 1f;
                }

                // Reducir velocidad si el enemigo está demasiado cerca del jugador (Asi hacer mas facil esquivarlos cerca del jugador) :D
                float distanciaAlJugador = math.distance(playerTransform.Position, enemigosTransform.Position);

                if (distanciaAlJugador < enemigosPropiedades.radioReducirVelocidad)
                {
                    enemigosPropiedades.velocidadEnemigos *= enemigosPropiedades.factorReduccionVelocidad;
                }

                enemigosTransform.Position.x += enemigosPropiedades.velocidadEnemigos * SystemAPI.Time.DeltaTime * direccionAlJugador.x;
                enemigosTransform.Position.y = 0.80f;  // Sino se quedan debajo del suelo xd
                enemigosTransform.Position.z += enemigosPropiedades.velocidadEnemigos * SystemAPI.Time.DeltaTime * direccionAlJugador.z;

                float direccion = GetRotationEnemigos(enemigosTransform.Position, playerTransform.Position);

                // Animación de lado a lado
                float tiempoActual = (float)SystemAPI.Time.ElapsedTime;
                float anguloAndar = 0.1f * math.sin(5f * tiempoActual);

                enemigosTransform.Rotation = quaternion.Euler(0, direccion, anguloAndar);

                entityManager.SetComponentData(ent, enemigosTransform);
            }
        }
        entidades.Dispose();
    }

    // Obtener la rotacion para el Zombie que apunte al jugador
    public static float GetRotationEnemigos(float3 enemigoPosition, float3 jugadorPosition)
    {
        var x = enemigoPosition.x - jugadorPosition.x;
        var y = enemigoPosition.z - jugadorPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}
