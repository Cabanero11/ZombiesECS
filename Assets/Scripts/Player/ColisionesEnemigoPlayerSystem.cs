using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Zombies;

[BurstCompile]
public partial struct ColisionesEnemigoPlayerSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Obtener el jugador
        Entity playerEntity = SystemAPI.GetSingletonEntity<DisparoData>(); 
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);

        float distanciaDeColision = 0.9f;

        // Obtener todos los enemigos
        NativeArray<Entity> enemigos = entityManager.GetAllEntities(Allocator.Temp);
        foreach (var enemigoEntity in enemigos)
        {
            if (entityManager.HasComponent<EnemigosPropiedades>(enemigoEntity))
            {
                LocalTransform enemigoTransform = entityManager.GetComponentData<LocalTransform>(enemigoEntity);

                // Comprobar colisión simple (puedes mejorar esto con una detección de colisión más precisa)
                if (math.distance(enemigoTransform.Position, playerTransform.Position) < distanciaDeColision)
                {

                    // Aplica daño al jugador
                    playerDamage.vidaJugador -= playerDamage.dañoAlJugador;
                    entityManager.SetComponentData(playerEntity, playerDamage);

                    // Si la vida es menor que 0 se muere el jugador :(
                    if(playerDamage.vidaJugador <= 0)
                    {
                        //entityManager.DestroyEntity(playerEntity);
                        playerDamage.jugadorMuerto = true;
                    }

                    // NO SE SI DESTRUIR ESE ZOMBIE
                    // SI HAY MUCHOS Y SE COMPLICA ESTARIA BIEN JHUM
                    entityManager.DestroyEntity(enemigoEntity);
                }
            }
        }

        enemigos.Dispose();
    }
}